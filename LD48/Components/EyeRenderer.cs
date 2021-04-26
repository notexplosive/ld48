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
        private readonly LevelTransition levelTransition;
        private CatmullRomCurve[] eyeCurves = Array.Empty<CatmullRomCurve>();
        private float openPercent;
        private Vector2 lookOffset;
        private Vector2? lookTarget;
        private Vector2 pointIlookAt;
        private float aimTimer;

        public Vector2 IrisCenter => (this.eyeCurves[2].Average + this.eyeCurves[3].Average) / 2 + transform.Position + new Vector2(this.lookOffset.X * 50, 0);

        private void LookAt(Vector2 target)
        {
            this.lookTarget = target;
        }

        public EyeRenderer(Actor actor, Player player, LevelTransition levelTransition) : base(actor)
        {
            this.player = player;
            this.levelTransition = levelTransition;
            this.levelTransition.onWakeUp += WakeUpAnimation;
            this.levelTransition.onFallAsleep += FallAsleepAnimation;

            this.openAmountAccessors = new TweenAccessors<float>(() => this.openPercent, val => this.openPercent = val);
            this.eyeTween = new TweenChain();

            BuildEye(20);
        }

        public override void Update(float dt)
        {
            if (this.player.IsAiming && this.player.IsAllowedToDeploy)
            {
                this.aimTimer += dt * 4;
            }
            else
            {
                this.aimTimer -= dt * 10;
            }
            this.aimTimer = Math.Clamp(this.aimTimer, 0, 1);

            this.eyeTween.Update(dt);
            BuildEye(400 * this.openPercent);

            if (this.lookTarget.HasValue)
            {
                var lookTargetPosition = this.lookTarget.Value - transform.Position;
                var disp = (lookTargetPosition - this.pointIlookAt);
                this.pointIlookAt += disp * dt * 15;
                pointIlookAt.X = Math.Clamp(pointIlookAt.X, -this.actor.scene.camera.ViewportWidth / 4, this.actor.scene.camera.ViewportWidth / 4) + MachinaGame.Random.DirtyRandom.Next(-5, 5);
                pointIlookAt.Y = Math.Clamp(pointIlookAt.Y, -this.actor.scene.camera.ViewportHeight / 4, this.actor.scene.camera.ViewportHeight / 2) + MachinaGame.Random.DirtyRandom.Next(-5, 5);
                this.lookOffset = new Vector2(pointIlookAt.X / this.actor.scene.camera.ViewportWidth / 2, pointIlookAt.Y / this.actor.scene.camera.ViewportHeight / 2);
            }

            if (this.player.IsPlayingButIdle)
            {
                if (MachinaGame.Random.DirtyRandom.NextDouble() < 0.001)
                {
                    Blink();
                }

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

                if (this.player.IsAiming && this.player.IsAllowedToDeploy)
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

        public void FallAsleepAnimation()
        {
            this.levelTransition.Asleep = true;
            this.lookTarget = null;
            ClearTween();
            TweenOpenAmountTo(0f, 2f);
        }

        public void WakeUpAnimation()
        {
            this.lookTarget = null;
            ClearTween();
            TweenOpenAmountTo(1, 1.5f);
            this.eyeTween.AppendCallback(() =>
            {
                this.levelTransition.Asleep = false;
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

            // Eye curves
            foreach (var curve in this.eyeCurves)
            {
                curve?.Draw(spriteBatch, transform.Position + new Vector2(this.lookOffset.X * 50, 0), transform.Depth, lineThickness);
            }

            // Main body circle
            spriteBatch.DrawCircle(new CircleF(transform.Position, 150), MachinaGame.Random.DirtyRandom.Next(25, 35), Color.White, lineThickness, transform.Depth);



            spriteBatch.DrawCircle(new CircleF(IrisCenter, 10 * this.openPercent + 10 * EaseFuncs.CubicEaseOut(this.aimTimer) * this.openPercent), 15, Color.White, lineThickness, transform.Depth);
        }

        private void BuildEye(float openAmount)
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
                var start = curves[0].GetPointAlong(progress);
                var end = curves[1].GetPointAlong(progress);

                return CatmullRomCurve.Create(count,
                    pull,
                    start,
                    end,
                    pull);
            }

            var progressForIris = 0.4f * (this.aimTimer / 20 + 1);
            var pullFactor = (1 + this.aimTimer);
            curves[2] = BuildIrisSegment(Math.Clamp(progressForIris + lookOffset.X, 0.2f, 0.8f), right * pullFactor);
            curves[3] = BuildIrisSegment(Math.Clamp(1 - progressForIris + lookOffset.X, 0.2f, 0.8f), left * pullFactor);

            this.eyeCurves = curves;
        }
    }
}
