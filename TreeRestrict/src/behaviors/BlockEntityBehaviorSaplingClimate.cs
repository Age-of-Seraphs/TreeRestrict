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
        
        internal bool growthStunted;
        public override void GetBlockInfo(IPlayer forPlayer, StringBuilder dsc)
        {
            base.GetBlockInfo(forPlayer, dsc);
            dsc.AppendLine("growth Stunted "+growthStunted);
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);
            tree.SetBool("growthStunted", growthStunted);
        }
        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldAccessForResolve)
        {
            base.FromTreeAttributes(tree, worldAccessForResolve);
            growthStunted = tree.GetBool("growthStunted");
        }
    }
}
