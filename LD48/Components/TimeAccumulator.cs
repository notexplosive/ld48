using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace OculusLeviathan.Components
{
    class TimeAccumulator : BaseComponent
    {
        private float time;

        public float Sin(float f) => MathF.Sin(time * f);
        public float Cos(float f) => MathF.Cos(time * f);

        public TimeAccumulator(Actor actor) : base(actor)
        {
            this.time = 0;
        }

        public override void Update(float dt)
        {
            this.time += dt;
        }
    }
}
