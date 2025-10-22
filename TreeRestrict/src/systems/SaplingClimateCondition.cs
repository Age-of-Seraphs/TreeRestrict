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
        
        public int MinTemp = -40;

        
        public int MaxTemp = 40;

        public int MinRain;

        
        public int MaxRain = 255;

        
        public int MinFert;

        
        public int MaxFert = 255;

        
        public int MinForest;

        
        public int MaxForest = 255;

        
        public float MinHeight = 1f;

        
        public float MaxHeight;

        public int TempMid;

        public float TempRange = 1f;

        public int RainMid;

        public float RainRange = 1f;

        public int FertMid;

        public float FertRange = 1f;

        public int ForestMid;

        public float ForestRange = 1f;

        public float HeightMid;

        public float HeightRange = 1f;

    }
}
