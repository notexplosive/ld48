using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace LD48.Components
{
    class Fish : BaseComponent
    {
        private float intertia = 5;
        private float bubbleSpawnTimer;

        public float Size
        {
            get; private set;
        }

        public Vector2 Velocity
        {
            get; private set;
        } = Vector2.Zero;
        public Nullable<Vector2> TargetPosition
        {
            get; set;
        }

        private readonly BubbleSpawner bubbleSpawner;

        public Fish(Actor actor, int sizeLevel) : base(actor)
        {
            Size = sizeLevel * 5;
            TargetPosition = null;
            this.bubbleSpawner = RequireComponent<BubbleSpawner>();
        }

        public override void DebugDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawCircle(new CircleF(transform.Position, Size * 2), 5, Color.Red, 1, transform.Depth);
        }

        public override void Update(float dt)
        {
            var velocityOffset = Vector2.Zero;

            if (TargetPosition.HasValue)
            {
                var direction = (TargetPosition.Value - transform.Position);
                direction.Normalize();
                velocityOffset += direction * dt * 60;
            }


            if (velocityOffset.LengthSquared() > 0)
            {
                Velocity += velocityOffset * dt * this.intertia;
            }

            if (Velocity.Length() < 1 && this.bubbleSpawnTimer < 0)
            {
                this.bubbleSpawnTimer = 2;
                for (int i = 0; i < 5; i++)
                {
                    this.bubbleSpawner.SpawnBubble(transform.Position, -Velocity / 5, i / 20f);
                }
            }
            this.bubbleSpawnTimer -= dt;

            if (Velocity.LengthSquared() > 0)
            {
                transform.Position += Velocity;
            }
        }
    }
}
