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
        private readonly Transform playerTransform;
        private Vector2 targetOffset;
        public float Size => this.stats.SizeLevel * 5;
        public Vector2 Velocity
        {
            get; private set;
        } = Vector2.Zero;

        public readonly FishStats stats;

        public Fish(Actor actor, Transform playerTransform, FishStats stats) : base(actor)
        {
            this.playerTransform = playerTransform;
            SetRandomTargetOffset();
            this.stats = stats;
        }

        public void SetRandomTargetOffset()
        {
            var viewportWidth = this.actor.scene.camera.ViewportWidth / 2;
            var viewportHeight = this.actor.scene.camera.ViewportHeight / 2;
            this.targetOffset = new Vector2(MachinaGame.Random.CleanRandom.Next(-viewportWidth, viewportWidth), MachinaGame.Random.CleanRandom.Next(-viewportHeight, viewportHeight));
        }

        public void SetTargetOffset(Vector2 val)
        {
            this.targetOffset = val;
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

            var direction = (playerTransform.Position + this.targetOffset - transform.Position);
            direction.Normalize();
            acceleration += direction * dt * 60 * this.stats.Mass;
            Velocity += acceleration * dt * MachinaGame.Random.CleanRandom.Next(3);

            ClampToTerminalVelocity();
            transform.Position += Velocity * dt * 60;
        }

        private void ClampToTerminalVelocity()
        {
            var length = Velocity.Length();
            if (length > this.stats.TerminalSpeed)
            {
                var normalVel = Velocity;
                normalVel.Normalize();
                Velocity = normalVel * this.stats.TerminalSpeed;
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
