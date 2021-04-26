using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace OculusLeviathan.Components
{
    /// <summary>
    /// Thing the eye looks at, if there are none of these left then the level ends
    /// </summary>
    class PlayerTarget : BaseComponent
    {
        private readonly Player player;

        public PlayerTarget(Actor actor, Player player) : base(actor)
        {
            this.player = player;
            this.player.CandidateTargets.Add(transform);
        }

        public override void OnDelete()
        {
            this.player.CandidateTargets.Remove(transform);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
