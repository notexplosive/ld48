using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace OculusLeviathan.Components
{
    class Eatable : BaseComponent
    {
        public readonly Fish fish;

        public Eatable(Actor actor) : base(actor)
        {
            this.fish = RequireComponent<Fish>();
        }

        public override void Update(float dt)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
