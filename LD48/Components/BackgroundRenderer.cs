using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace LD48.Components
{
    class BackgroundRenderer : BaseComponent
    {
        private readonly Camera foregroundCamera;
        private float cameraPos;
        private List<Vector2> points;

        public BackgroundRenderer(Actor actor, Camera foregroundCamera) : base(actor)
        {
            this.foregroundCamera = foregroundCamera;
            var viewportSize = new Point(this.actor.scene.camera.ViewportWidth, this.actor.scene.camera.ViewportHeight);
        }

        public override void Update(float dt)
        {
            this.cameraPos = -foregroundCamera.Position.Y / 2;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
        }
    }
}
