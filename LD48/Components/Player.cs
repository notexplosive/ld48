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
        private Actor lure;

        public bool IsAiming
        {
            get; private set;
        }
        public bool IsAllowedToDeploy => !IsLureDeployed && !this.levelTransition.IsInLevelTransition;

        public Vector2 MousePos
        {
            get; private set;
        }
        public readonly List<Transform> CandidateTargets = new List<Transform>();
        public Vector2 CameraPos
        {
            get; private set;
        }
        public bool IsLureDeployed => this.lure != null;

        public Vector2 LureEnd => this.lure.transform.Position;

        public bool IsPlayingButIdle => !IsAiming && IsAllowedToDeploy;
        private readonly LevelTransition levelTransition;

        public Player(Actor actor) : base(actor)
        {
            this.levelTransition = RequireComponent<LevelTransition>();
        }

        public override void Update(float dt)
        {
            if (IsAiming && !IsLureDeployed && !this.levelTransition.IsInLevelTransition)
            {
                this.actor.scene.TimeScale = 0.25f;
            }
            else
            {
                this.actor.scene.TimeScale = 1f;
            }

            if (this.CandidateTargets.Count == 0 && !this.levelTransition.IsInLevelTransition && IsPlayingButIdle)
            {
                this.levelTransition.FinishLevel(4);
            }

            var cameraDisplacement = transform.Position - this.actor.scene.camera.Position - this.actor.scene.camera.ViewportCenter;
            if (cameraDisplacement.Y > 0)
            {
                this.actor.scene.camera.Position += cameraDisplacement * 0.2f;
            }
        }

        public override void OnMouseUpdate(Vector2 currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            MousePos = currentPosition;
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
                new LureRenderer(this.lure, this, this.actor.GetComponentsInImmediateChildren<EyeRenderer>()[0]);
            }
        }

    }
}
