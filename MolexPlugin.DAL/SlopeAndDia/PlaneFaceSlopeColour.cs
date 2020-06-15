using Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 0度平面
    /// </summary>
    public class PlaneFaceSlopeColour : AbstractFaceSlopeColour
    {

        public PlaneFaceSlopeColour(AnalysisFaceSlopeAndDia analy) : base(analy)
        {

        }

        public override int FaceColour()
        {
            if (UMathUtils.IsEqual(analy.MaxSlope, 0) && UMathUtils.IsEqual(analy.MaxSlope, analy.MinSlope))
            {

                return 25;
            }
            else
            {
                return base.NextColour();
            }
        }
        public override string GetFaceType()
        {
            if (UMathUtils.IsEqual(analy.MaxSlope, 0) && UMathUtils.IsEqual(analy.MaxSlope, analy.MinSlope))
            {
                return "平面";

            }
            else
            {
                return base.NextFacetype();
            }
        }
    }
}
