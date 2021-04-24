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

        public readonly List<Transform> CandidateTargets = new List<Transform>();


        public Vector2 Velocity
        {
            get; private set;
        }

        private readonly TweenChain playerTween;

        public Vector2 CameraPos
        {
            get; private set;
        }
        public bool IsLureDeployed => this.lure != null;

        public Transform LureEnd => this.lure.transform;

        public Player(Actor actor) : base(actor)
        {
            Velocity = Vector2.Zero;
            this.playerTween = new TweenChain();

            this.playerTween.AppendWaitTween(1);
            this.playerTween.AppendCallback(() => { this.input.downward = true; });
            this.playerTween.AppendWaitTween(2);
            this.playerTween.AppendCallback(() => { this.input.downward = false; });
            this.playerTween.AppendCallback(() =>
            {
                StartNextLevel();
            });
        }

        public override void Update(float dt)
        {
            this.playerTween.Update(dt);

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
                y *= (1 - dt);
            }

            Velocity = new Vector2(0, y);
            transform.Position += Velocity * dt * 60;

            var cameraDisplacement = transform.Position - this.actor.scene.camera.Position - this.actor.scene.camera.ViewportCenter;
            if (cameraDisplacement.Y > 0)
            {
                this.actor.scene.camera.Position += cameraDisplacement * 0.2f;
            }

            if (this.CandidateTargets.Count == 0 && this.playerTween.IsFinished)
            {
                GoDeeper(4);
            }
        }

        public void GoDeeper(int duration)
        {
            this.playerTween.Clear();
            this.playerTween.AppendWaitTween(3);
            this.playerTween.AppendCallback(() => { this.input.downward = true; });
            this.playerTween.AppendWaitTween(duration);
            this.playerTween.AppendCallback(() => { this.input.downward = false; });
            this.playerTween.AppendWaitTween(duration / 2);
            this.playerTween.AppendCallback(() =>
            {
                StartNextLevel();
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
            if (button == MouseButton.Left)
            {
                if (state == ButtonState.Released)
                {
                    SpawnLure(currentPosition);
                    this.actor.scene.TimeScale = 1;
                }

                if (state == ButtonState.Pressed)
                {
                    this.actor.scene.TimeScale = 0.25f;
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
