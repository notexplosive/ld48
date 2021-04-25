using LD48.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace LD48.Data
{
    class Level
    {
        public int FishCount = 1;
        public FishStats FishStats;

        public static Level[] Array = new Level[]
        {
            new Level
            {
                FishCount = 1,
                FishStats = FishStats.basic
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

            new Level
            {
                FishCount = 12,
                FishStats = FishStats.medium
            }

        };
    }
}
