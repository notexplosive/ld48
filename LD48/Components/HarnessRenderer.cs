using LD48.Data;
using Machina.Components;
using Machina.Data;
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
        private readonly Wire[] wires = Array.Empty<Wire>();
        private float wireTimer = 0f;
        private Wire currentWire;
        private float totalTimer;
        private Point harnessOffset;

        public HarnessRenderer(Actor actor) : base(actor)
        {
            this.bubbleSpawner = RequireComponent<BubbleSpawner>();
            this.levelTransition = RequireComponent<LevelTransition>();
            wires = new Wire[7];


            var rand = MachinaGame.Random.DirtyRandom;
            wires[0] = new Wire(new Vector2(rand.Next(-40, 40), -200 - rand.Next(0, 50)), new Vector2(-47, -100));
            wires[1] = new Wire(new Vector2(rand.Next(-40, 40), -200 - rand.Next(0, 50)), new Vector2(-60, -110));
            wires[2] = new Wire(new Vector2(rand.Next(-40, 40), -200 - rand.Next(0, 50)), new Vector2(-25, -115));
            wires[3] = new Wire(new Vector2(rand.Next(-40, 40), -200 - rand.Next(0, 50)), new Vector2(0, -120));
            wires[4] = new Wire(new Vector2(rand.Next(-40, 40), -200 - rand.Next(0, 50)), new Vector2(70, -130));
            wires[5] = new Wire(new Vector2(rand.Next(-40, 40), -200 - rand.Next(0, 50)), new Vector2(45, -90));
            wires[6] = new Wire(new Vector2(rand.Next(-40, 40), -200 - rand.Next(0, 50)), new Vector2(55, -120));
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
                var harnessRect = HarnessRect;
                var vel = new Vector2(rand.Next(-5, 5), rand.Next(10, 15));
                this.bubbleSpawner.SpawnBubble(harnessRect.Center.ToVector2(), vel, 0);
            }

            if (this.levelTransition.CurrentLevel.DamagedHarness)
            {
                this.harnessOffset = new Point((int) (MathF.Sin(this.totalTimer * 40) * 5), 0);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var lineThickness = 3f;


            foreach (var wire in this.wires)
            {
                if (wire != null)
                {
                    wire.BuildCurveWithCustomStart(wire.start + this.harnessOffset.ToVector2());
                    wire.Draw(spriteBatch, transform.Position, transform.Depth, lineThickness);

                    spriteBatch.DrawCircle(new CircleF(wire.points[0] + transform.Position, 5), 5, Color.White, lineThickness, transform.Depth);
                    spriteBatch.DrawCircle(new CircleF(wire.points[^1] + transform.Position, 5), 5, Color.White, lineThickness, transform.Depth);

                    if (wire == this.currentWire)
                    {
                        // Electrical impulse
                        spriteBatch.DrawCircle(new CircleF(wire.GetPointAlong(this.wireTimer) + transform.Position, 7), 7, Color.Yellow, 7, transform.Depth - 1);
                    }
                }
            }

            spriteBatch.DrawRectangle(HarnessRect, Color.White, lineThickness, transform.Depth);
        }

        private Rectangle HarnessRect
        {
            get
            {

                return new Rectangle(new Point(-50, -300) + transform.Position.ToPoint() + harnessOffset, new Point(100, 100));
            }
        }

        private class Wire
        {
            public readonly Vector2 start;
            public readonly Vector2 end;
            private Vector2 pull1;
            private Vector2 pull2;
            private CatmullRomCurve cachedCurve;

            public Vector2[] points => this.cachedCurve.points;
            public Vector2 GetPointAlong(float f) => this.cachedCurve.GetPointAlong(f);
            public void Draw(SpriteBatch spriteBatch, Vector2 translate, Depth depth, float lineThickness) => this.cachedCurve.Draw(spriteBatch, translate, depth, lineThickness);

            public Wire(Vector2 start, Vector2 end)
            {
                this.start = start;
                this.end = end;

                var displacement = (this.end - this.start);
                var mid = this.start + (displacement / 2);
                var rand = MachinaGame.Random.DirtyRandom;
                float angle1 = (float) rand.NextDouble() * MathF.PI * 2;
                float angle2 = (float) rand.NextDouble() * MathF.PI * 2;
                this.pull1 = mid + new Vector2(MathF.Sin(angle1), MathF.Cos(angle1)) * displacement.Length();
                this.pull2 = mid + new Vector2(MathF.Sin(angle2), MathF.Cos(angle2)) * displacement.Length();

                BuildCurveWithDefault();
            }

            public void BuildCurveWithDefault()
            {
                this.cachedCurve = CatmullRomCurve.Create(25, this.pull1, this.start, this.end, this.pull2);
            }

            public void BuildCurveWithCustomEnd(Vector2 customEnd)
            {
                this.cachedCurve = CatmullRomCurve.Create(25, this.pull1, this.start, customEnd, this.pull2);
            }

            public void BuildCurveWithCustomStart(Vector2 customStart)
            {
                this.cachedCurve = CatmullRomCurve.Create(25, this.pull1, customStart, this.end, this.pull2);
            }
        }

    }
}
