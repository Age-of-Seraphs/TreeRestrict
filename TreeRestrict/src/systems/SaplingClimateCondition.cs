using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

namespace TreeRestrict.src.systems
{
    public class SaplingClimateCondition
    {
        public HashSet<string> AssetLocations = new HashSet<string>();
        //new List<AssetLocation>();
        
        public float MinTemp = -20f;

        
        public float MaxTemp = 40f;

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
