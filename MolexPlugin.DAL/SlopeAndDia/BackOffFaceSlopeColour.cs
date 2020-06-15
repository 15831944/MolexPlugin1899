using Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 倒扣面
    /// </summary>
    public class BackOffFaceSlopeColour : AbstractFaceSlopeColour
    {

        public BackOffFaceSlopeColour(AnalysisFaceSlopeAndDia analy) : base(analy)
        {

        }

        public override int FaceColour()
        {
            if (analy.MaxSlope > Math.Round(Math.PI / 2, 3))
            {

                return 186;
            }
            else
            {
                return base.NextColour();
            }
        }

        public override string GetFaceType()
        {
            if (analy.MaxSlope > Math.Round(Math.PI / 2, 3))
            {
                return "倒扣面";

            }
            else
            {
                return base.NextFacetype();
            }
        }
    }
}
