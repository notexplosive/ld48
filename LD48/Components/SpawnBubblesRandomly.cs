using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace LD48.Components
{
    class SpawnBubblesRandomly : BaseComponent
    {
        private readonly BubbleSpawner bubbleSpawner;
        private readonly Fish fish;
        private float bubbleSpawnTimer;

        public SpawnBubblesRandomly(Actor actor) : base(actor)
        {
            this.bubbleSpawner = RequireComponent<BubbleSpawner>();
            this.fish = RequireComponent<Fish>();
        }

        public override void Update(float dt)
        {
            if (this.bubbleSpawnTimer < 0 && MachinaGame.Random.DirtyRandom.NextDouble() < 0.15)
            {
                this.bubbleSpawnTimer = (int) (MachinaGame.Random.DirtyRandom.NextDouble() * 5);
                for (int i = 0; i < 5; i++)
                {
                    this.bubbleSpawner.SpawnBubble(transform.Position, this.fish.Velocity, i / 20f);
                }
            }
            this.bubbleSpawnTimer -= dt;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
