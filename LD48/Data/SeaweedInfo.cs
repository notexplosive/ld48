using System;
using System.Collections.Generic;
using System.Text;

namespace LD48.Data
{
    public class SeaweedInfo
    {
        public float XPercent = 0.5f;

        /// <summary>
        /// Default angle is upright
        /// </summary>
        public float Angle = 0;
        public int NodeCount = 5;

        public static SeaweedInfo[] sidesEasy =
            new SeaweedInfo[] { new SeaweedInfo { XPercent = -0.1f, Angle = MathF.PI / 8, NodeCount = 10 }, new SeaweedInfo { XPercent = 1.1f, Angle = -MathF.PI / 8, NodeCount = 10 } };
        public static SeaweedInfo[] sidesMedium =
            new SeaweedInfo[] { new SeaweedInfo { XPercent = -0.1f, Angle = MathF.PI / 8, NodeCount = 15 }, new SeaweedInfo { XPercent = 1.1f, Angle = -MathF.PI / 8, NodeCount = 15 } };
        public static SeaweedInfo[] sidesHard =
            new SeaweedInfo[] { new SeaweedInfo { XPercent = -0.1f, Angle = MathF.PI / 4, NodeCount = 15 }, new SeaweedInfo { XPercent = 1.1f, Angle = -MathF.PI / 4, NodeCount = 15 } };
        public static SeaweedInfo[] forest =
            new SeaweedInfo[] {
                new SeaweedInfo { XPercent = 0.15f, Angle = 0, NodeCount = 15 },
                new SeaweedInfo { XPercent = 0.35f, Angle = 0, NodeCount = 15 },
                new SeaweedInfo { XPercent = 0.65f, Angle = 0, NodeCount = 15 },
                new SeaweedInfo { XPercent = 0.85f, Angle = 0, NodeCount = 15 }
            };

        public float Length => NodeCount * 60;
    }
}
