using Machina.Components;
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

        public readonly List<Transform> CandidateTargets = new List<Transform>();


        public Vector2 Velocity
        {
            get; private set;
        }

        public Vector2 CameraPos
        {
            get; private set;
        }
        public bool IsLureDeployed => this.lure != null;

        public Transform LureEnd => this.lure.transform;

        public Player(Actor actor) : base(actor)
        {
            Velocity = Vector2.Zero;
        }

        public override void Update(float dt)
        {
            var y = Velocity.Y;
            if (this.input.upward)
            {
                y += dt * 5;
            }

            if (this.input.downward)
            {
                y -= dt * 5;
            }

            if (this.input.None)
            {
                y *= (1 - dt);
            }

            Velocity = new Vector2(0, y);
            transform.Position += Velocity;

            var cameraDisplacement = transform.Position - this.actor.scene.camera.Position - this.actor.scene.camera.ViewportCenter;
            this.actor.scene.camera.Position += cameraDisplacement * 0.2f;
        }


        public override void OnKey(Keys key, ButtonState state, ModifierKeys modifiers)
        {
            if (key == Keys.Down && modifiers.None)
            {
                this.input.upward = state == ButtonState.Pressed;
            }

            if (key == Keys.Up && modifiers.None)
            {
                this.input.downward = state == ButtonState.Pressed;
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
