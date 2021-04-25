using LD48.Data;
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
        private float bubbleSpawnTimer;

        public float Size => this.stats.SizeLevel * 5;

        private readonly Transform targetTransform;
        private Vector2 targetOffset;
        private float targetResetTimer;

        public Vector2 Velocity
        {
            get; private set;
        } = Vector2.Zero;

        private readonly BubbleSpawner bubbleSpawner;
        public readonly FishStats stats;

        public Fish(Actor actor, Transform targetTransform, FishStats stats) : base(actor)
        {
            this.targetTransform = targetTransform;
            this.targetOffset = CalculateTargetOffset();
            this.bubbleSpawner = RequireComponent<BubbleSpawner>();
            this.stats = stats;
        }

        private Vector2 CalculateTargetOffset()
        {
            var viewportHeight = this.actor.scene.camera.ViewportHeight / 2;
            return new Vector2(MachinaGame.Random.CleanRandom.Next(-viewportHeight, viewportHeight), MachinaGame.Random.CleanRandom.Next(-viewportHeight, viewportHeight));

        }

        public override void DebugDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawCircle(new CircleF(transform.Position, HitRadius), 5, Color.Red, 1, transform.Depth);
            spriteBatch.DrawCircle(new CircleF(this.targetTransform.Position + this.targetOffset, 5), 5, Color.Red, 1, transform.Depth);
        }

        public float HitRadius => Size * 2 + Velocity.Length() * 2.5f;

        public override void Update(float dt)
        {
            var velocityOffset = Vector2.Zero;

            if (this.targetTransform != null)
            {
                var direction = (targetTransform.Position + this.targetOffset - transform.Position);
                direction.Normalize();
                velocityOffset += direction * dt * 60;
            }


            if (velocityOffset.LengthSquared() > 0)
            {
                Velocity += velocityOffset * dt * this.stats.Inertia * MachinaGame.Random.CleanRandom.Next(3);
            }

            if (MachinaGame.Random.CleanRandom.NextDouble() < 0.1 && this.targetResetTimer < 0)
            {
                this.targetOffset = CalculateTargetOffset();
                this.targetResetTimer = MachinaGame.Random.CleanRandom.Next(4, 12);
            }

            this.targetResetTimer -= dt;

            if (this.bubbleSpawnTimer < 0 && MachinaGame.Random.DirtyRandom.NextDouble() < 0.15)
            {
                this.bubbleSpawnTimer = (int) (MachinaGame.Random.DirtyRandom.NextDouble() * 5);
                for (int i = 0; i < 5; i++)
                {
                    this.bubbleSpawner.SpawnBubble(transform.Position, Velocity, i / 20f);
                }
            }
            this.bubbleSpawnTimer -= dt;

            var length = Velocity.Length();
            if (length > this.stats.TerminalSpeed)
            {
                var normalVel = Velocity;
                normalVel.Normalize();
                Velocity = normalVel * this.stats.TerminalSpeed;
            }

            if (Velocity.LengthSquared() > 0)
            {
                transform.Position += Velocity * dt * 60;
            }
        }

        public override void OnKey(Keys key, ButtonState state, ModifierKeys modifiers)
        {
            if (MachinaGame.DebugLevel >= DebugLevel.Passive)
            {
                if (key == Keys.K && modifiers.Control)
                {
                    this.actor.Destroy();
                }
            }
        }
    }
}
