using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace OculusLeviathan.Components
{
    class WorldStuff : BaseComponent
    {
        private SoundEffectInstance ambientSound;


        public WorldStuff(Actor actor) : base(actor)
        {
            this.ambientSound = MachinaGame.Assets.GetSoundEffectInstance("underwater-ambience");
            this.ambientSound.IsLooped = true;
            this.ambientSound.Play();
            this.ambientSound.Volume = 0.5f;
        }

        public override void Update(float dt)
        {
            this.ambientSound.Pitch = this.actor.scene.TimeScale - 1;
        }

        public override void OnKey(Keys key, ButtonState state, ModifierKeys modifiers)
        {
            if (key == Keys.F4 && modifiers.None && state == ButtonState.Released)
            {
                MachinaGame.Fullscreen = !MachinaGame.Fullscreen;
            }
        }
    }
}
