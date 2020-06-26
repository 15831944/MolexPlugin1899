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
    public class OnlyBlindHoleFeature : AbstractHoleFeater
    {

        public OnlyBlindHoleFeature(HoleBuilder builder) : base(builder)
        {
            this.Type = HoleType.OnlyBlindHole;

        }

        public override ArcEdgeData GetTopEdge()
        {
            string err = "";
            List<ArcEdgeData> arcs = new List<ArcEdgeData>();
            foreach (Edge eg in this.Builder.CylFeater[0].Cylinder.Data.Face.GetEdges())
            {
                if (eg.SolidEdgeType == Edge.EdgeType.Circular)
                {
                    ArcEdgeData data = EdgeUtils.GetArcData(eg, ref err);
                    arcs.Add(data);
                }
            }
            arcs.Sort(delegate (ArcEdgeData a, ArcEdgeData b)
            {
                Matrix4 mat = this.Builder.CylFeater[0].Cylinder.Matr;
                Point3d centerPt1 = a.Center;
                Point3d centerPt2 = b.Center;
                mat.ApplyPos(ref centerPt1);
                mat.ApplyPos(ref centerPt2);
                return centerPt2.Z.CompareTo(centerPt1.Z);
            });
            return arcs[0];
        }

        protected override void GetDirection()
        {
            Vector3d dir = this.Builder.CylFeater[0].Direction;
            Point3d cylCenterPt = this.Builder.CylFeater[0].Cylinder.CenterPt;
            Body body = this.Builder.CylFeater[0].Cylinder.Data.Face.GetBody();
            int k = TraceARay.AskTraceARay(body, cylCenterPt, dir);
            if (k != 0)
            {
                this.Direction = dir;
            }
            else
            {
                this.Direction = new Vector3d(-dir.X, -dir.Y, -dir.Z);
                this.Builder.SetDirection(this.Direction);
            }
        }
        protected override void GetStartAndEndPt()
        {
            this.StratPt = this.Builder.CylFeater[0].StartPt;
            this.EndPt = this.Builder.CylFeater[0].Cylinder.EndPt;
        }
    }
}
