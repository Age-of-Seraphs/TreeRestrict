using HarmonyLib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TreeRestrict;
using TreeRestrict.src.systems;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace TreeRestrict.src.behaviors
{
    public class BlockBehaviorSaplingClimate : BlockBehavior
    {
        public BlockBehaviorSaplingClimate(Block block)
        : base(block){}

        private ClimateCondition placeClimate;

        public override bool TryPlaceBlock(IWorldAccessor world, IPlayer byPlayer, ItemStack itemstack, BlockSelection blockSel, ref EnumHandling handling, ref string failureCode)
        {
            var attr = block.Attributes?["treeGen"].AsString();
            var key = AssetLocation.Create(attr, block.Code.Domain).Path; // Get block attributes
            var Pos = blockSel.Position;
            placeClimate = world.BlockAccessor.GetClimateAt(Pos, EnumGetClimateMode.WorldGenValues);

           // SaplingClimateCondition conditional =
            ObjectCacheUtil.TryGet<Dictionary<string,SaplingClimateCondition>>(world.Api,"saplingClimateConditionCache").TryGetValue(key, out SaplingClimateCondition value);
            
            if (value != null)
            {
                bool allgood =
                  placeClimate.Temperature >= value.MinTemp && placeClimate.Temperature <= value.MaxTemp &&
                  placeClimate.Rainfall >= value.MinRain && placeClimate.Rainfall <= value.MaxRain &&
                  placeClimate.Fertility >= value.MinFert && placeClimate.Fertility <= value.MaxFert &&
                  placeClimate.ForestDensity >= value.MinForest && placeClimate.ForestDensity <= value.MaxForest &&
                  Pos.Y >= value.MinHeight && Pos.Y <= value.MaxHeight;
             
                if (!allgood)
                {
                    world.Api.Logger.Event("cannot place sapling in this region");
                    return false;
                } // Some error or whatever
            }
            return base.TryPlaceBlock(world, byPlayer, itemstack, blockSel, ref handling, ref failureCode);
        }
    }
}
