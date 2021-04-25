using LD48.Data;
using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace LD48.Components
{
    public class LevelTransition : BaseComponent
    {
        public Input input;
        private readonly TweenChain levelTransitionTween;
        private bool readyToStartLevel;
        public bool IsInLevelTransition => !this.levelTransitionTween.IsFinished || Asleep;
        private int levelIndex = 0;

        public bool Asleep
        {
            get; set;
        } = true;
        public Vector2 Velocity
        {
            get; private set;
        }

        public Action onWakeUp;
        public Action onFallAsleep;

        public LevelTransition(Actor actor) : base(actor)
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

        public override void Update(float dt)
        {
            this.levelTransitionTween.Update(dt);
            ProcessInput(dt);
            transform.Position += Velocity * dt * 60;

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
                        this.actor.scene, transform.Position + fishPos, RequireComponent<Player>(), currentLevel.FishStats);
                }

                foreach (var seaweedInfo in currentLevel.Seaweed)
                {
                    Game1.SpawnSeaweed(this.actor.scene, transform.Position, seaweedInfo, this);
                }

                for (int i = 0; i < currentLevel.JellyfishCount; i++)
                {
                    Game1.SpawnJellyfish(this.actor.scene, this);
                }
                this.levelIndex++;
            }
            else
            {
                MachinaGame.Print("Finished!");
            }

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

        public struct Input
        {
            public bool upward;
            public bool downward;

            public bool None => !upward && !downward;
        }
    }
}
