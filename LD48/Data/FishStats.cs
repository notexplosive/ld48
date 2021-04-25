﻿using System;
using System.Collections.Generic;
using System.Text;

namespace LD48.Data
{
    public class FishStats
    {
        public float Inertia;
        public float TerminalSpeed;
        public float SizeLevel;
        public float AttentionSpan = 5;

        public static FishStats basic = new FishStats
        {
            Inertia = 2,
            TerminalSpeed = 2,
            SizeLevel = 5,
        };

        public static FishStats jellyfish = new FishStats
        {
            AttentionSpan = 1,
            Inertia = 2,
            TerminalSpeed = 2,
            SizeLevel = 16
        };

        public static FishStats chonkers = new FishStats
        {
            Inertia = 15,
            TerminalSpeed = 10,
            SizeLevel = 8,
        };

        public static FishStats tiny = new FishStats
        {
            Inertia = 30,
            TerminalSpeed = 12,
            SizeLevel = 2,
        };

        public static FishStats medium = new FishStats
        {
            Inertia = 30,
            TerminalSpeed = 13,
            SizeLevel = 3,
        };
    }
}
