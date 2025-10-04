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
        public AssetLocation AssetLocation;
        
        public int MinTemp = -40;

        
        public int MaxTemp = 40;

        
        public int MinRain;

        
        public int MaxRain = 255;

        
        public int MinFert;

        
        public int MaxFert = 255;

        
        public int MinForest;

        
        public int MaxForest = 255;

        
        public float MinHeight;

        
        public float MaxHeight = 1f;
    }
}
