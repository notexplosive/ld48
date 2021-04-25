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
                // Smallfish -> Easy
                FishCount = 8,
                FishStats = FishStats.tinyPlayful,
            },

            new Level
            {
                // Smallfish -> Medium
                FishCount = 3,
                FishStats = FishStats.tinyErratic,
            },

            new Level
            {
                // Seaweed -> Medium
                FishCount = 3,
                FishStats = FishStats.fastChonkers,
                Seaweed = SeaweedInfo.forest
            },

            new Level
            {
                // Seaweed -> Medium 2
                FishCount = 3,
                FishStats = FishStats.easyChonkers,
                Seaweed = SeaweedInfo.sidesVeryHard
            },

            new Level
            {
                // Seaweed -> Introduce
                FishCount = 5,
                FishStats = FishStats.medium,
                Seaweed = SeaweedInfo.sidesHard
            },

            new Level
            {
                // First level
                FishCount = 3,
                FishStats = FishStats.extremelyEasy,
            },

            new Level
            {
                // Seaweed -> Hard
                FishCount = 2,
                FishStats = FishStats.tiny,
                Seaweed = SeaweedInfo.forest
            },

            new Level
            {
                // Lots of jellyfish, finding the fish is challenging but otherwise easy
                FishCount = 5,
                FishStats = FishStats.extremelyEasy,
                JellyfishCount = 15
            },

            new Level
            {
                FishCount = 3,
                FishStats = FishStats.easyChonkers
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
