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
    public enum Phase
    {
        Attempting,
        Caught,
        Feeding,
        Retracting
    }
    public class LureRenderer : BaseComponent
    {
        private readonly EyeRenderer eye;
        private readonly Player player;
        private readonly TweenChain tween;
        private Vector2 rootPosition;
        private BubbleSpawner bubbleSpawner;
        private float progress;
        private Phase currentPhase = Phase.Attempting;

        public LureRenderer(Actor actor, Player player, EyeRenderer eye) : base(actor)
        {
            this.eye = eye;
            this.player = player;
            this.rootPosition = transform.Parent.Position - new Vector2(0, 250);
            this.bubbleSpawner = RequireComponent<BubbleSpawner>();
            var targetLocalPos = transform.LocalPosition;
            this.tween = new TweenChain();
            var accessors = new TweenAccessors<float>(() => this.progress, val => this.progress = val);
            tween.AppendFloatTween(1f, 0.5f, EaseFuncs.QuadraticEaseOut, accessors);
            tween.AppendCallback(() =>
            {
                var caughtFish = false;
                foreach (var targetActor in actor.scene.GetAllActors())
                {
                    var fish = targetActor.GetComponent<Fish>();
                    if (fish != null)
                    {
                        if ((targetActor.transform.Position - transform.Position).Length() < 10 + fish.HitRadius)
                        {
                            caughtFish = true;
                            fish.actor.Destroy();
                        }
                    }
                }

                if (caughtFish)
                {
                    bubbleSpawner.SpawnBubble(EndPos, Vector2.Zero, 0.1f);
                    bubbleSpawner.SpawnBubble(EndPos, Vector2.Zero, 0.2f);
                    bubbleSpawner.SpawnBubble(EndPos, Vector2.Zero, 0.3f);
                    bubbleSpawner.SpawnBubble(EndPos, Vector2.Zero, 0.4f);

                    this.currentPhase = Phase.Caught;
                    tween.AppendFloatTween(0f, 1f, EaseFuncs.QuadraticEaseOut, accessors);
                    tween.AppendCallback(() =>
                    {
                        this.currentPhase = Phase.Feeding;
                        transform.LocalPosition = new Vector2(0, 0);
                    });
                    tween.AppendFloatTween(1f, 0.25f, EaseFuncs.QuadraticEaseOut, accessors);
                    tween.AppendWaitTween(0.25f);
                    tween.AppendCallback(() =>
                    {
                        this.currentPhase = Phase.Retracting;
                        this.eye.ClearTween();
                        this.eye.TweenOpenAmountTo(0f, 0.25f);
                        this.eye.TweenOpenAmountTo(1f, 0.5f);
                    });
                    tween.AppendWaitTween(0.5f);
                    tween.AppendFloatTween(0f, 1f, EaseFuncs.QuadraticEaseOut, accessors);
                }
                else
                {
                    tween.AppendCallback(() =>
                    {
                        this.currentPhase = Phase.Retracting;
                        bubbleSpawner.SpawnBubble(EndPos, Vector2.Zero, 0f);
                    });
                    tween.AppendFloatTween(0f, 0.5f, EaseFuncs.QuadraticEaseOut, accessors);
                }
            });


        }

        public override void Update(float dt)
        {
            this.tween.Update(dt);
            if (this.currentPhase == Phase.Caught && MachinaGame.Random.DirtyRandom.NextDouble() < 0.1f)
            {
                this.bubbleSpawner.SpawnBubble(EndPos, Vector2.Zero, 0.1f);
            }

            if (this.tween.IsFinished)
            {
                this.actor.Destroy();
                this.player.ResetLure();
            }
        }

        public Vector2 EndPos => this.rootPosition + (transform.Position - this.rootPosition) * this.progress;


        public override void Draw(SpriteBatch spriteBatch)
        {
            var lineThickness = 3f;
            spriteBatch.DrawLine(EndPos, this.rootPosition, Color.White, lineThickness, transform.Depth);

            int circleRadius = 10;

            if (this.currentPhase == Phase.Caught || currentPhase == Phase.Feeding)
            {
                circleRadius = 50;
            }

            spriteBatch.DrawCircle(new CircleF(EndPos, circleRadius), 6, Color.White, lineThickness, transform.Depth);
        }
    }
}
