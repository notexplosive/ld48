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
    class SeaweedRenderer : BaseComponent
    {
        private readonly Seaweed seaweed;

        public SeaweedRenderer(Actor actor) : base(actor)
        {
            this.seaweed = RequireComponent<Seaweed>();
        }

        public override void Update(float dt)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var lineThickness = 3f;
            var color = Color.White;

            if (this.seaweed.HitTimer > 0)
            {
                color = Color.HotPink;
            }
            spriteBatch.DrawLine(transform.Position, transform.Position + seaweed.EndPoint, color, lineThickness, transform.Depth + 1);
            foreach (var node in this.seaweed.nodes)
            {
                spriteBatch.DrawCircle(new CircleF(node.CenterPos, node.radius), 10, color, lineThickness, transform.Depth - 1);
            }
        }
    }
}
