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
        private readonly CircleF[] points;

        public BackgroundRenderer(Actor actor, Camera foregroundCamera) : base(actor)
        {
            this.foregroundCamera = foregroundCamera;
            this.points = new CircleF[32];

            var camera = this.actor.scene.camera;
            var rand = MachinaGame.Random.DirtyRandom;

            for (int i = 0; i < 32; i++)
            {

                this.points[i] = new CircleF(new Point(RandomX, rand.Next(0, camera.ViewportHeight)), rand.Next(50, 70));
            }

        }

        public int RandomX
        {
            get
            {
                var rand = MachinaGame.Random.DirtyRandom;
                var camera = this.actor.scene.camera;

                return (int) camera.ViewportCenter.X + rand.Next(camera.ViewportWidth / 4, camera.ViewportWidth / 2) * ((rand.NextSingle() < 0.5f) ? -1 : 1);
            }
        }

        public override void Update(float dt)
        {
            var camera = this.actor.scene.camera;

            camera.Position = new Vector2(0, foregroundCamera.Position.Y / 2);
            camera.Zoom = foregroundCamera.Zoom;
            var rand = MachinaGame.Random.DirtyRandom;


            for (int i = 0; i < 32; i++)
            {
                if (points[i].Position.Y < camera.Position.Y - points[i].Radius)
                {
                    points[i].Position = new Point(RandomX, (int) camera.Position.Y + camera.ViewportHeight + (int) points[i].Radius);
                    points[i].Radius -= 2;
                }
            }


        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var camera = this.actor.scene.camera;
            float depth = (camera.Position.Y - camera.ViewportHeight * 10) / ((float) camera.ViewportHeight * 20);
            depth = Math.Clamp(depth, 0f, 1f);
            float inverseDepth = 1f - depth;

            this.actor.scene.sceneLayers.BackgroundColor = GetBGColor(0.01f, 0.01f, 0.1f, inverseDepth);

            // var lineThickness = 3f;
            foreach (var point in points)
            {
                if (point.Radius > 0)
                {
                    spriteBatch.DrawCircle(point, 10, GetBGColor(0.01f + depth, 0.05f + depth, 0.5f + depth, inverseDepth), point.Radius, transform.Depth);
                }
            }
        }

        private Color GetBGColor(float r, float g, float b, float inverseDepth)
        {
            if (inverseDepth == 1f)
            {
                return new Color(r, g, b);
            }

            var factor = Math.Clamp(inverseDepth, 0.2f, 1f);

            return new Color(r * factor, g * factor, b * factor);
        }
    }
}
