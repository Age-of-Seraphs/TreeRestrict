using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeRestrict.src.Config;
using TreeRestrict.src.systems;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.GameContent;
using Vintagestory.ServerMods;

namespace TreeRestrict.src.Blocks
{
    public class BlockEntityClimatizedSapling : BlockEntity
    {
        private double totalHoursTillGrowth;

        private long growListenerId;

        public EnumTreeGrowthStage stage;

        public bool plantedFromSeed;

        private NormalRandom normalRandom;

        private MeshData dirtMoundMesh
        {
            get
            {
                ICoreClientAPI capi = Api as ICoreClientAPI;
                if (capi == null)
                {
                    return null;
                }

                return ObjectCacheUtil.GetOrCreate(Api, "dirtMoundMesh", delegate
                {
                    Shape shape = Shape.TryGet(capi, AssetLocation.Create("shapes/block/plant/dirtmound.json", base.Block.Code.Domain));
                    capi.Tesselator.TesselateShape(base.Block, shape, out var modeldata);
                    return modeldata;
                });
            }
        }

        private NatFloat nextStageDaysRnd
        {
            get
            {
                if (stage == EnumTreeGrowthStage.Seed)
                {
                    NatFloat natFloat = NatFloat.create(EnumDistribution.UNIFORM, 1.5f, 0.5f);
                    if (base.Block?.Attributes != null)
                    {
                        return base.Block.Attributes["growthDays"].AsObject(natFloat);
                    }

                    return natFloat;
                }

                NatFloat natFloat2 = NatFloat.create(EnumDistribution.UNIFORM, 7f, 2f);
                if (base.Block?.Attributes != null)
                {
                    return base.Block.Attributes["matureDays"].AsObject(natFloat2);
                }

                return natFloat2;
            }
        }

        private float GrowthRateMod => Api.World.Config.GetString("saplingGrowthRate").ToFloat(1f);

        #region climate vars

        static TreeRestrictServerConfig modConfig = TreeRestrictModSystem.serverConfig;

        private HashSet<string> treeGens;

        private bool canGrowEver;

        private float[] climateFlags;

        ClimateCondition nowClimate
        {
            get
            {
                return Api.World.BlockAccessor.GetClimateAt(Pos, EnumGetClimateMode.WorldGenValues);
            }
        }

        private float GrowthRateModClimate;
        #endregion climate vars

        public override void Initialize(ICoreAPI api)
        {
            base.Initialize(api);

            if (api is ICoreServerAPI)
            {
                if (climateFlags == null || treeGens == null)
                {
                    setClimateFlags();
                }
                if (canGrowEver)
                {
                    normalRandom = new NormalRandom(api.World.Seed);
                    growListenerId = RegisterGameTickListener(CheckGrow, 2000);
                }
            }
        }

        public override void OnBlockPlaced(ItemStack byItemStack = null)
        {
            stage = ((!(byItemStack?.Collectible is ItemTreeSeed)) ? EnumTreeGrowthStage.Sapling : EnumTreeGrowthStage.Seed);
            plantedFromSeed = stage == EnumTreeGrowthStage.Seed;
            totalHoursTillGrowth = Api.World.Calendar.TotalHours + (double)(nextStageDaysRnd.nextFloat(1f, Api.World.Rand) * 24f * GrowthRateMod * GrowthRateModClimate);
        }

