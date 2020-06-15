using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 平面斜率和半径
    /// </summary>
    public class PlaneFaceSlopeAndDia : AbstractFaceSlopeAndDia
    {
        public PlaneFaceSlopeAndDia(FaceData data) : base(data)
        {

        }
        public override void GetSlopeAndDia(Vector3d vec, out double[] slope, out double[] dia)
        {
            slope = new double[2];
            dia = new double[2];           
            slope[0] = slope[1] = UMathUtils.Angle(vec, Data.Dir);
            dia[0] = dia[1] = 0;
        }
    }
}
