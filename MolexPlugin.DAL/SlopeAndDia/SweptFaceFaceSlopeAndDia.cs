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
    /// 自由曲面斜率和半径
    /// </summary>
    public class SweptFaceFaceSlopeAndDia : AbstractFaceSlopeAndDia
    {
        public SweptFaceFaceSlopeAndDia(FaceData data) : base(data)
        {

        }
        public override void GetSlopeAndDia(Vector3d vec, out double[] slope, out double[] dia)
        {
            slope = new double[2];
            dia = new double[2]; 
            double[] rid;
            FaceUtils.GetSweptSlope(this.Data.Face, vec, out slope, out rid);
            dia[0] = 2 * rid[0];
            dia[1] = 2 * rid[1];
        }
    }
}
