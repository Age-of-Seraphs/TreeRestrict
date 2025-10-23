using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeRestrict.src.systems
{
    public class TreeRestrictConfig
    {
        public bool enableDebugLogging = false;

        public bool enableStuntedGrowthTempurature = true;
        public bool enableStuntedGrowthRain = true;
        public bool enableStuntedGrowthFertility = true;
        public bool enableStuntedGrowthForest = true;
        public bool enableStuntedGrowthHeight = true;

        public float temperatureRangeMultiplier = 1f;
        public float rainRangeMultiplier = 1f;
        public float fertilityRangeMultiplier = 1f;
        public float forestRangeMultiplier = 1f;
        public float heightRangeMultiplier = 1f;
    }
}
