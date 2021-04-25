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
    class TargetRedical : BaseComponent
    {
        private Player player;
        private float aimTimer;
        private float accumulatedTime;

        public TargetRedical(Actor actor, Player player) : base(actor)
        {
            this.player = player;
        }

        public override void Update(float dt)
        {
            if (this.player.IsAllowedToDeploy && this.player.IsAiming)
            {
                this.aimTimer += dt * 20;
            }
            else
            {
                this.aimTimer -= dt * 30;
            }
            this.aimTimer = Math.Clamp(this.aimTimer, 0, 1);

            this.accumulatedTime += dt;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var lineThickness = 3f;
            var color = this.player.IsAllowedToDeploy ? Color.Yellow : Color.Gray;
            var factor = 1 - this.aimTimer;
            var outerRadius = 16 + 16 * factor;
            spriteBatch.DrawCircle(new CircleF(this.player.MousePos, outerRadius), 16, color, lineThickness, transform.Depth);
            spriteBatch.DrawCircle(new CircleF(this.player.MousePos, 8 + 4 * factor * 2), 16, color, lineThickness, transform.Depth);

            for (float i = 0; i < MathF.PI * 2; i += MathF.PI / 2)
            {
                var theta = this.accumulatedTime * 4 + i;
                var miniRadius = 10;
                spriteBatch.DrawCircle(new CircleF(this.player.MousePos + new Vector2(MathF.Cos(theta), MathF.Sin(theta)) * (outerRadius + miniRadius), miniRadius), 10, color, lineThickness, transform.Depth);
            }
        }
    }
}
