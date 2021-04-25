using LD48.Data;
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
        private int levelIndex = 0;
        public bool IsAiming
        {
            get; private set;
        }

        public bool IsAllowedToDeploy => !IsLureDeployed && !IsInLevelTransition;

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

        public bool IsPlayingButIdle => !IsAiming && IsAllowedToDeploy;

        public bool IsInLevelTransition => !this.levelTransitionTween.IsFinished || Asleep;
        public Action onWakeUp;
        public Action onFallAsleep;
        private bool readyToStartLevel;

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
                this.readyToStartLevel = true;
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

            if (IsAiming && !IsLureDeployed && !IsInLevelTransition)
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
                FinishLevel(4);
            }

            if (this.readyToStartLevel && Velocity.Y == 0)
            {
                this.readyToStartLevel = false;
                StartNextLevel();
                WakeUp();
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

        public void FinishLevel(int duration)
        {
            this.levelTransitionTween.Clear();
            this.levelTransitionTween.AppendCallback(() => { FallAsleep(); });
            this.levelTransitionTween.AppendWaitTween(3);
            this.levelTransitionTween.AppendCallback(() => { this.input.downward = true; });
            this.levelTransitionTween.AppendWaitTween(duration);
            this.levelTransitionTween.AppendCallback(() => { this.input.downward = false; });
            this.levelTransitionTween.AppendCallback(() =>
            {
                this.readyToStartLevel = true;
            });
        }

        public void StartNextLevel()
        {
            var levels = Level.All;
            MachinaGame.Print("Starting level", this.levelIndex, levels.Length);

            if (levels.Length > this.levelIndex)
            {
                var currentLevel = levels[this.levelIndex];

                for (int i = 0; i < currentLevel.FishCount; i++)
                {
                    var camWidth = this.actor.scene.camera.ViewportWidth;
                    float randomX = MachinaGame.Random.CleanRandom.Next(camWidth / 2, camWidth) * ((MachinaGame.Random.CleanRandom.NextDouble() < 0.5) ? -1f : 1f);
                    var fishPos = new Vector2(randomX, 0);

                    Game1.SpawnNewFish(
                        this.actor.scene, transform.Position + fishPos, this, currentLevel.FishStats);
                }

                foreach (var seaweedInfo in currentLevel.Seaweed)
                {
                    Game1.SpawnSeaweed(this.actor.scene, transform.Position, seaweedInfo);
                }
                this.levelIndex++;
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
            if (button == MouseButton.Left)
            {
                if (state == ButtonState.Released && IsAiming)
                {
                    if (IsAllowedToDeploy)
                    {
                        SpawnLure(currentPosition);
                    }
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
                this.lure.transform.LocalDepth = -1;
                new BubbleSpawner(this.lure, new Machina.Data.MinMax<int>(7, 14));
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
