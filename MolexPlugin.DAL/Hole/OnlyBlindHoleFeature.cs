using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.BlockStyler;
using Basic;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 单一盲孔
    /// </summary>
    public  class OnlyBlindHoleFeature : AbstractHoleFeater
    {

        public OnlyBlindHoleFeature(List<CylinderFeater> cylFeat) : base(cylFeat)
        {
            this.Type = HoleType.OnlyBlindHole;
            
        }

        
    }
}
