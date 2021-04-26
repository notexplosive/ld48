using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace OculusLeviathan.Components
{
    class ResetTargetOffsetPeriodically : BaseComponent
    {
        private float targetResetTimer;
        private readonly Fish fish;

        public ResetTargetOffsetPeriodically(Actor actor) : base(actor)
        {
            this.fish = RequireComponent<Fish>();
        }

        public override void Update(float dt)
        {
            if (this.targetResetTimer < 0)
            {
                this.fish.SetRandomTargetOffset();
                this.targetResetTimer = this.fish.stats.LingerTime * MachinaGame.Random.CleanRandom.NextSingle(0.5f, 1.5f);
            }
            this.targetResetTimer -= dt;
        }
    }
}
