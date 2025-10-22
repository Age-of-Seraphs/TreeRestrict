using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeRestrict.src.systems;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using TreeRestrict;
using Vintagestory.API.Util;
using Vintagestory.ServerMods.NoObf;

namespace TreeRestrict.src.behaviors
{
    internal class BlockEntityBehaviorSaplingClimate : BlockEntityBehavior
    {

        public BlockEntityBehaviorSaplingClimate(BlockEntity blockEntity)
        : base(blockEntity)
        {
        }
        public EnumTreeGrowthStage stage;
        
        protected int saplingStunts;

        internal string[] stuntedReasons;
        public override void GetBlockInfo(IPlayer forPlayer, StringBuilder dsc)
        {
            base.GetBlockInfo(forPlayer, dsc);
            dsc.AppendLine("growth Stunted "+saplingStunts);
        }
        public void setSaplingStunts(int stunts)
        {
            saplingStunts = stunts;
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);
            tree.SetInt("saplingStunt", saplingStunts);
        }
        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldAccessForResolve)
        {
            base.FromTreeAttributes(tree, worldAccessForResolve);
            saplingStunts = tree.GetInt("saplingStunt");
        }
    }
}
