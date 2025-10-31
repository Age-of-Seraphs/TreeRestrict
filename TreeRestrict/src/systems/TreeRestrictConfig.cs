using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TreeRestrict.src.systems
{
    public class TreeRestrictConfig
    {
        
        public bool enableDebugLogging = false;

        public bool enableStuntedGrowthTempurature = true;
        public bool enableStuntedGrowthRain = true;
        public bool enableStuntedGrowthFertility = true;
        public bool enableStuntedGrowthForest = false;
        public bool enableStuntedGrowthHeight = true;

        //Growth Multipliers for vars. Set to 0 to disable it.
        public float climateGrowthMod = 12f;
        //public float rainfallGrowthMul = 0;
        //public float fertilityGrowthMul = 0;
        //public float forestDensityGrowthMul = 0;
        //public float heightGrowthMul = 0;

        //(-20 | 40)
        public float MinTempAddition = 0f;
        public float MaxTempAddition = 0f;

        //these are added to all objects in treegenproperties.json for each respective property. (0 | 255)
        public int MinRainAddition = 0;
        public int MaxRainAddition = 0;

        public int MinFertAddition = 0;
        public int MaxFertAddition = 0;

        public int MinForestAddition = 0;
        public int MaxForestAddition = 0;

        public int MinHeightAddition = 0;
        public int MaxHeightAddition = 0;

        //height ingame is normally between 0 and 1. These here are added to the min and max height values from the tree gen properties. They are very sensetive.
        //setting both to 1f will essentially disable height checks.
        public float heightMinBound = 1f;
        public float heightMaxBoundAddition = 0f;

        public Dictionary<string, string[]> treeGenCategories = new Dictionary<string, string[]>
        {
            {"silverbirch",["riverbirch", "himalayanbirch"] },
            { "englishoak" , ["oldenglishoak"]},
            {"sugarmaple" , ["japanesemaple", "norwaymaple", "mountainmaple"] },
            {"scotspine" , ["bristleconepine", "mountainpine","fir"] },
            {"larch" , ["deepforestlarch"] },
            {"baldcypress",["baldcypressswamp"] },
            {"kapok",["largekapok", "largekapok2","oldkapok"] }
        };

        public string[] treeGenBlacklist = new string[] {"bamboo-grown-green", "bamboo-grown-brown" };

    }
}
