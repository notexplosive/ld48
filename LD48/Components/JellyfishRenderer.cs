using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace LD48.Components
{
    class JellyfishRenderer : BaseComponent
    {
        private Jellyfish jellyfish;
        private Fish fish;
        private BubbleSpawner bubbleSpawner;
        private HairStrand[] strands;
        private int follicleCount = 5;
        private float hairTimer;

        public JellyfishRenderer(Actor actor) : base(actor)
        {
            this.jellyfish = RequireComponent<Jellyfish>();
            this.fish = RequireComponent<Fish>();
            this.bubbleSpawner = RequireComponent<BubbleSpawner>();

            this.strands = new HairStrand[this.follicleCount];
            for (int i = 0; i < this.follicleCount; i++)
            {
                this.strands[i] = new HairStrand(transform);
            }
        }

        public override void Update(float dt)
        {
            var angularVel = ((this.fish.Velocity.ToAngle() + MathF.PI / 2) - transform.Angle) / 40;
            transform.Angle += angularVel;

            if (Math.Abs(angularVel) > 0.06f)
            {
                for (int i = 0; i < follicleCount; i++)
                {
                    this.bubbleSpawner.SpawnBubble(FolliclePos(i), this.fish.Velocity, 0f);
                }
            }

            if (this.hairTimer < 0)
            {
                for (int i = 0; i < follicleCount; i++)
                {
                    this.strands[i].Cycle(FolliclePos(i));
                }
                this.hairTimer = 0.05f;
            }
            this.hairTimer -= dt;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var color = Color.White;
            var lineThickness = 3;

            if (this.jellyfish.HitTimer > 0)
            {
                color = Color.HotPink;
            }
            spriteBatch.DrawCircle(new CircleF(transform.Position, this.fish.Size), 20, color, lineThickness, transform.Depth);

            for (int i = 0; i < follicleCount; i++)
            {
                this.strands[i].Draw(spriteBatch, FolliclePos(i), lineThickness, transform.Depth, color);
            }
        }

        private Vector2 FolliclePos(int i)
        {
            var aof = MathF.PI / this.follicleCount * i - MathF.PI / 2;
            return transform.Position + new Vector2(MathF.Cos(transform.Angle + aof), MathF.Sin(transform.Angle + aof)) * (this.fish.Size - MachinaGame.Random.DirtyRandom.Next(0, (int) this.fish.Size / 4));
        }

        private class HairStrand
        {
            private readonly LinkedList<Vector2> list = new LinkedList<Vector2>();
            private readonly int listLength = 20;

            public HairStrand(Transform transform)
            {
                for (int i = 0; i < this.listLength; i++)
                {
                    this.list.AddFirst(transform.Position);
                }
            }

            public void Cycle(Vector2 folliclePos)
            {
                this.list.AddFirst(folliclePos);
                this.list.RemoveLast();
            }

            public void Draw(SpriteBatch spriteBatch, Vector2 folliclePos, float lineThickness, Depth depth, Color color)
            {
                var prevNode = folliclePos;
                var i = 0;
                var rand = MachinaGame.Random.DirtyRandom;
                foreach (var node in this.list)
                {
                    var opacity = 1 - (float) i / this.listLength;
                    var drawNode = node + new Vector2(rand.Next(-5, 5), rand.Next(-5, 5));
                    spriteBatch.DrawLine(drawNode, prevNode, color * opacity, lineThickness, depth);
                    prevNode = drawNode;
                    i++;
                }
            }
        }
    }
}
