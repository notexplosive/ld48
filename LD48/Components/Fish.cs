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

        private readonly Transform playerTransform;
        private Vector2 targetOffset;
        private float targetResetTimer;

        public Vector2 Velocity
        {
            get; private set;
        } = Vector2.Zero;

        private readonly BubbleSpawner bubbleSpawner;
        public readonly FishStats stats;

        public Fish(Actor actor, Transform playerTransform, FishStats stats) : base(actor)
        {
            this.playerTransform = playerTransform;
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
            spriteBatch.DrawCircle(new CircleF(this.playerTransform.Position + this.targetOffset, 5), 5, Color.Red, 1, transform.Depth);
        }

        public float HitRadius => Size * 2 + Velocity.Length() * 2.5f;

        public override void Update(float dt)
        {
            var acceleration = Vector2.Zero;

            if (this.playerTransform != null)
            {
                var direction = (playerTransform.Position + this.targetOffset - transform.Position);
                direction.Normalize();
                acceleration += direction * dt * 60;
            }


            if (acceleration.LengthSquared() > 0)
            {
                Velocity += acceleration * dt * this.stats.Inertia * MachinaGame.Random.CleanRandom.Next(3);
            }

            if (this.targetResetTimer < 0)
            {
                this.targetOffset = CalculateTargetOffset();
                this.targetResetTimer = this.stats.AttentionSpan + MachinaGame.Random.CleanRandom.Next(-2, 2);
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
