using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace LD48.Components
{
    class Jellyfish : BaseComponent
    {
        private readonly Fish fish;
        public float HitTimer
        {
            get; private set;
        }

        public Jellyfish(Actor actor, LevelTransition transition) : base(actor)
        {
            this.fish = RequireComponent<Fish>();
            transition.onFallAsleep += RunAwayAndDie;
        }

        public override void Update(float dt)
        {
            this.HitTimer -= dt;
        }

        private void RunAwayAndDie()
        {
            this.fish.SetTargetOffset(new Vector2(5000, 0));
            new DestroyTimer(this.actor, 10);
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
