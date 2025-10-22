using HarmonyLib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TreeRestrict;
using TreeRestrict.src.systems;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.Common;
using Vintagestory.GameContent;
using Vintagestory.Server;
using Vintagestory.ServerMods.NoObf;

namespace TreeRestrict.src.behaviors
{
    public class BlockBehaviorSaplingClimate : BlockBehavior
    {
        public BlockBehaviorSaplingClimate(Block block)
        : base(block){}

        private ClimateCondition placeClimate;

        private object saplingClimateConds;

        private TreeRestrictConfig serverConfig = TreeRestrictModSystem.serverConfig;
        public override void Initialize(JsonObject properties)
        {
            base.Initialize(properties);
        }
        public override bool DoPlaceBlock(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ItemStack byItemStack, ref EnumHandling handling)
        {
            if (world.Api.Side == EnumAppSide.Client)
            {
                return true;
            }
            var api = world.Api;
            var attributes = block.Attributes?["treeGen"].AsString();
            var treeType = AssetLocation.Create(attributes, block.Code.Domain).Path;
            var Pos = blockSel.Position;
            placeClimate = world.BlockAccessor.GetClimateAt(Pos, EnumGetClimateMode.WorldGenValues);
            
            Dictionary<string, SaplingClimateCondition> saplingClimateConds = ObjectCacheUtil.TryGet<Dictionary<string,SaplingClimateCondition>>(world.Api, "saplingClimateConditionCache");
            if (saplingClimateConds == null)
            {
                api.Logger.Error("Unable to find saplingClimateConditionCache for placement at {0} on side: {1}",Pos,api.Side);
                handling = EnumHandling.PreventSubsequent;
                return false;
            }

            /*
            if (BEBehavior == null) 
            {
                api.Logger.Error("tried to apply treerestrict conditions but Sapling BlockEntity at {0} is null", Pos);
                handling = EnumHandling.PreventSubsequent;
                return false;
            }
            */
            
            SaplingClimateCondition cond = new SaplingClimateCondition();



            var stoptemp = 1;
            if (saplingClimateConds.TryGetValue(treeType, out cond))
            {
                api.Logger.Event("placeclimate temp:" + placeClimate.Temperature + " : " + placeClimate.WorldGenTemperature);
                api.Logger.Event("TempMid: " + cond.TempMid + " TempRange: " + cond.TempRange + " TempMax: " + cond.MaxTemp + " TempMin: " + cond.MinTemp);


                int stunts = 0;
                var tempDiff = Math.Abs(placeClimate.Temperature - cond.TempMid);
                if (tempDiff > cond.TempRange) {
                    stunts += 1;
                    api.Logger.Event("stunted by temp");
                    if (tempDiff > cond.TempRange + stoptemp)
                    {
                        stunts += 2;
                        api.Logger.Event("stopped by temp"); 
                    }
                }
                var rainfallDiff = Math.Abs(placeClimate.Rainfall - cond.RainMid);
                if (rainfallDiff > cond.RainMid)
                {
                    stunts += 4;
                    api.Logger.Event("stunted by rainfall");
                    if (rainfallDiff > cond.RainRange + stoptemp)
                    {
                        stunts += 8;
                        api.Logger.Event("stopped by rainfall");
                    }
                }

                var fertDiff = Math.Abs(placeClimate.Rainfall - cond.FertMid);
                if (fertDiff > cond.FertMid)
                {
                    stunts += 0x10;
                    api.Logger.Event("stunted by fertility");
                    if (fertDiff > cond.FertMid + stoptemp)
                    {
                        stunts += 0x20;
                        api.Logger.Event("stopped by fertility");
                    }
                }

                var forestDiff = Math.Abs(placeClimate.ForestDensity - cond.ForestMid);
                if (forestDiff > cond.ForestMid)
                {
                    stunts += 0x40;
                    api.Logger.Event("stunted by forest density");
                    if (forestDiff > cond.ForestMid + stoptemp)
                    {
                        stunts += 0x80;
                        api.Logger.Event("stopped by forest density");
                    }
                }

                var heightDiff = Math.Abs(Pos.Y - cond.HeightMid);
                if (heightDiff > cond.HeightMid)
                {
                    stunts += 0x100;
                    api.Logger.Event("stunted by height");
                    if (heightDiff > cond.HeightMid + stoptemp)
                    {
                        stunts += 0x200;
                        api.Logger.Event("stopped by height");
                    }
                }
            }
            




            return base.DoPlaceBlock(world, byPlayer, blockSel, byItemStack,ref handling);
        }
    }
}
