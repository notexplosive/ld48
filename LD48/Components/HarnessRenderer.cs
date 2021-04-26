using LD48.Data;
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
    class HarnessRenderer : BaseComponent
    {
        private readonly BubbleSpawner bubbleSpawner;
        private readonly LevelTransition levelTransition;
        private readonly CatmullRomCurve[] wires = Array.Empty<CatmullRomCurve>();
        private float wireTimer = 0f;
        private CatmullRomCurve currentWire;
        private float totalTimer;

        public HarnessRenderer(Actor actor) : base(actor)
        {
            this.bubbleSpawner = RequireComponent<BubbleSpawner>();
            this.levelTransition = RequireComponent<LevelTransition>();
            wires = new CatmullRomCurve[6];
            wires[0] = CatmullRomCurve.Create(25, new Vector2(MachinaGame.Random.DirtyRandom.Next(-100, 100) + 100, -140), new Vector2(-40, -200), new Vector2(-69, -100), new Vector2(100, -96));
            wires[1] = CatmullRomCurve.Create(25, new Vector2(MachinaGame.Random.DirtyRandom.Next(-100, 100) + 100, -140), new Vector2(40, -200), new Vector2(60, -110), new Vector2(-100, -96));
            wires[2] = CatmullRomCurve.Create(25, new Vector2(MachinaGame.Random.DirtyRandom.Next(-100, 100) + 100, -140), new Vector2(40, -200), new Vector2(30, -115), new Vector2(-100, -96));
            wires[3] = CatmullRomCurve.Create(25, new Vector2(MachinaGame.Random.DirtyRandom.Next(-100, 100) + 100, -140), new Vector2(-40, -200), new Vector2(-30, -120), new Vector2(-100, -96));
        }

        public override void Update(float dt)
        {
            this.wireTimer += dt * 5;
            this.totalTimer += dt;

            if (this.wireTimer > 1)
            {
                this.wireTimer = 0;
                this.currentWire = this.wires[MachinaGame.Random.DirtyRandom.Next(this.wires.Length)];
            }

            var rand = MachinaGame.Random.DirtyRandom;
            if (this.levelTransition.CurrentLevel.DamagedHarness && rand.NextDouble() < 0.03)
            {
                var harnessRect = GetHarnessRect;
                var vel = new Vector2(rand.Next(-5, 5), rand.Next(10, 15));
                this.bubbleSpawner.SpawnBubble(harnessRect.Center.ToVector2(), vel, 0);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var lineThickness = 3f;
            foreach (var curve in this.wires)
            {
                if (curve != null)
                {
                    curve.Draw(spriteBatch, transform.Position, transform.Depth, lineThickness);

                    spriteBatch.DrawCircle(new CircleF(curve.points[0] + transform.Position, 5), 5, Color.White, lineThickness, transform.Depth);
                    spriteBatch.DrawCircle(new CircleF(curve.points[^1] + transform.Position, 5), 5, Color.White, lineThickness, transform.Depth);

                    if (curve == this.currentWire)
                    {
                        // Electrical impulse
                        spriteBatch.DrawCircle(new CircleF(curve.GetPointAlong(this.wireTimer) + transform.Position, 7), 7, Color.Yellow, 7, transform.Depth - 1);
                    }
                }
            }

            spriteBatch.DrawRectangle(GetHarnessRect, Color.White, lineThickness, transform.Depth);
        }

        private Rectangle GetHarnessRect
        {
            get
            {
                var offset = new Point(0, 0);
                if (this.levelTransition.CurrentLevel.DamagedHarness)
                {
                    var rand = MachinaGame.Random.DirtyRandom;
                    offset = new Point((int) (MathF.Sin(this.totalTimer * 40) * 5), 0);
                }
                return new Rectangle(new Point(-50, -300) + transform.Position.ToPoint() + offset, new Point(100, 100));
            }
        }

    }
}
