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
        public bool enableStuntedGrowthForest = true;
        public bool enableStuntedGrowthHeight = true;

        //these are added to all objects in treegenproperties.json for each respective property. (0 - 255)
        public int MinTempAddition = 0;
        public int MaxTempAddition = 0;

        public int MinRainAddition = 0;
        public int MaxRainAddition = 0;

        public int MinFertAddition = 0;
        public int MaxFertAddition = 0;

        public int MinForestAddition = 0;
        public int MaxForestAddition = 0;

        public int MinHeightAddition = 0;
        public int MaxHeightAddition = 0;

        //height ingame is normally between 0 and 1. These here are added to the min and max height values from the tree gen properties. They are very sensetive.
        //setting both to 1f will essentially disable things.
        public float heightMinBound = 0f;
        public float heightMaxBoundAddition = 0f;


        // SaplingClimateCondition categories. Blacklist will prevent the treeGens in the array from being entered into the object cache. Objects in wh whitelist will be categorized by the key value
        /*
        public string[] treeGenWhitelist = new string[]
        {
            silverbirch:["riverbirch","himalayanbirch"],
            "englishoak":["oldenglishoak"],
            "larch":["deepforestlarch" ],
            "sugarmaple":["japanesemaple","norwaymaple","mountainmaple" ],
             "scotspine":["bristleconepine","mountainpine"] 
        };
        */
        private class treeGenWhitelistDefault
        {
            public string[] silverbirch = ["riverbirch", "himalayanbirch"];
            public string[] englishoak = ["oldenglishoak"];
            public string[] larch = ["deepforestlarch"];
            public string[] sugarmaple = ["japanesemaple", "norwaymaple", "mountainmaple"];
            //public string[] scotspine = ["bristleconepine", "mountainpine"];
        };

        public object treeGenWhitelist = new treeGenWhitelistDefault();

        public string[] treeGenBlacklist = new string[] {"bamboo-grown-green", "bamboo-grown-brown" };

    }
}
