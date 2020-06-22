using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;
using NXOpen.Utilities;
using Basic;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 外形点
    /// </summary>
    public class NXObjectAooearancePoint
    {
        private List<NXObject> objs;
        private Part workPart;
        private UserSingleton user;
        /// <summary>
        /// 外形尺寸
        /// </summary>
        public Point3d DisPt { get; private set; }
        /// <summary>
        /// 中心点
        /// </summary>
        public Point3d CenterPt { get; private set; }

        public NXObjectAooearancePoint(params NXObject[] objs)
        {
            this.objs = objs.ToList();
            this.workPart = Session.GetSession().Parts.Work;
            user = UserSingleton.Instance();
            GetBoundingBox();
        }
        /// <summary>
        /// 获取最大外形
        /// </summary>
        private void GetBoundingBox()
        {
            if (!user.Jurisd.GetComm())
                return;
            CoordinateSystem wcs = workPart.WCS.CoordinateSystem;
            Matrix4 mat = new Matrix4();
            mat.Identity();
            mat.TransformToCsys(wcs, ref mat);
            Point3d centerPt = new Point3d();
            Point3d disPt = new Point3d();
            BoundingBoxUtils.GetBoundingBoxInLocal(objs.ToArray(), null, mat, ref centerPt, ref disPt);
            this.CenterPt = centerPt;
            this.DisPt = disPt;
        }
        /// <summary>
        /// 创建外形点
        /// </summary>
        /// <param name="centerPt"></param>
        /// <param name="disPt"></param>
        /// <returns></returns>
        public List<NXObject> CreatePoint()
        {
            UFSession theUFSession = UFSession.GetUFSession();
            if (UMathUtils.IsEqual(this.DisPt.X, 0) && UMathUtils.IsEqual(this.DisPt.Y, 0) && UMathUtils.IsEqual(this.DisPt.Z, 0))
                return null;
            double[] x = { CenterPt.X - DisPt.X, CenterPt.X, CenterPt.X + DisPt.X };
            double[] y = { CenterPt.Y - DisPt.Y, CenterPt.Y, CenterPt.Y + DisPt.Y };
            double[] z = { CenterPt.Z - DisPt.Z, CenterPt.Z, CenterPt.Z + DisPt.Z };
            Matrix4 mat = new Matrix4();
            List<NXObject> points = new List<NXObject>();
            mat.Identity();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        Point3d temp = new Point3d(x[i], y[j], z[k]);
                        mat.ApplyPos(ref temp);
                        Tag pointTag = Tag.Null;
                        theUFSession.Curve.CreatePoint(new double[] { temp.X, temp.Y, temp.Z }, out pointTag);
                        theUFSession.Obj.SetColor(pointTag, 186);
                        points.Add(NXObjectManager.Get(pointTag) as NXObject);
                    }
                }
            }
            return points;
        }
        /// <summary>
        /// 获取点到面的投影点
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="face"></param>
        /// <returns></returns>
        public Point3d GetPointToFaceDis(Point pt, Face face)
        {
            FaceData faceData = FaceUtils.AskFaceData(face);
            Matrix4 mat = new Matrix4();
            mat.Identity();
            mat.TransformToZAxis(faceData.Point, faceData.Dir);
            Point3d temp = pt.Coordinates;
            mat.ApplyPos(ref temp);
            Matrix4 invers = mat.GetInversMatrix();
            temp.Z = 0;
            invers.ApplyPos(ref temp);
            return temp;
        }
    }
}
