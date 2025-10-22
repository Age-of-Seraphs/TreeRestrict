using Cairo;
using System;
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
using Vintagestory.Common;
using Vintagestory.ServerMods.NoObf;

namespace TreeRestrict
{
    public class TreeRestrictModSystem : ModSystem
    {
        public static TreeRestrictConfig serverConfig;
        public override void Start(ICoreAPI api)
        {
            var modid = this.Mod.Info.ModID;
            if (api.Side == EnumAppSide.Server)
            {
                LoadServersideConfig((ICoreServerAPI)api);
            }

            api.RegisterBlockBehaviorClass(modid + ":BlockBehaviorSaplingClimate", typeof(BlockBehaviorSaplingClimate));
            api.RegisterBlockEntityBehaviorClass(modid + ":BEBehaviorSaplingClimate", typeof(BlockEntityBehaviorSaplingClimate));
        }

        public override void AssetsLoaded(ICoreAPI api)
        {
            if (api.Side == EnumAppSide.Client) { return; }

            var treeVariants = api.Assets.Get("worldgen/treengenproperties.json").ToObject<TreeGenProperties>().TreeGens;

            api.ObjectCache["saplingClimateConditionCache"] = treeVariants.GroupBy(item => item.Generator.Path.ToString())
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
                    MaxHeight = group.Min(item => item.MaxHeight),

                    TempMid = (int)(group.Max(item => item.MaxTemp) + group.Min(item => item.MinTemp)) / 2,
                    TempRange = (int)(group.Max(item => item.MaxTemp) - group.Min(item => item.MinTemp)) / 2,

                    RainMid = (int)(group.Max(item => item.MaxRain) + group.Min(item => item.MinRain)) / 2,
                    RainRange = (int)(group.Max(item => item.MaxRain) - group.Min(item => item.MinRain)) / 2,

                    FertMid = (int)(group.Max(item => item.MaxFert) + group.Min(item => item.MinFert)) / 2,
                    FertRange = (int)(group.Max(item => item.MaxFert) - group.Min(item => item.MinFert)) / 2,

                    ForestMid = (int)(group.Max(item => item.MaxForest) + group.Min(item => item.MinForest)) / 2,
                    ForestRange = (int)(group.Max(item => item.MaxForest) - group.Min(item => item.MinForest)) / 2,

                    HeightMid = (int)(group.Max(item => item.MaxHeight) + group.Min(item => item.MinHeight)) / 2,
                    HeightRange = (float)(group.Max(item => item.MaxHeight) - group.Min(item => item.MinHeight)) / 2
                })
                .ToDictionary(x => x.AssetLocation.Path, x => x);
        }
        private void LoadServersideConfig(ICoreServerAPI api)
        {
            try
            {
                serverConfig = api.LoadModConfig<TreeRestrictConfig>("");
                if (serverConfig == null) //if the 'MyConfigData.json' file isn't found...
                {
                    serverConfig = new TreeRestrictConfig();
                }
                //Save a copy of the mod config.
                api.StoreModConfig<TreeRestrictConfig>(serverConfig, "treerestrict-server.json");
            }
            catch (Exception e)
            {
                //Couldn't load the mod config... Create a new one with default settings, but don't save it.
                Mod.Logger.Error("Could not load config! Loading default settings instead.");
                Mod.Logger.Error(e);
                serverConfig = new TreeRestrictConfig();
            }
        }
    }
}
