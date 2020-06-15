using Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 斜度面
    /// </summary>
    public class SlantFaceSlopeColour : AbstractFaceSlopeColour
    {

        public SlantFaceSlopeColour(AnalysisFaceSlopeAndDia analy) : base(analy)
        {

        }

        public override int FaceColour()
        {
            if ((analy.MaxSlope < Math.Round(Math.PI / 2, 3) && analy.MaxSlope > 0))
            {
                return 36;
            }
            else
            {
                return base.NextColour();
            }
        }

        public override string GetFaceType()
        {

            if ((analy.MaxSlope < Math.Round(Math.PI / 2, 3) && analy.MaxSlope > 0))
            {
                return "斜度面";

            }
            else
            {
                return base.NextFacetype();
            }
        }
    }
}
