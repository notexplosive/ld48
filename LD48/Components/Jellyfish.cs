﻿using Machina.Components;
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
        private Fish fish;

        public Jellyfish(Actor actor, Player player) : base(actor)
        {
            this.fish = RequireComponent<Fish>();
            player.onFallAsleep += RunAwayAndDie;
        }

        private void RunAwayAndDie()
        {
            this.fish.SetTargetOffset(new Vector2(5000, 0));
            new DestroyTimer(this.actor, 5);
        }

        public override void Update(float dt)
        {

        }
    }
}
