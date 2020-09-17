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
    /// 分析面
    /// </summary>
    public class AnalysisFaceSlopeAndDia : IComparable<AnalysisFaceSlopeAndDia>
    {
        public Face Face { get; private set; }
        private UserSingleton user;
        private Vector3d vec;
        /// <summary>
        /// 最小半径
        /// </summary>
        public double MinDia { get; private set; } = 0;
        /// <summary>
        /// 最大半径
        /// </summary>
        public double MaxDia { get; private set; } = 0;
        /// <summary>
        /// 最小斜度
        /// </summary>
        public double MinSlope { get; private set; } = 0;
        /// <summary>
        /// 最大斜度
        /// </summary>
        public double MaxSlope { get; private set; } = 0;
        /// <summary>
        /// 面数据
        /// </summary>
        public FaceData Data { get; private set; }
        public AnalysisFaceSlopeAndDia(Face face, Vector3d vec)
        {
            this.Face = face;
            this.vec = vec;
            double[] dia = new double[2];
            double[] slope = new double[2];
            user = UserSingleton.Instance();
            if (user.UserSucceed && user.Jurisd.GetComm())
            {
                AbstractFaceSlopeAndDia absface = FaceSlopeAndDiaFactory.CreateFaceSlopeAndDia(face);
                this.Data = absface.Data;
                absface.GetSlopeAndDia(vec, out slope, out dia);
                this.MaxSlope = slope[1];
                this.MinSlope = slope[0];
                this.MinDia = dia[0];
                this.MaxDia = dia[1];
            }
            else
            {
                this.Data = FaceUtils.AskFaceData(face);
                return;
            }
        }
        /// <summary>
        /// 设置颜色
        /// </summary>
        public void SetColour()
        {
            AbstractFaceSlopeColour colour = FaceSlopeColourBuilder.Build(this);
            int cr = colour.FaceColour();
            if (cr != 0)
                this.Face.Color = cr;
        }
        /// <summary>
        /// 判断是否是倒扣面
        /// </summary>
        /// <returns></returns>
        public bool isBackOffFace()
        {
            if (Math.Round(this.MaxSlope, 3) > Math.Round(Math.PI / 2, 3))
            {
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// 以vec向量为Z向高低排序
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(AnalysisFaceSlopeAndDia other)
        {
            Part workPart = Session.GetSession().Parts.Work;
            CoordinateSystem wcs = workPart.WCS.CoordinateSystem;
            Matrix4 mat = new Matrix4();
            mat.Identity();
            mat.TransformToZAxis(wcs.Origin, this.vec);
            Point3d centerPt1 = UMathUtils.GetMiddle(this.Data.BoxMinCorner, this.Data.BoxMaxCorner);
            Point3d centerPt2 = UMathUtils.GetMiddle(other.Data.BoxMinCorner, other.Data.BoxMaxCorner);
            mat.ApplyPos(ref centerPt1);
            mat.ApplyPos(ref centerPt2);
            return centerPt1.Z.CompareTo(centerPt2.Z);
        }
    }
}
