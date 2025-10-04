using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TreeRestrict;
using TreeRestrict.src.systems;

using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace TreeRestrict.src.behaviors
{
    internal class BlockBehaviorSaplingClimate : BlockBehavior
    {
        private ClimateCondition placeClimate;

        private List<SaplingClimateCondition> saplingClimateConditions;

        public BlockBehaviorSaplingClimate(Block block)
        : base(block)
        {}
        public override bool TryPlaceBlock(IWorldAccessor world, IPlayer byPlayer, ItemStack itemstack, BlockSelection blockSel, ref EnumHandling handling, ref string failureCode)
        {
            var key = AssetLocation.Create(block.Attributes?["treeGen"].AsString(), block.Code.Domain); // Get block attributes
            
            // Read the list from the mod system
            var sys = world.Api.ModLoader.GetModSystem<TreeRestrictModSystem>();
            var conditions = sys?.saplingClimateConditions; // For IReadOnlyList<SaplingClimateCondition>
            var Pos = blockSel.Position;
            placeClimate = world.BlockAccessor.GetClimateAt(Pos, EnumGetClimateMode.WorldGenValues);
            
            var cond = conditions?.FirstOrDefault(c => c.AssetLocation.Equals(key));
            if (cond != null)
            {
                bool allgood =
                  placeClimate.Temperature >= cond.MinTemp && placeClimate.Temperature <= cond.MaxTemp &&
                  placeClimate.Rainfall >= cond.MinRain && placeClimate.Rainfall <= cond.MaxRain &&
                  placeClimate.Fertility >= cond.MinFert && placeClimate.Fertility <= cond.MaxFert &&
                  placeClimate.ForestDensity >= cond.MinForest && placeClimate.ForestDensity <= cond.MaxForest &&
                  Pos.Y >= cond.MinHeight && Pos.Y <= cond.MaxHeight;
                block.GetBEBehavior<BlockEntityBehaviorSaplingClimate>(Pos).growthStunted = allgood;
                //block.Attributes?["growthStunted"]

                if (!allgood)
                {
                    handling = EnumHandling.PreventDefault;
                    failureCode = "This Sapling cannot grow here";
                    world.Api.Logger.Event("cannot place sapling in this region");
                    return false;
                } // Some error or whatever
            }
            
            else {
                throw new Exception("missing entry in world generator for tree");
            }

















                return base.TryPlaceBlock(world, byPlayer, itemstack, blockSel, ref handling, ref failureCode);
        }
    }
}
