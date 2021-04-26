using OculusLeviathan.Data;
using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace OculusLeviathan.Components
{
    public class TextCrawlRenderer : BaseComponent
    {
        private readonly BoundedTextRenderer textRenderer;
        private readonly LevelTransition levelTransition;

        private TextCrawl CurrentTextCrawl => this.levelTransition.CurrentTextCrawl;
        public TextCrawlRenderer(Actor actor, LevelTransition levelTransition) : base(actor)
        {
            this.textRenderer = RequireComponent<BoundedTextRenderer>();
            this.levelTransition = levelTransition;
        }

        public override void Update(float dt)
        {
            if (CurrentTextCrawl != null)
            {
                this.textRenderer.Text = CurrentTextCrawl.CurrentText;
            }
            else
            {
                this.textRenderer.Text = "";
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
