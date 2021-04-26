using Machina.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace LD48.Data
{
    public class TextCrawl
    {
        private readonly string fullText;
        private readonly int finalIndex;
        private float totalTime;
        private int CurrentIndex => Math.Clamp((int) this.totalTime, 0, this.finalIndex);
        public string CurrentText => this.fullText.Substring(0, CurrentIndex);

        public TextCrawl(string text)
        {
            this.fullText = text;
            this.totalTime = 0f;
            this.finalIndex = this.fullText.Length;
        }

        public bool IsFinished => CurrentIndex == this.finalIndex;

        public void Update(float dt)
        {
            int oldIndex = (int) this.totalTime;
            this.totalTime += dt * 20;
            int newIndex = (int) this.totalTime;

            if (oldIndex != newIndex && !IsFinished && CurrentText.Substring(oldIndex, 1) != " ")
            {
                var s = MachinaGame.Assets.GetSoundEffectInstance("key-click-single");
                s.Volume = 0.5f;
                s.Pitch = (float) MachinaGame.Random.DirtyRandom.NextDouble();
                s.Play();
            }
        }

        public void SkipToEnd()
        {
            this.totalTime = this.finalIndex;
        }
    }
}
