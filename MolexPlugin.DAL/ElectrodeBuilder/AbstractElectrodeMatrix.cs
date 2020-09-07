using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 电极表达式抽象类
    /// </summary>
    public abstract class AbstractElectrodeMatrix
    {
        protected Matrix4 workMatr;
        protected Matrix4 eleMatr = null;
        protected List<Body> eleHead;
        protected Point3d disPt;
        protected Point3d centerPt;
        /// <summary>
        /// 电极矩阵
        /// </summary>
        public Matrix4 EleMatr
        {
            get
            {
                if (eleMatr == null)
                    eleMatr = GetEleMatr();
                return eleMatr;
            }
        }
        public AbstractElectrodeMatrix()
        {


        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="workMatr"></param>
        /// <param name="eleHead"></param>
        /// <param name="workpiecePart"></param>
        public virtual void Initialinze(Matrix4 workMatr, List<Body> eleHead, Part workpiecePart = null)
        {
            this.workMatr = workMatr;
            this.eleHead = eleHead;
            GetHeadBounbingBox();
        }
        /// <summary>
        /// 获取电极矩阵
        /// </summary>
        /// <returns></returns>
        protected abstract Matrix4 GetEleMatr();
        /// <summary>
        /// 单齿设定值
        /// </summary>
        /// <returns></returns>
        public abstract Point3d GetSingleHeadSetValue();
        /// <summary>
        /// 获取备料
        /// </summary>
        /// <param name="pitch"></param>
        /// <param name="zDatum">是否有基准台</param>
        /// <returns></returns>
        public abstract double[] GetPreparation(ElectrodePitchInfo pitch, bool zDatum);
        /// <summary>
        /// 获取电极高度
        /// </summary>
        /// <param name="exp">拉伸</param>
        /// <returns></returns>
        public abstract double GetZHeight(double exp);
        /// <summary>
        /// 设置矩阵原点
        /// </summary>
        /// <param name="pre">备料</param>
        /// <param name="originPt"></param>
        public virtual void SetMatrixOrigin(int[] pre,Point3d originPt)
        {
            Matrix4 mat = new Matrix4();
            mat.Identity();
            mat.TransformToZAxis(originPt, this.EleMatr.GetXAxis(), this.EleMatr.GetYAxis());
            this.eleMatr = mat;
        }
        /// <summary>
        /// 获取电极头在Work坐标系下得外形点
        /// </summary>
        protected void GetHeadBounbingBox()
        {
            Matrix4 workInv = workMatr.GetInversMatrix();
            CartesianCoordinateSystem csys = BoundingBoxUtils.CreateCoordinateSystem(workMatr, workInv);
            BoundingBoxUtils.GetBoundingBoxInLocal(eleHead.ToArray(), csys, workMatr, ref centerPt, ref disPt);

        }

        /// <summary>
        /// 获取工件在Work坐标系下得外形点
        /// </summary>
        protected void GetWorkpieceBounbingBox(Part workpiecePart, out Point3d workpieceCenterPt, out Point3d workpieceDisPt)
        {
            workpieceDisPt = new Point3d();
            workpieceCenterPt = new Point3d();
            Matrix4 workInv = workMatr.GetInversMatrix();
            CartesianCoordinateSystem csys = BoundingBoxUtils.CreateCoordinateSystem(workMatr, workInv);
            BoundingBoxUtils.GetBoundingBoxInLocal(workpiecePart.Bodies.ToArray(), csys, workMatr, ref workpieceCenterPt, ref workpieceDisPt);

        }
        /// <summary>
        /// 分析电极是否有倒扣
        /// </summary>
        /// <returns></returns>
        public bool AnalyeBackOffFace()
        {
            bool isBack = true;
            foreach (Body by in eleHead)
            {
                AnalysisBodySlopeAndMinDia analysis = new AnalysisBodySlopeAndMinDia(this.EleMatr.GetZAxis(), by);
                isBack = analysis.AskBackOffFace();
            }
            return isBack;
        }

        /// <summary>
        /// 获取电极设定值
        /// </summary>
        /// <param name="pitch"></param>
        /// <param name="zDatum"></param>
        /// <returns></returns>
        public Point3d GetHeadSetValue(ElectrodePitchInfo pitch, bool zDatum)
        {
            Point3d temp = GetSingleHeadSetValue();
            double x1 = temp.X + (pitch.PitchXNum - 1) * pitch.PitchX / 2;
            double y1 = temp.Y + (pitch.PitchYNum - 1) * pitch.PitchY / 2;
            if (zDatum)
            {
                double[] pre = GetPreparation(pitch, zDatum);
                if (pre[0] >= pre[1])
                {
                    x1 = temp.X + (pitch.PitchXNum - 2) * pitch.PitchX / 2;
                }
                else
                {
                    y1 = temp.Y + (pitch.PitchYNum - 2) * pitch.PitchY / 2;
                }
            }
            return new Point3d(x1, y1, temp.Z);
        }


        /// <summary>
        /// 获取XY外形大小
        /// </summary>
        /// <returns></returns>
        public double[] GetHeadDis()
        {
            double[] dis = new double[2];
            dis[0] = Math.Round(this.disPt.X, 3);
            dis[1] = Math.Round(this.disPt.Y, 3);
            return dis;
        }
    }
}
