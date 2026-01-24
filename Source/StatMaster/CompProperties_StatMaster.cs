using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace StatMaster
{
    public class CompProperties_StatMaster : CompProperties
    {
        public CompProperties_StatMaster()
        {
            this.compClass = typeof(CompStatMaster);
        }
    }
}
