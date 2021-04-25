using LD48.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace LD48.Data
{
    class Level
    {
        public int FishCount = 1;
        public int JellyfishCount;
        public FishStats FishStats;
        public SeaweedInfo[] Seaweed = Array.Empty<SeaweedInfo>();

        public static Level[] All = new Level[]
        {
            new Level
            {
                FishCount = 1,
                FishStats = FishStats.basic,
                JellyfishCount = 3
            },

            new Level
            {
                // Pretty good introduction to Seaweed mechanic
                FishCount = 2,
                FishStats = FishStats.tiny,
                Seaweed = SeaweedInfo.forest
            },

            new Level
            {
                FishCount = 3,
                FishStats = FishStats.chonkers
            },

            new Level
            {
                FishCount = 5,
                FishStats = FishStats.tiny
            },

            // Late hard level
            new Level
            {
                FishCount = 12,
                FishStats = FishStats.medium,
                Seaweed = SeaweedInfo.sidesEasy
            }

        };
    }
}
