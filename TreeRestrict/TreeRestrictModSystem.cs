using System.Collections.Generic;
using System.Linq;
using TreeRestrict.src.behaviors;
using TreeRestrict.src.systems;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.ServerMods.NoObf;

namespace TreeRestrict
{
    public class TreeRestrictModSystem : ModSystem
    {
        
        public List<SaplingClimateCondition> saplingClimateConditions;
        public override void Start(ICoreAPI api)
        {
            var modid = this.Mod.Info.ModID;
            api.RegisterBlockClass(modid + ":BlockBehaviorSaplingClimate",typeof(BlockBehaviorSaplingClimate));
            api.RegisterBlockEntityBehaviorClass(modid + ":BEBehaviorSaplingClimate", typeof(BlockEntityBehaviorSaplingClimate));
        }
       
        public override void AssetsLoaded(ICoreAPI api)
        {
            if(api.Side == EnumAppSide.Client) { return; }
            TreeVariant[] treeVariants = api.Assets.Get("worldgen/treengenproperties.json").ToObject<TreeGenProperties>().TreeGens;

            saplingClimateConditions = treeVariants
                .GroupBy(item => item.Generator)
                .Select(group => new SaplingClimateCondition
                {
                    AssetLocation = group.Key,
                    MinTemp = (int)group.Min(item => item.MinTemp),
                    MaxTemp = (int)group.Max(item => item.MaxTemp),
                    MinRain = (int)group.Min(item => item.MinRain),
                    MaxRain = (int)group.Max(item => item.MaxRain),
                    MinFert = (int)group.Min(item => item.MinFert),
                    MaxFert = (int)group.Max(item => item.MaxFert),
                    MinForest = (int)group.Min(item => item.MinForest),
                    MaxForest = (int)group.Max(item => item.MaxForest),
                    MinHeight = (float)group.Min(item => item.MinHeight),
                    MaxHeight = (float)group.Max(item => item.MaxHeight)
                }).ToList();
            api.Logger.Error(saplingClimateConditions.ToString());
        }
    }
}
