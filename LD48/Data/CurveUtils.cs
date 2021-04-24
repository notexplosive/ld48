using Machina.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LD48.Data
{
    public class CatmullRomCurve
    {
        public readonly Vector2[] points;
        private readonly Vector2 pullStart;
        private readonly Vector2 start;
        private readonly Vector2 end;
        private readonly Vector2 pullEnd;

        public int Count => points.Length;

        public static CatmullRomCurve Create(int count, Vector2 pullStart, Vector2 start, Vector2 end, Vector2 pullEnd)
        {
            var points = new Vector2[count + 1];
            for (int i = 0; i < count; i++)
            {
                var pos = Vector2.CatmullRom(pullStart, start, end, pullEnd, i / (float) count);
                points[i] = pos;
            }
            points[count] = end;
            return new CatmullRomCurve(points, pullStart, start, end, pullEnd);
        }

        private CatmullRomCurve(Vector2[] points, Vector2 pullStart, Vector2 start, Vector2 end, Vector2 pullEnd)
        {
            this.points = points;
            this.pullStart = pullStart;
            this.start = start;
            this.end = end;
            this.pullEnd = pullEnd;
        }

        public CatmullRomCurve GetTranslated(Vector2 offset)
        {
            var translatedPoints = new Vector2[Count];
            int i = 0;
            foreach (var point in points)
            {
                translatedPoints[i] = point + offset;
                i++;
            }
            return new CatmullRomCurve(translatedPoints, this.pullStart, this.start, this.end, this.pullEnd);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 translateBy, Depth depth, float lineThickness)
        {
            for (int i = 0; i < Count - 1; i++)
            {
                spriteBatch.DrawLine(this.points[i] + translateBy, this.points[i + 1] + translateBy, Color.White, lineThickness, depth);
            }
        }

        public Vector2 GetPointAt(float progress)
        {
            return Vector2.CatmullRom(this.pullStart, this.start, this.end, this.pullEnd, progress);
        }
    }
}