        private void CheckGrow(float dt)
        {
            if (Api.World.Calendar.TotalHours < totalHoursTillGrowth || Api.World.BlockAccessor.GetClimateAt(Pos, EnumGetClimateMode.ForSuppliedDate_TemperatureOnly, Api.World.Calendar.TotalDays).Temperature < 5f)
            {
                return;
            }

            if (stage == EnumTreeGrowthStage.Seed)
            {
                stage = EnumTreeGrowthStage.Sapling;
                totalHoursTillGrowth = Api.World.Calendar.TotalHours + (double)(nextStageDaysRnd.nextFloat(1f, Api.World.Rand) * 24f * GrowthRateMod * GrowthRateModClimate);
                MarkDirty(redrawOnClient: true);
                return;
            }

            BlockFacing[] hORIZONTALS = BlockFacing.HORIZONTALS;
            for (int i = 0; i < hORIZONTALS.Length; i++)
            {
                Vec3i normali = hORIZONTALS[i].Normali;
                int posX = Pos.X + normali.X * 32;
                int posZ = Pos.Z + normali.Z * 32;
                if (Api.World.BlockAccessor.IsValidPos(posX, Pos.InternalY, posZ) && Api.World.BlockAccessor.GetChunkAtBlockPos(posX, Pos.InternalY, posZ) == null)
                {
                    return;
                }
            }

            string text = Api.World.BlockAccessor.GetBlock(Pos).Attributes?["treeGen"].AsString();
            if (text == null || !canGrowEver)
            {
                UnregisterGameTickListener(growListenerId);
                growListenerId = 0L;
                return;
            }

            AssetLocation key = new AssetLocation(text);
            if (!(Api as ICoreServerAPI).World.TreeGenerators.TryGetValue(key, out var value))
            {
                UnregisterGameTickListener(growListenerId);
                growListenerId = 0L;
                return;
            }

            Api.World.BlockAccessor.SetBlock(0, Pos);
            Api.World.BulkBlockAccessor.ReadFromStagedByDefault = true;
            float size = 0.6f + (float)Api.World.Rand.NextDouble() * 0.5f;
            TreeGenParams treeGenParams = new TreeGenParams
            {
                skipForestFloor = true,
                size = size,
                otherBlockChance = 0f,
                vinesGrowthChance = 0f,
                mossGrowthChance = 0f
            };
            value.GrowTree(Api.World.BulkBlockAccessor, Pos.DownCopy(), treeGenParams, normalRandom);
            Api.World.BulkBlockAccessor.Commit();
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);
            tree.SetDouble("totalHoursTillGrowth", totalHoursTillGrowth);
            tree.SetInt("growthStage", (int)stage);
            tree.SetBool("plantedFromSeed", plantedFromSeed);
            tree.SetBool("canGrowEver", canGrowEver);
            tree.SetFloat("growthRateModClimate", GrowthRateModClimate);
            if (treeGens != null)
            {
                tree.SetString("treeGens", string.Join(',', treeGens));
            }
            tree.SetString("climateFlags", string.Join(",", climateFlags));
        }

        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldForResolving)
        {
            base.FromTreeAttributes(tree, worldForResolving);
            totalHoursTillGrowth = tree.GetDouble("totalHoursTillGrowth");
            stage = (EnumTreeGrowthStage)tree.GetInt("growthStage", 1);
            plantedFromSeed = tree.GetBool("plantedFromSeed");
            canGrowEver = tree.GetBool("canGrowEver");
            GrowthRateModClimate = tree.GetFloat("growthRateModClimate", 1f);
            var treeGenString = tree.GetString("treeGens");
            if (treeGenString != null)
            {
                treeGens = treeGenString.Split(',').ToHashSet();
            }
            var stringFlags = tree.GetString("climateFlags");
            if (stringFlags != null)
            {
                climateFlags = tree.GetString("climateFlags").Split(',').Select(float.Parse).ToArray();
            }
            else if (Api is ICoreServerAPI)
            {
                setClimateFlags();
            }
        }

        public ItemStack[] GetDrops()
        {
            if (stage == EnumTreeGrowthStage.Seed)
            {
                Item item = Api.World.GetItem(AssetLocation.Create("treeseed-" + base.Block.Variant["wood"], base.Block.Code.Domain));
                return new ItemStack[1]
                {
                new ItemStack(item)
                };
            }

            return new ItemStack[1]
            {
            new ItemStack(base.Block)
            };
        }

        public string GetBlockName()
        {
            if (stage == EnumTreeGrowthStage.Seed)
            {
                return Lang.Get("treeseed-planted-" + base.Block.Variant["wood"]);
            }

            return base.Block.OnPickBlock(Api.World, Pos).GetName();
        }

        public override void GetBlockInfo(IPlayer forPlayer, StringBuilder dsc)
        {
            base.GetBlockInfo(forPlayer, dsc);
            double num = (totalHoursTillGrowth - Api.World.Calendar.TotalHours) / (double)Api.World.Calendar.HoursPerDay;

            if (canGrowEver)
            {
                if (stage == EnumTreeGrowthStage.Seed)
                {
                    if (num <= 1.0)
                    {
                        dsc.AppendLine(Lang.Get("Will sprout in less than a day"));
                        return;
                    }

                    dsc.AppendLine(Lang.Get("Will sprout in about {0} days", (int)num));
                }
                else if (num <= 1.0)
                {
                    dsc.AppendLine(Lang.Get("Will mature in less than a day"));
                }
                else
                {
                    dsc.AppendLine(Lang.Get("Will mature in about {0} days", (int)num));
                }
            }
            else
            {
                dsc.AppendLine(Lang.Get("This {0} will never grow here", stage));
            }

            #region blockinfoclimatetags
            if (climateFlags != null)
            {

                if (climateFlags[0] != 0)
                {
                    dsc.AppendLine(Lang.Get("Annual Tempurature is too {0} for this {1} to grow", (climateFlags[0] > 0) ? "Hot" : "Cold", stage));
                }
                if (climateFlags[1] != 0)
                {
                    dsc.AppendLine(Lang.Get("Annual Rainfall is too {0} for this {1} to grow", (climateFlags[1] > 0) ? "High" : "Low", stage));
                }
                if (climateFlags[2] != 0)
                {
                    dsc.AppendLine(Lang.Get("Annual Fertility is too {0} for this {1} to grow", (climateFlags[2] > 0) ? "High" : "Low", stage));
                }
                if (climateFlags[3] != 0)
                {
                    dsc.AppendLine(Lang.Get("Annual Forest Density is too {0} for this {1} to grow", (climateFlags[3] > 0) ? "High" : "Low", stage));
                }
                if (climateFlags[4] != 0)
                {
                    dsc.AppendLine(Lang.Get("This {0} will never grow at this altitude. It is too {1}!", stage, (climateFlags[4] > 0) ? "High" : "Low"));
                }
                if(TreeRestrictModSystem.clientConfig.DebugMode)
                {
                    dsc.AppendLine($"| Tempurature | {climateFlags[0],8:F6} | {climateFlags[0] * 60,8:F6} |");
                    dsc.AppendLine($"| Rainfall | {climateFlags[1],8:F6} | {climateFlags[1] * 255,8:F6} |");
                    dsc.AppendLine($"| Fertility | {climateFlags[2],8:F6} | {climateFlags[2] * 255,8:F6} |");
                    dsc.AppendLine($"| Forest Density | {climateFlags[3],8:F6} | {climateFlags[3] * 255,8:F6} |");
                    dsc.AppendLine($"| Height | {climateFlags[4],8:F6} | {climateFlags[4] * Api.World.BlockAccessor.MapSizeY,8:F6} |");
                    dsc.AppendLine(Lang.Get("\nTotal Growth Multiplier: " + climateFlags.Select(x => Math.Abs(x)).Sum() + 1));
                }
                

            }
            #endregion blockinfoclimatetags
        }

        public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tessThreadTesselator)
        {
            if (plantedFromSeed)
            {
                mesher.AddMeshData(dirtMoundMesh);
            }

            if (stage == EnumTreeGrowthStage.Seed)
            {
                return true;
            }

            return base.OnTesselation(mesher, tessThreadTesselator);
        }


        private void setClimateFlags()
        {
            Dictionary<string,SaplingClimateCondition> saplingClimateConds = ObjectCacheUtil.TryGet<Dictionary<string, SaplingClimateCondition>>(Api, "saplingClimateConditionCache");
            string text = Api.World.BlockAccessor.GetBlock(Pos).Attributes?["treeGen"].AsString();
            canGrowEver = true;

            if (saplingClimateConds == null || text == null)
            {
                if (text == null)
                {
                    Api.Logger.Error("unable to get treeGen Attribute for block entity at {0}. Removing Growth Listener", Pos);
                }
                else
                {
                    Api.Logger.Error("unable to get Sapling Climate Conditions for block entity at {0}. Removing Growth Listener", Pos);
                }

                canGrowEver = false;
                UnregisterGameTickListener(growListenerId);
                growListenerId = 0L;
                return;

            }

            climateFlags = new float[5];
            treeGens = new HashSet<string>();
            if (saplingClimateConds.TryGetValue(text, out SaplingClimateCondition cond))
            {
                treeGens.AddRange(cond.AssetLocations);

                if (modConfig.enableStuntedGrowthTempurature)
                {
                    
                    
                      climateFlags[0] = GetSignedRangeDeviation(nowClimate.Temperature + 20, cond.MinTemp + 20, cond.MaxTemp + 20) * 1/60f;
                }
                if (modConfig.enableStuntedGrowthRain)
                {
                    Api.World.Logger.Event(Api.World.BlockAccessor.GetClimateAt(Pos, EnumGetClimateMode.WorldGenValues).WorldgenRainfall.ToString());

                    climateFlags[1] = GetSignedRangeDeviation(nowClimate.Rainfall, cond.MinRain, cond.MaxRain);
                }
                if (modConfig.enableStuntedGrowthFertility)
                {
                    climateFlags[2] = GetSignedRangeDeviation(nowClimate.Fertility, cond.MinFert, cond.MaxFert);
                }
                if (modConfig.enableStuntedGrowthForest)
                {
                    climateFlags[3] = GetSignedRangeDeviation(nowClimate.ForestDensity, cond.MinForest, cond.MaxForest);
                }
                if (modConfig.enableStuntedGrowthHeight)
                {
                    float heightRel = ((float)Pos.Y  /(float)Api.World.BlockAccessor.MapSizeY);
                    climateFlags[4] = GetSignedRangeDeviation(heightRel, cond.MinHeight, cond.MaxHeight);

                    if (climateFlags[4] != 0)
                    {
                        canGrowEver = false;
                    }
                }
                GrowthRateModClimate = (1f + climateFlags.Select(x => Math.Abs(x)).Sum() * modConfig.climateGrowthMod);
                Api.Logger.Event("[{0}]", string.Join(",", climateFlags));
            }
        }
        
        public static float GetSignedRangeDeviation(float value, float min, float max)
        {
            return value - Math.Clamp(value, min, max);
        }
    }
}
