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

                this.points[i] = new CircleF(new Point(RandomX, rand.Next(0, camera.ViewportHeight)), rand.Next(30, 50));
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
                if (points[i].Position.Y < camera.Position.Y - 32)
                {
                    points[i].Position = new Point(RandomX, (int) camera.Position.Y + camera.ViewportHeight + 32);
                    points[i].Radius -= 2;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var lineThickness = 3f;
            foreach (var point in points)
            {
                if (point.Radius > 0)
                {
                    spriteBatch.DrawCircle(point, 10, new Color(0.15f, 0.15f, 0.15f), lineThickness, transform.Depth);
                }
            }

        }
    }
}
