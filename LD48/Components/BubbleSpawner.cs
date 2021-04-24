using Machina.Engine;
using Machina.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Machina.Data;

namespace LD48.Components
{
    class BubbleSpawner : BaseComponent
    {
        public MinMax<int> bubbleSize;
        private float timer;

        public BubbleSpawner(Actor actor, MinMax<int> bubbleSize) : base(actor)
        {
            this.bubbleSize = bubbleSize;
            this.timer = 0;
        }

        public override void Update(float dt)
        {
            if (MachinaGame.Random.DirtyRandom.NextDouble() < 0.1 && this.timer < 0)
            {
                SpawnBubble();
            }
            this.timer -= dt;
        }

        public void SpawnBubble()
        {
            var bubble = this.actor.scene.AddActor("bubble");
            new Bubble(bubble, MachinaGame.Random.DirtyRandom.Next(this.bubbleSize.min, this.bubbleSize.max));
            new DestroyTimer(bubble, 5);
            bubble.transform.Position = this.actor.transform.Position;
            this.timer = 1f;
        }
    }
}
