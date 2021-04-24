using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace LD48.Components
{
    class Bubble : BaseComponent
    {
        private float delay;
        private float size;
        private float maxSize;
        private Vector2 velocity;

        public Bubble(Actor actor, int size, Vector2 startingVelocity, float delay) : base(actor)
        {
            this.delay = delay;
            this.size = 1f;
            this.maxSize = size;
            this.velocity = startingVelocity * MachinaGame.Random.DirtyRandom.Next(10) / 10;
        }

        public override void Update(float dt)
        {
            this.delay -= dt;

            if (delay < 0)
            {
                this.size = size *= 2;
                if (this.size > this.maxSize)
                {
                    this.size = maxSize;
                }

                this.velocity.Y -= dt * 5;
                transform.Position += this.velocity * dt * 60;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (delay < 0)
            {
                var lineThickness = 3;
                spriteBatch.DrawCircle(new CircleF(transform.Position, this.size), 15, Color.White, lineThickness, transform.Depth);
            }
        }
    }
}
