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
        private float size;
        private float maxSize;
        private float velocity;

        public Bubble(Actor actor, int size) : base(actor)
        {
            this.size = 0.01f;
            this.maxSize = size;
            this.velocity = 0;
        }

        public override void Update(float dt)
        {
            this.size = size *= 2;
            if (this.size > this.maxSize)
            {
                this.size = maxSize;
            }

            this.velocity += dt;
            transform.Position += new Vector2(0, -velocity);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var lineThickness = 3;
            spriteBatch.DrawCircle(new CircleF(transform.Position, this.size), 15, Color.White, lineThickness, transform.Depth);
        }
    }
}
