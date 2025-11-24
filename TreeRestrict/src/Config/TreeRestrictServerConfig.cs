using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TreeRestrict.src.Config
{
    public class TreeRestrictServerConfig
    {

        //Enable/Disable stunted growth for each variable.
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

        /*these are added to all objects in treegenproperties.json for each respective property. (0 | 255) 
         * Setting one of these basically shifts the min/max value for every single sapling type.
         * See TreeRestrictModSystem.AssetsLoaded for details on how these are applied.
         */
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
        public float heightMinBound = 0f;
        public float heightMaxBoundAddition = 0f;


        //Blacklist is run before the catagory dictionary If a treeVariant is inside this string array then it means it will be excluded from climate checks.
        public string[] treeGenBlacklist = new string[] { "bamboo-grown-green", "bamboo-grown-brown" };


        //Dictionary of tree gen categories. Basically it links all trees inside the array to the key value specified in a sapling's treegen property.
        //It's not neccecary to put the key in the values array, it checks for both.

        //Due to this config, This means that all pine trees, from cold weather pines to tropical pines, will share the same minimum and maximum climate conditions.
        public Dictionary<string, string[]> treeGenCategories = new Dictionary<string, string[]>
        {
            //Vanilla 
            {"silverbirch",["riverbirch", "himalayanbirch"] },
            { "englishoak" , ["oldenglishoak"]},
            {"sugarmaple" , ["japanesemaple", "norwaymaple", "mountainmaple"] },
            {"scotspine" , ["bristleconepine", "mountainpine","fir"] },
            {"larch" , ["deepforestlarch"] },
            {"baldcypress",["baldcypressswamp"] },
            {"kapok",["largekapok", "largekapok2","oldkapok"] },

            //Floral Zones
            {"acaciaaneura",["acaciatetragonophylla"] },
            {"afrocarpusfalcatus",["afrocarpusfalcatuslarge"]},
            {"araucariaheterophylla",["araucariaheterophyllalarge"] },
            {"birch",["betulaplatyphylla"] },
            {"caricapapaya",["caricapapayaempty"] },
            {"cocosnucifera",["cocosnuciferaempty", "cocosnuciferalow", "cocosnuciferashort"] },
            {"corymbiaaparrerinja",["corymbiaaparrerinjacrooked"] },
            {"ceibapentandra",["ceibapentandralarge"] },
            {"dacrydiumcupressinum",["dacrydiumcupressinummedium", "dacrydiumcupressinumsmall"] },
            {"diospyroswhyteana",["diospyroswhyteanaempty"] },
            {"lagunculariaracemosa",["lagunculariaracemosaempty"] },
            {"nothofagusmenziesii",["nothofagusmenziesiilarge"] },
            {"pinuscaribaea",["pinusdensiflora", "pinushalepensis", "pinustecunumanii"] },            
            {"podocarpustotara",["podocarpustotaralarge"] },
            {"psuedopanaxcrassifolius",["psuedopanaxcrassifoliusyoung"] },
            {"pterocelastrustricuspidatus",["pterocelastrustricuspidatuslarge"] },
            {"quercuscostaricensis",["quercuscostaricensistall"] },
            {"roystonearegia",["roystonearegiaempty"] }
        };
    }
}
