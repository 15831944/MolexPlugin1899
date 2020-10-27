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
    /// 圆柱体斜率和半径
    /// </summary>
    public class CylinderFaceSlopeAndDia : AbstractFaceSlopeAndDia
    {
        public CylinderFaceSlopeAndDia(FaceData data) : base(data)
        {

        }
        public override void GetSlopeAndDia(Vector3d vec, out double[] slope, out double[] dia)
        {
            slope = new double[2];
            dia = new double[2];
            double anlge = UMathUtils.Angle(vec, Data.Dir);
            dia[0] = dia[1] = 2 * Data.Radius;
            if (UMathUtils.IsEqual(anlge, 0) || UMathUtils.IsEqual(anlge, Math.PI))
            {
                slope[0] = slope[1] = Math.PI / 2;
            }
            else
            {
                double[] rid;
                FaceUtils.GetSweptSlope(this.Data.Face, vec, out slope, out rid);
            }
        }
    }
}
