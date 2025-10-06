using Cairo;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TreeRestrict.src.behaviors;
using TreeRestrict.src.systems;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.ServerMods.NoObf;

namespace TreeRestrict
{
    public class TreeRestrictModSystem : ModSystem
    {
        public object testobj;
        public override void Start(ICoreAPI api)
        {
            var modid = this.Mod.Info.ModID;
            api.RegisterBlockBehaviorClass(modid + ":BlockBehaviorSaplingClimate", typeof(BlockBehaviorSaplingClimate));
            api.RegisterBlockEntityBehaviorClass(modid + ":BEBehaviorSaplingClimate", typeof(BlockEntityBehaviorSaplingClimate));
        }
       
        public override void AssetsLoaded(ICoreAPI api)
        {
            if(api.Side == EnumAppSide.Client) { return; }
            var treeVariants = api.Assets.Get("worldgen/treengenproperties.json").ToObject<TreeGenProperties>().TreeGens;
            ObjectCacheUtil.GetOrCreate(api, "saplingClimateConditionCache", () => {
                return treeVariants.GroupBy(item => item.Generator.Path.ToString())
                .Select(group => new SaplingClimateCondition
                {
                    AssetLocation = group.First().Generator,
                    MinTemp = (int)group.Min(item => item.MinTemp),
                    MaxTemp = (int)group.Max(item => item.MaxTemp),
                    MinRain = (int)group.Min(item => item.MinRain),
                    MaxRain = (int)group.Min(item => item.MaxRain),
                    MinFert = (int)group.Min(item => item.MinFert),
                    MaxFert = (int)group.Min(item => item.MaxFert),
                    MinForest = (int)group.Min(item => item.MinForest),
                    MaxForest = (int)group.Min(item => item.MaxForest),
                    MinHeight = group.Min(item => item.MinHeight),
                    MaxHeight = group.Min(item => item.MaxHeight) 
                })
                .ToDictionary(x => x.AssetLocation.Path, x => x);
            });

           // api.Logger.Notification($"[TreeRestrict]: Loaded {((api.ObjectCache.Values.).Count()} sapling climate conditions");
          //  api.Logger.Notification($"[TreeRestrict]: Loaded {((IEnumerable<SaplingClimateCondition>)testobj).Count()} sapling climate conditions");
        }
    }
}
