using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace LD48.Components
{
    class Fish : BaseComponent
    {
        private float intertia = 5;

        public float Size
        {
            get; private set;
        }

        public Vector2 Velocity
        {
            get; private set;
        } = Vector2.Zero;
        public Nullable<Vector2> TargetPosition
        {
            get; set;
        }

        public Fish(Actor actor, int sizeLevel) : base(actor)
        {
            Size = sizeLevel * 5;
            TargetPosition = null;
        }

        public override void Update(float dt)
        {
            var velocityOffset = Vector2.Zero;

            if (TargetPosition.HasValue)
            {
                var direction = (TargetPosition.Value - transform.Position);
                direction.Normalize();
                velocityOffset += direction;
            }


            if (velocityOffset.LengthSquared() > 0)
            {
                Velocity += velocityOffset * dt * this.intertia;
            }

            if (Velocity.LengthSquared() > 0)
            {
                transform.Position += Velocity;
            }
        }

        public override void OnMouseButton(MouseButton button, Vector2 currentPosition, ButtonState state)
        {
            if (button == MouseButton.Left && state == ButtonState.Pressed)
            {
                TargetPosition = currentPosition;
            }
        }
    }
}
