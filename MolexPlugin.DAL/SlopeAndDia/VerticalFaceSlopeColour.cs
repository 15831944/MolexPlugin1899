using Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 90度面
    /// </summary>
    public class VerticalFaceSlopeColour : AbstractFaceSlopeColour
    {

        public VerticalFaceSlopeColour(AnalysisFaceSlopeAndDia analy) : base(analy)
        {

        }

        public override int FaceColour()
        {
            if (UMathUtils.IsEqual(analy.MaxSlope, Math.PI / 2) && UMathUtils.IsEqual(analy.MaxSlope, analy.MinSlope))
            {
                return 211;
            }
            else
            {
                return base.NextColour();
            }
        }

        public override string GetFaceType()
        {
            if (UMathUtils.IsEqual(analy.MaxSlope, Math.PI / 2) && UMathUtils.IsEqual(analy.MaxSlope, analy.MinSlope))
            {
                return "垂直面";

            }
            else
            {
                return base.NextFacetype();
            }
        }
    }
}
