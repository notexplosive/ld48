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
    class BackgroundRenderer : BaseComponent
    {
        private readonly Camera foregroundCamera;
        private readonly CircleF[] points;
        private float scrollFactor;

        public BackgroundRenderer(Actor actor, Camera foregroundCamera, float scrollFactor) : base(actor)
        {
            this.foregroundCamera = foregroundCamera;
            this.points = new CircleF[32];

            var camera = this.actor.scene.camera;
            var rand = MachinaGame.Random.DirtyRandom;

            for (int i = 0; i < 32; i++)
            {

                var edge = (int) (camera.ViewportHeight / camera.Zoom);
                this.points[i] = new CircleF(new Point(RandomX, rand.Next(-edge * 2, edge * 2)), rand.Next(50, 70));
            }

            this.scrollFactor = scrollFactor;
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

            camera.Position = new Vector2(0, foregroundCamera.Position.Y / 2 * this.scrollFactor);
            camera.Zoom = foregroundCamera.Zoom * this.scrollFactor;
            var rand = MachinaGame.Random.DirtyRandom;


            for (int i = 0; i < 32; i++)
            {
                var cameraCenter = camera.Position.Y + camera.ViewportCenter.Y;
                var edgeDisplacement = camera.ViewportHeight / 2 / camera.Zoom;
                var topEdge = cameraCenter - edgeDisplacement;
                var bottomEdge = cameraCenter + edgeDisplacement;

                if (points[i].Position.Y < topEdge - points[i].Radius)
                {
                    points[i].Position = new Point(RandomX, (int) (bottomEdge + (bottomEdge / 4 * (float) MachinaGame.Random.DirtyRandom.NextDouble())) + (int) points[i].Radius);
                    points[i].Radius -= 2;
                }
            }


        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var camera = this.actor.scene.camera;
            float depth = (camera.Position.Y - camera.ViewportHeight * 20) / ((float) camera.ViewportHeight * 30);
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
