using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace LD48.Components
{
    public class Player : BaseComponent
    {
        public Input input;
        private Actor lure;
        private int level = 0;
        public bool IsAiming
        {
            get; private set;
        }

        public Vector2 MousePos
        {
            get; private set;
        }
        public readonly List<Transform> CandidateTargets = new List<Transform>();

        public bool Asleep
        {
            get; set;
        } = true;

        public Vector2 Velocity
        {
            get; private set;
        }

        private readonly TweenChain levelTransitionTween;

        public Vector2 CameraPos
        {
            get; private set;
        }
        public bool IsLureDeployed => this.lure != null;

        public Vector2 LureEnd => this.lure.transform.Position;

        public bool IsPlayingButIdle => !IsLureDeployed && !IsAiming && !Asleep;

        public bool IsInLevelTransition => !this.levelTransitionTween.IsFinished || Asleep;
        public Action onWakeUp;
        public Action onFallAsleep;

        public Player(Actor actor) : base(actor)
        {
            Velocity = Vector2.Zero;
            this.levelTransitionTween = new TweenChain();

            // Intro
            this.levelTransitionTween.AppendWaitTween(1);
            this.levelTransitionTween.AppendCallback(() => { this.input.downward = true; });
            this.levelTransitionTween.AppendWaitTween(2);
            this.levelTransitionTween.AppendCallback(() => { this.input.downward = false; });
            this.levelTransitionTween.AppendCallback(() =>
            {
                WakeUp();
                StartNextLevel();
            });
        }

        private void WakeUp()
        {
            //Asleep = false;
            this.onWakeUp?.Invoke();
        }

        private void FallAsleep()
        {
            //Asleep = true;
            this.onFallAsleep?.Invoke();
        }

        public override void Update(float dt)
        {
            this.levelTransitionTween.Update(dt);

            ProcessInput(dt);
            transform.Position += Velocity * dt * 60;

            if (IsAiming)
            {
                this.actor.scene.TimeScale = 0.25f;
            }
            else
            {
                this.actor.scene.TimeScale = 1f;
            }

            var cameraDisplacement = transform.Position - this.actor.scene.camera.Position - this.actor.scene.camera.ViewportCenter;
            if (cameraDisplacement.Y > 0)
            {
                this.actor.scene.camera.Position += cameraDisplacement * 0.2f;
            }

            if (this.CandidateTargets.Count == 0 && !IsInLevelTransition && IsPlayingButIdle)
            {
                AdvanceToNextLevel(4);
            }
        }

        private void ProcessInput(float dt)
        {
            var y = Velocity.Y;
            if (this.input.upward)
            {
                y -= dt * 5;
            }

            if (this.input.downward)
            {
                y += dt * 5;
            }

            if (this.input.None)
            {
                if (y > 0)
                {
                    y -= dt * 5;
                }

                if (y < 0)
                {
                    y += dt * 5;
                }

                if (Math.Abs(y) < dt * 5)
                {
                    y = 0;
                }
            }

            Velocity = new Vector2(0, y);
        }

        public override void OnMouseUpdate(Vector2 currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            MousePos = currentPosition;
        }

        public void AdvanceToNextLevel(int duration)
        {
            this.levelTransitionTween.Clear();
            this.levelTransitionTween.AppendCallback(() => { FallAsleep(); });
            this.levelTransitionTween.AppendWaitTween(3);
            this.levelTransitionTween.AppendCallback(() => { this.input.downward = true; });
            this.levelTransitionTween.AppendWaitTween(duration);
            this.levelTransitionTween.AppendCallback(() => { this.input.downward = false; });
            this.levelTransitionTween.AppendWaitTween(duration / 2);
            this.levelTransitionTween.AppendCallback(() =>
            {
                StartNextLevel();
                WakeUp();
            });
        }

        public void StartNextLevel()
        {
            MachinaGame.Print("Starting level", this.level, Fish.FishStats.Levels.Length);

            if (Fish.FishStats.Levels.Length > this.level)
            {
                var stats = Fish.FishStats.Levels[this.level];

                for (int i = 0; i < stats.Count; i++)
                {
                    var camWidth = this.actor.scene.camera.ViewportWidth;
                    float randomX = MachinaGame.Random.CleanRandom.Next(camWidth / 2, camWidth) * ((MachinaGame.Random.CleanRandom.NextDouble() < 0.5) ? -1f : 1f);
                    var fishPos = new Vector2(randomX, this.actor.scene.camera.ViewportHeight / 2);

                    Game1.SpawnNewFish(
                        this.actor.scene, transform.Position + fishPos, this, stats);
                }
                this.level++;
            }
            else
            {
                MachinaGame.Print("Finished!");
            }

        }

        public void ResetLure()
        {
            this.lure = null;
        }

        public override void OnMouseButton(MouseButton button, Vector2 currentPosition, ButtonState state)
        {
            if (button == MouseButton.Left && !IsInLevelTransition && !IsLureDeployed)
            {
                if (state == ButtonState.Released && IsAiming)
                {
                    SpawnLure(currentPosition);
                    IsAiming = false;
                }

                if (state == ButtonState.Pressed)
                {
                    IsAiming = true;
                }
            }
        }

        private void SpawnLure(Vector2 targetPosition)
        {
            if (this.lure == null)
            {
                this.lure = this.actor.transform.AddActorAsChild("Lure");
                this.lure.transform.Position = targetPosition;
                new BubbleSpawner(this.lure, new Machina.Data.MinMax<int>(14, 24));
                new LureRenderer(this.lure, this, this.actor.GetComponent<EyeRenderer>());
            }
        }

        public struct Input
        {
            public bool upward;
            public bool downward;

            public bool None => !upward && !downward;
        }
    }
}
