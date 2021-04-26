using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace OculusLeviathan.Components
{
    class Jellyfish : BaseComponent
    {
        private readonly Fish fish;
        public bool HasEnded
        {
            get; private set;
        }

        private LevelTransition transition;

        public float HitTimer
        {
            get; private set;
        }

        public Jellyfish(Actor actor, LevelTransition transition) : base(actor)
        {
            this.fish = RequireComponent<Fish>();
            this.HasEnded = false;
            this.transition = transition;
            transition.onFallAsleep += RunAwayAndDie;
        }


        public override void OnDelete()
        {
            transition.onFallAsleep -= RunAwayAndDie;
        }

        public override void Update(float dt)
        {
            HitTimer -= dt;

            if (this.HasEnded)
            {
                this.fish.SetTargetOffset(new Vector2(5000, 0));
            }
        }

        private void RunAwayAndDie()
        {
            if (!this.HasEnded)
            {
                this.HasEnded = true;
                new DestroyTimer(this.actor, 10);
            }
        }

        public void Hit()
        {
            this.fish.SetRandomTargetOffset();
            HitTimer = 1;
        }

        public bool IsHit(Vector2 position, float lureSize)
        {
            var f = lureSize + this.fish.Size;
            return (this.actor.transform.Position - position).LengthSquared() < f * f;
        }
    }
}
