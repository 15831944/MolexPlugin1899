using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;
using NXOpen.UF;

namespace MolexPlugin.Model
{
    public class WorkpieceDrawingModel
    {
        private Part part;
        private Matrix4 workMatr;
        private Point maxPt = null;
        private Point minPt = null;
        public Part WorkpiecePart { get { return part; } }
        /// <summary>
        /// 外形点
        /// </summary>
        public Point3d DisPt { get; private set; }
        /// <summary>
        /// 中心点
        /// </summary>
        public Point3d CenterPt { get; private set; }
        /// <summary>
        /// 最大点
        /// </summary>
        public Point MaxPt
        {
            get
            {
                if (maxPt == null)
                    CreateMinAndMaxPt();
                return maxPt;
            }
        }
        /// <summary>
        /// 最小点
        /// </summary>
        public Point MinPt
        {
            get
            {
                if (maxPt == null)
                    CreateMinAndMaxPt();
                return minPt;
            }
        }
        public WorkpieceDrawingModel(Part part, Matrix4 workMatr)
        {
            this.part = part;
            this.workMatr = workMatr;
            GetBoundingBox();
        }

        private void GetBoundingBox()
        {
            Point3d centerPt = new Point3d();
            Point3d disPt = new Point3d();
            Part workPart = Session.GetSession().Parts.Work;
            List<Body> bodys = new List<Body>();
            //  NXOpen.Assemblies.Component ct = AssmbliesUtils.GetPartComp(workPart, part)[0];
            foreach (NXOpen.Assemblies.Component ct in AssmbliesUtils.GetPartComp(workPart, part))
            {
                if (!ct.IsSuppressed)
                {
                    foreach (Body body in this.part.Bodies.ToArray())
                    {
                        bodys.Add(AssmbliesUtils.GetNXObjectOfOcc(ct.Tag, body.Tag) as Body);
                    }
                }
            }
            Matrix4 invers = workMatr.GetInversMatrix();
            CartesianCoordinateSystem csys = BoundingBoxUtils.CreateCoordinateSystem(workMatr, invers);//坐标
            BoundingBoxUtils.GetBoundingBoxInLocal(bodys.ToArray(), csys, workMatr, ref centerPt, ref disPt);
            LayerUtils.MoveDisplayableObject(201, bodys.ToArray());
            this.CenterPt = centerPt;
            this.DisPt = disPt;
        }
        /// <summary> 
        /// 创建外形点
        /// </summary>
        /// <returns></returns>
        private void CreateMinAndMaxPt()
        {
            Matrix4 invers = this.workMatr.GetInversMatrix();
            UFSession theUFSession = UFSession.GetUFSession();
            Point3d minPt = new Point3d(this.CenterPt.X - this.DisPt.X, this.CenterPt.Y - this.DisPt.Y, this.CenterPt.Z - this.DisPt.Z);
            Point3d maxPt = new Point3d(this.CenterPt.X + this.DisPt.X, this.CenterPt.Y + this.DisPt.Y, this.CenterPt.Z + this.DisPt.Z);
            invers.ApplyPos(ref maxPt);
            invers.ApplyPos(ref minPt);
            this.minPt = PointUtils.CreatePoint(minPt);
            theUFSession.Obj.SetLayer(this.minPt.Tag, 254);
            this.maxPt = PointUtils.CreatePoint(maxPt);
            theUFSession.Obj.SetLayer(this.maxPt.Tag, 254);
        }
        /// <summary>
        /// 获取隐藏工件
        /// </summary>
        /// <param name="workpieceCt"></param>
        /// <returns></returns>
        public List<NXOpen.Assemblies.Component> GetHiddenObjects(List<NXOpen.Assemblies.Component> workpieceCt)
        {
            List<NXOpen.Assemblies.Component> hidden = new List<NXOpen.Assemblies.Component>();
            foreach (NXOpen.Assemblies.Component ct in workpieceCt)
            {
                if (!ct.Name.Equals(part.Name, StringComparison.CurrentCultureIgnoreCase))
                    hidden.Add(ct);
            }
            return hidden;
        }
        /// <summary>
        /// 获取视图比例
        /// </summary>
        /// <param name="xMax"></param>
        /// <param name="yMax"></param>
        /// <returns></returns>
        public double GetScale(double xMax, double yMax)
        {
            double x = xMax / (this.DisPt.X * 2);
            double y = yMax / (this.DisPt.Y * 2 + this.DisPt.Z * 2);
            if (x > y)
            {
                if (y > 1)
                    return Math.Floor(y);
                else
                    return Math.Round(y, 1);
            }
            else
            {
                if (x > 1)
                    return Math.Floor(x);
                else
                    return Math.Round(x, 1);
            }
        }



    }
}
