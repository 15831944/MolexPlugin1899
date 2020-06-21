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
    /// 圆柱面
    /// </summary>
    public class CircularConeFace : AbstractCircleFace
    {
        /// <summary>
        /// 起点
        /// </summary>
        public Point3d StartPt { get; protected set; }
        /// <summary>
        /// 终点
        /// </summary>
        public Point3d EndPt { get; protected set; }
        /// <summary>
        /// 最小半径
        /// </summary>
        public double MinRadius
        {
            get
            {
                return this.Data.RadData;
            }
        }
        /// <summary>
        /// 最大半径
        /// </summary>
        public double MaxRadius { get { return this.Data.Radius; } }

        public double Length { get; private set; }
        public CircularConeFace(FaceData data) : base(data)
        {
            SetCircularConePoint();
        }
        /// <summary>
        /// 设置属性点
        /// </summary>
        private void SetCircularConePoint()
        {
            Point3d centerPt = new Point3d();
            Point3d disPt = new Point3d();
            Point3d start = new Point3d();
            Point3d end = new Point3d();
            Matrix4 inve = this.Matr.GetInversMatrix();
            this.GetFaceBoundingBox(out centerPt, out disPt);
            start.Z = centerPt.Z - disPt.Z;
            end.Z = centerPt.Z + disPt.Z;
            inve.ApplyPos(ref centerPt);
            inve.ApplyPos(ref start);
            inve.ApplyPos(ref end);
            this.CenterPt = centerPt;
            this.StartPt = start;
            this.EndPt = end;
            this.Length = disPt.Z * 2;
        }
    }
}
