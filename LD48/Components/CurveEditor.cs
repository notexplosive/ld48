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
    class CurveEditor : BaseComponent
    {
        private float time;

        public CurveEditor(Actor actor) : base(actor)
        {
            for (int i = 0; i < 4; i++)
            {
                var child = transform.AddActorAsChild("node");
                new BoundingRect(child, new Point(32, 32)).SetOffsetToCenter();
                new Hoverable(child);
                new Clickable(child);
                new Draggable(child);
                new MoveOnDrag(child);
                new BoundingRectRenderer(child);
            }

            this.time = 0f;
        }

        public override void Update(float dt)
        {
            this.time += dt * 5;
        }

        public override void OnKey(Keys key, ButtonState state, ModifierKeys modifiers)
        {
            if (key == Keys.T && state == ButtonState.Pressed && modifiers.Control)
            {
                MachinaGame.Print("--");
                for (var i = 0; i < transform.ChildCount; i++)
                {
                    MachinaGame.Print(transform.ChildAt(i).transform.LocalPosition);
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var curve = CatmullRomCurve.Create(25,
                transform.ChildAt(0).transform.Position,
                transform.ChildAt(1).transform.Position,
                transform.ChildAt(2).transform.Position,
                transform.ChildAt(3).transform.Position);

            for (int i = 0; i < curve.Count - 1; i++)
            {
                if (this.time > i && this.time < i + 1)
                {
                    spriteBatch.DrawCircle(new CircleF(curve.points[i], 5f), 3, Color.White, 1f, transform.Depth);
                }
            }

            if (time > curve.Count)
            {
                time = 0;
            }

            curve.Draw(spriteBatch, Vector2.Zero, transform.Depth, 1f);
        }
    }
}
