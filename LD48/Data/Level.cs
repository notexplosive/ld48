using LD48.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace LD48.Data
{
    public class Level
    {
        public int FishCount;
        public int JellyfishCount;
        public FishStats FishStats;
        public SeaweedInfo[] Seaweed = Array.Empty<SeaweedInfo>();
        public bool DamagedHarness;
        public bool HarnessVulnerable;


        public static readonly Level[] All = new Level[]
        {
            new Level
            {
                // First level
                FishCount = 3,
                FishStats = FishStats.extremelyEasy,
            },

            // SMALL FISH

            new Level
            {
                // Smallfish -> Easy
                FishCount = 5,
                FishStats = FishStats.tinyLinger,
            },

            new Level
            {
                // Smallfish -> Medium
                FishCount = 4,
                FishStats = FishStats.tiny,
            },

            new Level
            {
                // Smallfish -> Hard
                FishCount = 5,
                FishStats = FishStats.tinyErratic,
            },

            // SEAWEED

            new Level
            {
                // Seaweed -> Easy
                FishCount = 5,
                FishStats = FishStats.medium,
                Seaweed = SeaweedInfo.sidesHard
            },

            new Level
            {
                // Seaweed -> Medium
                FishCount = 3,
                FishStats = FishStats.mediumLinger,
                Seaweed = SeaweedInfo.forest
            },

            new Level
            {
                // Seaweed -> Medium 2
                FishCount = 6,
                FishStats = FishStats.easyChonkers,
                Seaweed = SeaweedInfo.sidesVeryHard
            },

            new Level
            {
                // Seaweed -> Hard
                FishCount = 5,
                FishStats = FishStats.tiny,
                Seaweed = SeaweedInfo.forest
            },


            // JELLYFISH

            new Level
            {
                // Jellyfish - Easy
                // Lots of jellyfish, finding the fish is challenging but otherwise easy
                FishCount = 8,
                FishStats = FishStats.fastChonkers,
                JellyfishCount = 1,
            },

            new Level
            {
                // Jellyfish - Medium
                // Lots of jellyfish, finding the fish is challenging but otherwise easy
                FishCount = 6,
                FishStats = FishStats.extremelyEasy,
                JellyfishCount = 6
            },

            new Level
            {
                // Jellyfish - Hard
                // Lots of jellyfish, finding the fish is challenging but otherwise easy
                FishCount = 5,
                FishStats = FishStats.tinyErratic,
                JellyfishCount = 5,
            },

            // MIXTURE

            new Level
            {
                FishCount = 5,
                FishStats = FishStats.tinyErratic,
                Seaweed = SeaweedInfo.forest,
                JellyfishCount = 1,
                DamagedHarness = true,
            },

            // FINALE
            new Level
            {
                // Final level
                DamagedHarness = true,
                HarnessVulnerable = true
            },
        };

    }
}
