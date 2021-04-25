﻿using Machina.Components;
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
        private readonly TweenChain lureTween;
        private Vector2 rootPosition => this.eye.IrisCenter;
        private BubbleSpawner bubbleSpawner;
        private float progress;
        private Phase currentPhase = Phase.Attempting;
        private float caughtFishSize;
        private bool hitPlant;
        private readonly float lureSize = 10;

        public LureRenderer(Actor actor, Player player, EyeRenderer eye) : base(actor)
        {
            this.eye = eye;
            this.player = player;
            this.bubbleSpawner = RequireComponent<BubbleSpawner>();
            var targetLocalPos = transform.LocalPosition;
            this.lureTween = new TweenChain();
            var accessors = new TweenAccessors<float>(() => this.progress, val => this.progress = val);
            lureTween.AppendFloatTween(1f, 0.25f, EaseFuncs.QuadraticEaseOut, accessors);
            lureTween.AppendCallback(() =>
            {
                var caughtFish = false;

                foreach (var targetActor in actor.scene.GetAllActors())
                {
                    var fish = targetActor.GetComponent<Fish>();
                    if (fish != null)
                    {
                        if ((targetActor.transform.Position - transform.Position).Length() < this.lureSize + fish.HitRadius)
                        {
                            caughtFish = true;
                            this.caughtFishSize = fish.Size;
                            fish.actor.Destroy();
                            break;
                        }
                    }

                    var plant = targetActor.GetComponent<Seaweed>();

                    if (!caughtFish && plant != null)
                    {
                        var node = plant.NodeAt(transform.Position, this.lureSize);
                        if (node != null)
                        {
                            this.hitPlant = true;
                            plant.Hit();
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
                    lureTween.AppendFloatTween(0f, 0.2f, EaseFuncs.QuadraticEaseOut, accessors);
                    lureTween.AppendCallback(() =>
                    {
                        this.currentPhase = Phase.Feeding;
                        transform.LocalPosition = new Vector2(0, 0);
                    });
                    lureTween.AppendWaitTween(0.25f);
                    lureTween.AppendCallback(() =>
                    {
                        this.currentPhase = Phase.Retracting;
                        this.eye.ClearTween();
                        this.eye.TweenOpenAmountTo(2f, 0.1f);
                        this.eye.TweenOpenAmountTo(0f, 0.15f);
                        this.eye.TweenOpenAmountTo(0f, 0.2f); // Stay closed
                        this.eye.TweenOpenAmountTo(1f, 0.25f);
                    });
                    lureTween.AppendCallback(() =>
                    {
                        var rand = MachinaGame.Random.DirtyRandom;
                        for (int i = 0; i < 10; i++)
                        {
                            bubbleSpawner.SpawnBubble(EndPos, new Vector2(rand.Next(-5, 5), rand.Next(5, 10)), 0f);
                        }
                    });
                    lureTween.AppendWaitTween(0.5f);
                    lureTween.AppendFloatTween(0f, 1f, EaseFuncs.QuadraticEaseOut, accessors);
                }
                else
                {
                    lureTween.AppendCallback(() =>
                    {
                        this.currentPhase = Phase.Retracting;
                        var rand = MachinaGame.Random.DirtyRandom;
                        for (int i = 0; i < 3; i++)
                        {
                            bubbleSpawner.SpawnBubble(EndPos, new Vector2(rand.Next(-5, 5), rand.Next(-5, 5)), 0f);
                        }
                    });

                    var retractDuration = 0.5f;
                    if (this.hitPlant)
                    {
                        retractDuration = 2.5f;
                    }

                    lureTween.AppendFloatTween(0f, retractDuration, EaseFuncs.QuadraticEaseOut, accessors);
                }
            });


        }

        public override void Update(float dt)
        {
            this.lureTween.Update(dt);
            if (this.currentPhase == Phase.Caught && MachinaGame.Random.DirtyRandom.NextDouble() < 0.1f)
            {
                // this.bubbleSpawner.SpawnBubble(EndPos, Vector2.Zero, 0.1f);
            }

            if (this.lureTween.IsFinished)
            {
                this.actor.Destroy();
                this.player.ResetLure();
            }
        }

        public Vector2 EndPos => this.rootPosition + (transform.Position - this.rootPosition) * this.progress;


        public override void Draw(SpriteBatch spriteBatch)
        {
            var lineThickness = 3f;
            var color = Color.Yellow;


            if (this.hitPlant)
            {
                color = Color.HotPink;
            }

            spriteBatch.DrawLine(EndPos, this.rootPosition, color, lineThickness, transform.Depth);

            float circleRadius = 10;

            if (this.currentPhase == Phase.Caught || currentPhase == Phase.Feeding)
            {
                circleRadius = this.caughtFishSize;
            }

            if (this.currentPhase == Phase.Caught || currentPhase == Phase.Feeding)
            {
                spriteBatch.DrawCircle(new CircleF(EndPos, circleRadius), 10, Color.White, lineThickness * 2, transform.Depth);
            }
        }
    }
}
