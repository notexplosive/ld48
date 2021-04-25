using System;
using System.Collections.Generic;
using System.Text;

namespace LD48.Data
{
    public class FishStats
    {
        public float Mass;
        public float TerminalSpeed;
        public float SizeLevel;
        public float LingerTime = 5;

        public static FishStats basic = new FishStats
        {
            Mass = 2,
            TerminalSpeed = 2,
            SizeLevel = 5,
        };

        public static FishStats jellyfish = new FishStats
        {
            LingerTime = 1,
            Mass = 20,
            TerminalSpeed = 5,
            SizeLevel = 10
        };

        public static FishStats chonkers = new FishStats
        {
            Mass = 15,
            TerminalSpeed = 10,
            SizeLevel = 8,
        };

        public static FishStats tiny = new FishStats
        {
            Mass = 30,
            TerminalSpeed = 12,
            SizeLevel = 2,
        };

        public static FishStats medium = new FishStats
        {
            Mass = 30,
            TerminalSpeed = 13,
            SizeLevel = 3,
        };
    }
}
