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
    ///  圆锥斜率和半径
    /// </summary>
    public class CircularConeFaceSlopeAndDia : AbstractFaceSlopeAndDia
    {
        public CircularConeFaceSlopeAndDia(FaceData data) : base(data)
        {

        }
        public override void GetSlopeAndDia(Vector3d vec, out double[] slope, out double[] dia)
        {
            slope = new double[2] { 99999, -99999 };
            dia = new double[2] { 99999, -99999 };        
            double anlge = UMathUtils.Angle(vec, Data.Dir);
            if (UMathUtils.IsEqual(anlge, 0) || UMathUtils.IsEqual(anlge, Math.PI))
            {
                slope[0] = slope[1] = UMathUtils.Angle(vec, FaceUtils.AskFaceNormal(this.Data.Face));
                foreach (Edge edge in this.Data.Face.GetEdges())
                {
                    if (edge.SolidEdgeType == Edge.EdgeType.Circular)
                    {
                        double radius = EdgeUtils.GetArcRadius(edge);

                        if (2 * radius > dia[1])
                            dia[1] = 2 * radius;
                        if (2 * radius < dia[0])
                            dia[0] = 2 * radius;
                    }
                }
                if (dia[0] == 99999)
                    dia[0] = 0;
            }
            else
            {
                double[] rid;
                FaceUtils.GetSweptSlope(this.Data.Face, vec, out slope, out rid);
                dia[0] = 2 * rid[0];
                dia[1] = 2 * rid[1];
            }
        }
    }
}
