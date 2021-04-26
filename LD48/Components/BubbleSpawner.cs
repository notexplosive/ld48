using Machina.Engine;
using Machina.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;
using Machina.Data;

namespace OculusLeviathan.Components
{
    class BubbleSpawner : BaseComponent
    {
        public MinMax<int> bubbleSize;

        public BubbleSpawner(Actor actor, MinMax<int> bubbleSize) : base(actor)
        {
            this.bubbleSize = bubbleSize;
        }

        public void SpawnBubble(Vector2 pos, Vector2 startingVel, float delay)
        {
            var bubble = this.actor.scene.AddActor("bubble");
            new Bubble(bubble, MachinaGame.Random.DirtyRandom.Next(this.bubbleSize.min, this.bubbleSize.max), startingVel, delay);
            MachinaGame.Random.DirtyRandom.NextUnitVector(out Vector2 offset);
            bubble.transform.Position = pos + offset * MachinaGame.Random.DirtyRandom.Next(5);
        }
    }
}
