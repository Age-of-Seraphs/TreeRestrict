using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeRestrict.src.systems
{
    [Flags]
    internal enum EnumSaplingGrowthFlags
    {
        Tempurature = 1,

        Rainfall = 2,

        Fertility = 4,

        ForestDensity = 8,

        Height = 0x10,

        Custom = 32
    }
}
