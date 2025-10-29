using Cairo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TreeRestrict.src.Blocks;
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
                api.Logger.Event(serverConfig.ToString());
            }
            api.RegisterBlockEntityClass(modid + ":BlockEntityClimatizedSapling", typeof(BlockEntityClimatizedSapling));
        }

        public override void AssetsLoaded(ICoreAPI api)
        {
            if (api.Side == EnumAppSide.Client) { return; }

            var treeVariants = api.Assets.Get("worldgen/treengenproperties.json").ToObject<TreeGenProperties>().TreeGens;
            api.ObjectCache["saplingClimateConditionCache"] = treeVariants.GroupBy(item => item.Generator.Path.ToString())
                .Select(group => new SaplingClimateCondition
                {
                    AssetLocation = group.First().Generator,
                    
                    MinTemp = Math.Clamp(Climate.DescaleTemperature(group.Min(item => item.MinTemp)), 0, 255),
                    MaxTemp = Math.Clamp(Climate.DescaleTemperature(group.Max(item => item.MaxTemp)), 0, 255),
                    
                    MinRain = Math.Clamp(group.Min(item => item.MinRain) + serverConfig.MinRainAddition, 0, 255) / 255f,
                    MaxRain = Math.Clamp(group.Max(item => item.MaxRain) + serverConfig.MaxRainAddition, 0,255) / 255f,
                    
                    MinFert = Math.Clamp(group.Min(item => item.MinFert) + serverConfig.MinFertAddition, 0, 255) / 255f,
                    MaxFert = Math.Clamp(group.Max(item => item.MaxFert) + serverConfig.MaxFertAddition, 0, 255) / 255f,
                    
                    MinForest = Math.Clamp(group.Min(item => item.MinForest) + serverConfig.MinForestAddition, 0, 255) / 255f,
                    MaxForest = Math.Clamp(group.Max(item => item.MaxForest) + serverConfig.MaxForestAddition, 0, 255) / 255f,

                    MinHeight = Math.Clamp(group.Min(item => item.MinHeight) + serverConfig.MinHeightAddition, 0, 1),
                    MaxHeight = Math.Clamp(group.Max(item => item.MaxHeight) + serverConfig.MaxHeightAddition, 0, 1)
                })
                .ToDictionary(x => x.AssetLocation.Path, x => x);
        }
        private void LoadServersideConfig(ICoreServerAPI api)
        {
            try
            {
                serverConfig = api.LoadModConfig<TreeRestrictConfig>("treerestrict-server.json");
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
