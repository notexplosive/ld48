using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LD48.Components
{
    class FishRenderer : BaseComponent
    {
        private readonly TimeAccumulator time;
        private readonly Fish fish;

        public FishRenderer(Actor actor) : base(actor)
        {
            this.time = RequireComponent<TimeAccumulator>();
            this.fish = RequireComponent<Fish>();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var vel = this.fish.Velocity;
            var velLength = vel.Length();
            var lengthOffset = velLength * 1.5f;
            var lineThickness = 6f;
            var halfBodyWidth = this.fish.Size * 1.25f + lengthOffset;
            spriteBatch.DrawEllipse(transform.Position, new Vector2(halfBodyWidth, this.fish.Size), 16, Color.White, lineThickness, transform.Depth);


            float flipFactor = Math.Clamp(-vel.X, -1, 1);

            var tailStart = transform.Position + new Vector2(halfBodyWidth, 0) * flipFactor;

            var tailWidth = this.fish.Size;
            var tailHeight = this.fish.Size;
            var wagSpeed = Math.Min(velLength / 10, 1);
            var wagIntensity = 5;
            var xAnim = this.time.Cos(wagSpeed * 2) * wagIntensity;
            var yAnim = this.time.Sin(wagSpeed) * wagIntensity;

            var tailTop = tailStart + new Vector2(flipFactor * tailWidth + xAnim, tailHeight + yAnim);
            var tailBottom = tailStart + new Vector2(flipFactor * tailWidth + xAnim, -tailHeight - yAnim);

            spriteBatch.DrawLine(tailStart, tailTop, Color.White, lineThickness, transform.Depth);
            spriteBatch.DrawLine(tailStart, tailBottom, Color.White, lineThickness, transform.Depth);
            spriteBatch.DrawLine(tailTop, tailBottom, Color.White, lineThickness, transform.Depth);
        }
    }
}
