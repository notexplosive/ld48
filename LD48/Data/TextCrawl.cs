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
            this.totalTime += dt * 20;
        }

        public void SkipToEnd()
        {
            this.totalTime = this.finalIndex;
        }
    }
}
