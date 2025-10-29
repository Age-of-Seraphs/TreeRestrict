using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

namespace TreeRestrict.src.systems
{
    public class SaplingClimateCondition
    {
        public AssetLocation AssetLocation = new AssetLocation();
        
        public int MinTemp = 0;

        
        public int MaxTemp = 255;

        public float MinRain = 0f;

        
        public float MaxRain = 1f;

        
        public float MinFert = 0f;

        
        public float MaxFert = 1f;

        
        public float MinForest = 1f;

        
        public float MaxForest = 1f;

        
        public float MinHeight = 0f;

        
        public float MaxHeight = 1f;
    }
}
