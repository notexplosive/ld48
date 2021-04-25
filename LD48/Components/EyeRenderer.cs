using LD48.Data;
using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Machina.ThirdParty;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace LD48.Components
{
    public class EyeRenderer : BaseComponent
    {
        private readonly TweenAccessors<float> openAmountAccessors;
        private readonly TweenChain eyeTween;
        private readonly Player player;
        private readonly CatmullRomCurve[] wires = Array.Empty<CatmullRomCurve>();
        private CatmullRomCurve[] curves = Array.Empty<CatmullRomCurve>();
        private float openPercent;
        private Vector2 lookOffset;
        private float wireTimer = 0f;
        private CatmullRomCurve currentWire;
        private Vector2 lookTarget;
        private Vector2 pointIlookAt;

        public Vector2 IrisCenter => (this.curves[2].Average + this.curves[3].Average) / 2 + transform.Position;

        private void LookAt(Vector2 target)
        {
            this.lookTarget = target;
        }

        public EyeRenderer(Actor actor) : base(actor)
        {
            this.player = RequireComponent<Player>();
            this.player.onWakeUp += WakeUp;
            this.player.onFallAsleep += FallAsleep;

            this.openAmountAccessors = new TweenAccessors<float>(() => this.openPercent, val => this.openPercent = val);
            this.eyeTween = new TweenChain();

            BuildEye(20, Vector2.Zero);

            wires = new CatmullRomCurve[6];
            wires[0] = CatmullRomCurve.Create(25, new Vector2(MachinaGame.Random.DirtyRandom.Next(-100, 100) + 100, -140), new Vector2(-40, -200), new Vector2(-69, -100), new Vector2(100, -96));
            wires[1] = CatmullRomCurve.Create(25, new Vector2(MachinaGame.Random.DirtyRandom.Next(-100, 100) + 100, -140), new Vector2(40, -200), new Vector2(60, -110), new Vector2(-100, -96));
            wires[2] = CatmullRomCurve.Create(25, new Vector2(MachinaGame.Random.DirtyRandom.Next(-100, 100) + 100, -140), new Vector2(40, -200), new Vector2(30, -115), new Vector2(-100, -96));
            wires[3] = CatmullRomCurve.Create(25, new Vector2(MachinaGame.Random.DirtyRandom.Next(-100, 100) + 100, -140), new Vector2(-40, -200), new Vector2(-30, -120), new Vector2(-100, -96));
        }

        public override void Update(float dt)
        {
            this.eyeTween.Update(dt);
            BuildEye(400 * this.openPercent, this.lookOffset);

            if (this.lookTarget != null)
            {
                var lookTargetPosition = this.lookTarget - transform.Position;
                var disp = (lookTargetPosition - this.pointIlookAt);
                this.pointIlookAt += disp * dt * 15;
                pointIlookAt.X = Math.Clamp(pointIlookAt.X, -this.actor.scene.camera.ViewportWidth / 4, this.actor.scene.camera.ViewportWidth / 4) + MachinaGame.Random.DirtyRandom.Next(-5, 5);
                pointIlookAt.Y = Math.Clamp(pointIlookAt.Y, -this.actor.scene.camera.ViewportHeight / 2, this.actor.scene.camera.ViewportHeight / 2) + MachinaGame.Random.DirtyRandom.Next(-5, 5);
                this.lookOffset = new Vector2(pointIlookAt.X / this.actor.scene.camera.ViewportWidth / 2, pointIlookAt.Y / this.actor.scene.camera.ViewportHeight / 4);
            }

            this.wireTimer += dt * 5;

            if (this.wireTimer > 1)
            {
                this.wireTimer = 0;
                this.currentWire = this.wires[MachinaGame.Random.DirtyRandom.Next(this.wires.Length)];

                if (MachinaGame.Random.DirtyRandom.NextDouble() < 0.05)
                {
                    Blink();
                }
            }

            if (this.player.IsPlayingButIdle)
            {
                var minLength = float.MaxValue;
                foreach (var targetTransform in this.player.CandidateTargets)
                {
                    var disp = transform.Position - targetTransform.Position;
                    var length = disp.LengthSquared();
                    if (length < minLength)
                    {
                        minLength = length;
                        LookAt(targetTransform.Position);
                    }
                }
            }
            else
            {
                if (this.player.IsLureDeployed)
                {
                    LookAt(this.player.LureEnd);
                }

                if (this.player.IsAiming)
                {
                    LookAt(this.player.MousePos);
                }
            }
        }

        public void Blink()
        {
            if (this.player.IsPlayingButIdle)
            {
                TweenOpenAmountTo(0f, 0.25f);
                TweenOpenAmountTo(1f, 0.25f);
            }
        }

        public void FallAsleep()
        {
            this.player.Asleep = true;
            ClearTween();
            TweenOpenAmountTo(0f, 2f);
        }

        public void WakeUp()
        {
            ClearTween();
            TweenDelay(3);
            TweenOpenAmountTo(1, 2f);
            this.eyeTween.AppendCallback(() =>
            {
                this.player.Asleep = false;
            });
        }

        public void ClearTween()
        {
            this.eyeTween.Clear();
        }

        public void TweenDelay(float duration)
        {
            this.eyeTween.AppendWaitTween(duration);
        }

        public void TweenOpenAmountTo(float percent, float duration)
        {
            this.eyeTween.AppendFloatTween(percent, duration, EaseFuncs.QuadraticEaseOut, this.openAmountAccessors);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var lineThickness = 3f;
            foreach (var curve in this.curves)
            {
                curve?.Draw(spriteBatch, transform.Position, transform.Depth, lineThickness);
            }

            foreach (var curve in this.wires)
            {
                curve?.Draw(spriteBatch, transform.Position, transform.Depth, lineThickness);

                if (curve != null)
                {
                    spriteBatch.DrawCircle(new CircleF(curve.points[0] + transform.Position, 5), 5, Color.White, lineThickness, transform.Depth);
                    spriteBatch.DrawCircle(new CircleF(curve.points[curve.points.Length - 1] + transform.Position, 5), 5, Color.White, lineThickness, transform.Depth);

                    if (curve == this.currentWire)
                    {
                        spriteBatch.DrawCircle(new CircleF(curve.GetPointAt(this.wireTimer) + transform.Position, 5), 3, Color.White, lineThickness, transform.Depth);
                    }
                }
            }

            spriteBatch.DrawRectangle(new Rectangle(transform.Position.ToPoint() + new Point(-50, -300), new Point(100, 100)), Color.White, lineThickness, transform.Depth);

            spriteBatch.DrawCircle(new CircleF(transform.Position, 150), 32, Color.White, lineThickness, transform.Depth);

            // coil attached to eye
            for (int i = 0; i < 5; i++)
            {
                spriteBatch.DrawCircle(new CircleF(transform.Position - new Vector2(0, 150 + i * 12), 12), 12, Color.White, lineThickness, transform.Depth);
            }

            // chain
            for (int i = 0; i < 15; i++)
            {
                spriteBatch.DrawCircle(new CircleF(transform.Position + new Vector2(0, -300 - i * 24 * Math.Max(1, -this.player.Velocity.Y / 20 + 1)), 12), 12, Color.White, lineThickness, transform.Depth);
            }

            spriteBatch.DrawCircle(new CircleF(IrisCenter, 15 * this.openPercent), 15, Color.White, lineThickness, transform.Depth);
        }

        private void BuildEye(float openAmount, Vector2 lookOffset)
        {
            var count = 32;
            var left = new Vector2(-100, 0);
            var right = -left;
            var basePull = new Vector2(0, openAmount);
            var curves = new CatmullRomCurve[6];
            // top curve
            var topPull = basePull - lookOffset * 5 * openAmount;
            curves[0] = CatmullRomCurve.Create(count,
                topPull,
                left,
                right,
                topPull);

            // bottom curve
            var bottomPull = -basePull - lookOffset * 2 * openAmount;
            curves[1] = CatmullRomCurve.Create(count,
                bottomPull,
                left,
                right,
                bottomPull);

            CatmullRomCurve BuildIrisSegment(float progress, Vector2 pull)
            {
                var start = curves[0].GetPointAt(progress);
                var end = curves[1].GetPointAt(progress);

                return CatmullRomCurve.Create(count,
                    pull,
                    start,
                    end,
                    pull);
            }

            var progressForIris = 0.4f;
            curves[2] = BuildIrisSegment(Math.Clamp(progressForIris + lookOffset.X, 0.2f, 0.8f), right);
            curves[3] = BuildIrisSegment(Math.Clamp(1 - progressForIris + lookOffset.X, 0.2f, 0.8f), left);

            this.curves = curves;
        }
    }
}
