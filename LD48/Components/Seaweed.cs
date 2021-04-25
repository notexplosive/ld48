using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace LD48.Components
{
    class Seaweed : BaseComponent
    {
        public readonly float length;
        private readonly float swayFactor;
        private float baseAngle;
        public Node[] nodes;
        private float time;
        private LevelTransition levelTransition;
        private bool dead;

        public float HitTimer
        {
            get;
            private set;
        }

        public Vector2 EndPoint => NormalizedEnd * this.length;
        public Vector2 StartPoint => transform.Position;
        public Vector2 NormalizedEnd => new Vector2(MathF.Cos(transform.Angle), MathF.Sin(transform.Angle));
        private float Sway => MathF.Sin(this.time) * swayFactor;

        public Seaweed(Actor actor, float length, int numberOfNodes, float swayFactorCoef, LevelTransition levelTransition) : base(actor)
        {
            this.length = length;
            this.swayFactor = 1f / swayFactorCoef;
            this.baseAngle = transform.Angle;

            this.nodes = new Node[numberOfNodes];
            for (int i = 0; i < numberOfNodes; i++)
            {
                var sign = i % 2 == 0 ? -1 : 1;
                this.nodes[i] = new Node(this, (this.length / (numberOfNodes - 1) * i), 40, MathF.PI / 3 * sign);
            }

            this.time = (float) MachinaGame.Random.CleanRandom.NextDouble() * MathF.PI * 2;

            this.levelTransition = levelTransition;
            this.levelTransition.onFallAsleep += AddDestroyTimer;
        }

        public override void OnDelete()
        {
            this.levelTransition.onFallAsleep -= AddDestroyTimer;
        }

        private void AddDestroyTimer()
        {
            if (!this.dead)
            {
                this.dead = true;
                new DestroyTimer(this.actor, 10);
            }
        }

        public override void Update(float dt)
        {
            foreach (var node in nodes)
            {
                node.Update(dt);
            }

            this.time += dt;

            transform.Angle = this.baseAngle + Sway;

            HitTimer -= dt;
        }

        public Node NodeAt(Vector2 position, float cursorRadius)
        {
            foreach (var node in nodes)
            {
                var f = node.radius + cursorRadius;
                if ((position - node.CenterPos).LengthSquared() < f * f)
                {
                    return node;
                }
            }

            return null;
        }

        public class Node
        {
            public readonly float radius;
            private readonly Seaweed branch;
            private readonly float placementAlongTrunk;
            private readonly float localAngle;
            private float time;
            private float Sway => MathF.Sin(this.time) / 4;
            public Vector2 StartPosition => this.branch.StartPoint + this.branch.NormalizedEnd * this.placementAlongTrunk;
            public Vector2 CenterPos => StartPosition + new Vector2(MathF.Cos(Angle + Sway), MathF.Sin(Angle + Sway)) * radius;
            public float Angle => this.branch.transform.Angle - this.localAngle;

            public Node(Seaweed branch, float placementAlongTrunk, float offsetFromBranch, float localAngle)
            {
                this.branch = branch;
                this.placementAlongTrunk = placementAlongTrunk;
                this.radius = offsetFromBranch;
                this.localAngle = localAngle;
            }

            public void Update(float dt)
            {
                this.time += dt * MachinaGame.Random.DirtyRandom.Next(1, 5);
            }
        }

        public void Hit()
        {
            this.HitTimer = 0.5f;
        }
    }
}
