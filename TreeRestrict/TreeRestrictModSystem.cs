using Cairo;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using TreeRestrict.src.Blocks;
using TreeRestrict.src.Config;
using TreeRestrict.src.systems;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.Client.NoObf;
using Vintagestory.Common;
using Vintagestory.ServerMods.NoObf;

namespace TreeRestrict
{
    public class TreeRestrictModSystem : ModSystem
    {
        public static TreeRestrictServerConfig serverConfig;

        public static TreeRestrictClientConfig clientConfig;

        string modid = "treerestrict";
        public override void Start(ICoreAPI api)
        {
            modid = Mod.Info.ModID;
            if (api.Side == EnumAppSide.Client)
            {
                LoadClientsideConfig((ICoreClientAPI)api);
            }
            if (api.Side == EnumAppSide.Server)
            {
                LoadServersideConfig((ICoreServerAPI)api);
            }

            //Replaces the vanilla game BlockEntitySapling upon server start. (Should be save to add/remove the mod but I'm just guessing.)
            //This also allows for adding modded sapling support without JSON patching for every possible mod.
            api.RegisterBlockEntityClass("Sapling", typeof(BlockEntityClimatizedSapling));
        }

        public override void AssetsLoaded(ICoreAPI api)
        {
            if (api.Side == EnumAppSide.Client) { return; }

            var treeVariants = api.Assets.Get("worldgen/treengenproperties.json").ToObject<TreeGenProperties>().TreeGens;


            // All values from the configs are normalized before being added to the Object
            api.ObjectCache["saplingClimateConditionCache"] = treeVariants
                .GroupBy(item => withinTreeGenCatagories(item.Generator.Path.ToString()))
                
                //Remove all groups within the blacklist
                .Where(x => !serverConfig.treeGenBlacklist.Contains(x.Key))

                //save tree variants to a new SaplingClimateCondition dictionary.
                .ToDictionary(x => x.Key, group => new SaplingClimateCondition
                {
                    AssetLocations = group.Select(item => item.Generator.Path.ToString()).ToHashSet(),

                    MinTemp = Math.Clamp(group.Min(item => item.MinTemp + serverConfig.MinTempAddition), -20f, 40f),
                    MaxTemp =  Math.Clamp(group.Max(item => item.MaxTemp + serverConfig.MaxTempAddition), -20f, 40f),

                    MinRain = Math.Clamp(group.Min(item => item.MinRain) + serverConfig.MinRainAddition, 0, 255) / 255f,
                    MaxRain = Math.Clamp(group.Max(item => item.MaxRain) + serverConfig.MaxRainAddition, 0, 255) / 255f,

                    MinFert = Math.Clamp(group.Min(item => item.MinFert) + serverConfig.MinFertAddition, 0, 255) / 255f,
                    MaxFert = Math.Clamp(group.Max(item => item.MaxFert) + serverConfig.MaxFertAddition, 0, 255) / 255f,

                    MinForest = Math.Clamp(group.Min(item => item.MinForest) + serverConfig.MinForestAddition, 0, 255) / 255f,
                    MaxForest = Math.Clamp(group.Max(item => item.MaxForest) + serverConfig.MaxForestAddition, 0, 255) / 255f,

                    MinHeight = Math.Clamp(group.Min(item => item.MinHeight) + serverConfig.MinHeightAddition, 0f, 1f),
                    MaxHeight = Math.Clamp(group.Max(item => item.MaxHeight) + serverConfig.MaxHeightAddition, 0f, 1f)
                });
            /*
            foreach (CollectibleObject obj in api.World.Collectibles)
            {
                // Make sure collectible or its code is not null
                if (obj == null || obj.Code == null)
                {
                    continue;
                }

                // Make sure attributes are not null
                if (obj is Block block && block.Class == "Sapling")
                {
             //       obj.Class = modid + ":BlockEntityClimatizedSapling";
                }
            }
            */
        }
        private string withinTreeGenCatagories(string generator)
        {
            foreach (var entry in serverConfig.treeGenCategories)
            {
                if (entry.Value.Contains(generator))
                {
                    return entry.Key;
                }
            }
            return generator;
            
        }
        private void LoadServersideConfig(ICoreServerAPI api)
        {
            try
            {
                serverConfig = api.LoadModConfig<TreeRestrictServerConfig>("Redbeard-Mods/climatespecifictrees-server.json");
                if (serverConfig == null) //if the 'MyConfigData.json' file isn't found...
                {
                    serverConfig = new TreeRestrictServerConfig();
                }
                //Save a copy of the mod config.
                api.StoreModConfig<TreeRestrictServerConfig>(serverConfig, "Redbeard-Mods/climatespecifictrees-server.json");
            }
            catch (Exception e)
            {
                //Couldn't load the mod config... Create a new one with default settings, but don't save it.
                Mod.Logger.Error("Could not load Climate Specific Trees server config! Loading default settings instead.");
                Mod.Logger.Error(e);
                serverConfig = new TreeRestrictServerConfig();
            }
        }
        private void LoadClientsideConfig(ICoreClientAPI api)
        {
            try
            {
                clientConfig = api.LoadModConfig<TreeRestrictClientConfig>("Redbeard-Mods/climatespecifictrees-client.json");
                if (clientConfig == null) //if the 'MyConfigData.json' file isn't found...
                {
                    clientConfig = new TreeRestrictClientConfig();
                }
                //Save a copy of the mod config.
                api.StoreModConfig<TreeRestrictClientConfig>(clientConfig, "Redbeard-Mods/climatespecifictrees-client.json");
            }
            catch (Exception e)
            {
                //Couldn't load the mod config... Create a new one with default settings, but don't save it.
                Mod.Logger.Error("Could not load Climate Specific Trees client config! Loading default settings instead.");
                Mod.Logger.Error(e);
                clientConfig = new TreeRestrictClientConfig();
            }
        }
    }
}
