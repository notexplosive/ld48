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
        private Vector2 velocity;
        private readonly float maxSize;
        private readonly int circleCount;

        public Bubble(Actor actor, int size, Vector2 startingVelocity, float delay) : base(actor)
        {
            this.delay = delay;
            this.size = 1f;
            this.maxSize = size;
            this.velocity = startingVelocity * MachinaGame.Random.DirtyRandom.Next(10) / 10;
            var circleCount = MachinaGame.Random.DirtyRandom.Next(1, 10) - 7;
            this.circleCount = Math.Clamp(circleCount, 1, 3);
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

            this.transform.Angle += dt * 4;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var cam = this.actor.scene.camera;
            var viewRect = new Rectangle(cam.Position.ToPoint(), new Point(cam.ViewportWidth, cam.ViewportHeight));
            if (!viewRect.Contains(this.actor.transform.Position.ToPoint()))
            {
                return;
            }

            if (delay < 0)
            {
                var lineThickness = 3;
                for (int i = 0; i < this.circleCount; i++)
                {
                    var pos = Vector2.Zero;
                    if (i > 0)
                    {
                        pos = new Vector2(MathF.Sin(transform.Angle), MathF.Cos(transform.Angle)) * this.size * 2;
                    }
                    spriteBatch.DrawCircle(new CircleF(pos + transform.Position, this.size), 15, Color.White, lineThickness, transform.Depth);
                }
            }
        }
    }
}
