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
        public readonly int length;
        private readonly float swayFactor;
        private float baseAngle;
        public Node[] nodes;
        private float time;

        public Vector2 EndPoint => NormalizedEnd * this.length;
        public Vector2 StartPoint => transform.Position;
        public Vector2 NormalizedEnd => new Vector2(MathF.Cos(transform.Angle), MathF.Sin(transform.Angle));
        private float Sway => MathF.Sin(this.time) * swayFactor;

        public Seaweed(Actor actor) : base(actor)
        {
            this.length = 400;
            this.swayFactor = 1 / 5f;
            var numberOfNodes = 10;
            this.baseAngle = transform.Angle;

            this.nodes = new Node[numberOfNodes];
            for (int i = 0; i < numberOfNodes; i++)
            {
                var sign = i % 2 == 0 ? -1 : 1;
                this.nodes[i] = new Node(this, (this.length / numberOfNodes * i), 40, MathF.PI / 3 * sign);
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
        }

        public Node NodeAt(Vector2 position, float cursorRadius)
        {
            foreach (var node in nodes)
            {
                if ((position - node.CenterPos).LengthSquared() < node.radius * node.radius + cursorRadius * cursorRadius)
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
    }
}
