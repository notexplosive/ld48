using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace LD48.Components
{
    class NewTargetOffsetWhenReachesCurrent : BaseComponent
    {
        private readonly Fish fish;

        public NewTargetOffsetWhenReachesCurrent(Actor actor) : base(actor)
        {
            this.fish = RequireComponent<Fish>();
        }

        public override void Update(float dt)
        {
            var distanceFromTarget = (this.actor.transform.Position - this.fish.TargetPosition).Length();
            if (distanceFromTarget < this.fish.Size)
            {
                this.fish.SetRandomTargetOffset();
            }
        }
    }
}
