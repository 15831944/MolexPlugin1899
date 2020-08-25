using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.Features;
using NXOpen.UF;
using Basic;
using NXOpen.BlockStyler;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 方块盒抽象类
    /// </summary>
    public abstract class AbstractSuperBox
    {
        protected Matrix4 matr = null;
        /// <summary>
        /// 方块盒特征
        /// </summary>
        public ToolingBox ToolingBoxFeature { get; protected set; } = null;

        protected List<NXObject> objs = new List<NXObject>();
        protected double[] Offset;
        /// <summary>
        /// 中心点
        /// </summary>
        public Point3d CenterPt { get; protected set; }

        public Point3d DisPt { get; protected set; }

        public Matrix4 Matr
        {
            get
            {
                if (matr == null)
                    return GetMatrix();
                else
                    return matr;
            }
        }
        public AbstractSuperBox(List<NXObject> objs, double[] offset)
        {
            this.objs = objs;
            this.Offset = offset;
        }
        /// <summary>
        /// 删除特征
        /// </summary>
        public bool DeleToolingBoxFeatures()
        {
            try
            {
                DeleteObject.Delete(ToolingBoxFeature);
                this.ToolingBoxFeature = null;
                return true;
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile(ex.Message);
                return false;
            }

        }
        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(int color)
        {

            UFSession theUFSession = UFSession.GetUFSession();
            if (this.ToolingBoxFeature != null)
                theUFSession.Obj.SetColor(this.ToolingBoxFeature.GetBodies()[0].Tag, color);
        }
        /// <summary>
        /// 设置透明度
        /// </summary>
        /// <param name="tl"></param>
        public void SetTranslucency(int tl)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            if (this.ToolingBoxFeature != null)
                theUFSession.Obj.SetTranslucency(this.ToolingBoxFeature.GetBodies()[0].Tag, tl);
        }
        /// <summary>
        /// 设置层
        /// </summary>
        /// <param name="layer"></param>
        public void SetLayer(int layer)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            if (this.ToolingBoxFeature != null)
                theUFSession.Obj.SetLayer(this.ToolingBoxFeature.GetBodies()[0].Tag, layer);
        }

        /// <summary>
        /// 获取中心点
        /// </summary>
        protected void GetBoundingBox()
        {
            CoordinateSystem wcs = Session.GetSession().Parts.Work.WCS.CoordinateSystem;
            Matrix4 mat = new Matrix4();
            mat.Identity();
            mat.TransformToCsys(wcs, ref mat);
            Point3d centerPt = new Point3d();
            Point3d disPt = new Point3d();
            BoundingBoxUtils.GetBoundingBoxInLocal(this.objs.ToArray(), null, mat, ref centerPt, ref disPt);
            this.CenterPt = centerPt;
            this.DisPt = disPt;
        }
        /// <summary>
        /// 获取矩阵
        /// </summary>
        /// <returns></returns>
        protected Matrix4 GetMatrix()
        {
            Vector3d xDir;
            Vector3d yDir;
            Session.GetSession().Parts.Work.WCS.CoordinateSystem.GetDirections(out xDir, out yDir);
            Matrix4 mat = new Matrix4();
            mat.Identity();
            mat.TransformToZAxis(this.CenterPt, xDir, yDir);
            return mat;
        }

        /// <summary>
        /// 创建特征
        /// </summary>
        public abstract void CreateSuperBox();

        /// <summary>
        /// 更新方位
        /// </summary>
        /// <param name="spec"></param>
        public abstract void UpdateSpecify(UIBlock ui);

        /// <summary>
        /// 关联线性向量到体面上
        /// </summary>
        /// <param name="ld">控件</param
        /// <param name="vec">向量</param>
        public abstract void SetDimForFace(ref NXOpen.BlockStyler.LinearDimension ld, Vector3d vec);
        /// <summary>
        /// 矩阵和偏置体更新方块盒
        /// </summary>
        /// <param name="matr"></param>
        /// <param name="offset"></param>
        public abstract void Update(Matrix4 matr, double[] offset);
        /// <summary>
        /// 选择更新方块盒
        /// </summary>
        /// <param name="nxobjects"></param>
        public void Update(List<NXObject> nxobjects)
        {
            if (nxobjects == null || nxobjects.Count == 0)
            {
                DeleteObject.Delete(this.ToolingBoxFeature);
                this.ToolingBoxFeature = null;
                return;
            }
            this.objs = nxobjects;
            GetBoundingBox();
            matr = GetMatrix();
            CreateSuperBox();
        }

    }
}
