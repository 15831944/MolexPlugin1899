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
    /// 分析模架
    /// </summary>
    public class AnalysisMold
    {
        private Body aBody;
        private Body bBody;
        private Matrix4 matr;
        private Part workPart;
        /// <summary>
        /// 上模
        /// </summary>
        public List<MoldBaseModel> UpModel { get; private set; } = new List<MoldBaseModel>();
        /// <summary>
        /// 下模
        /// </summary>
        public List<MoldBaseModel> DownModel { get; private set; } = new List<MoldBaseModel>();

        public AnalysisMold(Body aBody, Body bBody)
        {
            this.aBody = aBody;
            this.bBody = bBody;
            workPart = Session.GetSession().Parts.Work;
            this.matr = GetMatr();
            MoldBaseModel aMold = new MoldBaseModel(aBody, matr);
            aMold.Name = "A板";
            UpModel.Add(aMold);
            MoldBaseModel bMold = new MoldBaseModel(bBody, matr);
            bMold.Name = "B板";
        }

        private Matrix4 GetMatr()
        {
            CoordinateSystem wcs = workPart.WCS.CoordinateSystem;
            Matrix4 mat = new Matrix4();
            mat.Identity();
            mat.TransformToCsys(wcs, ref matr);
            Matrix4 inv = mat.GetInversMatrix();
            MoldBaseModel aMold = new MoldBaseModel(aBody, mat);
            MoldBaseModel bMold = new MoldBaseModel(bBody, mat);
            Vector3d vec = UMathUtils.GetVector(bMold.CenterPt, aMold.CenterPt);
            Point3d center = UMathUtils.GetMiddle(bMold.CenterPt, aMold.CenterPt);
            inv.ApplyPos(ref center);
            mat.TransformToZAxis(center, vec);
            return mat;
        }
        /// <summary>
        /// 获取模板
        /// </summary>
        /// <returns></returns>
        public void GetBase(out List<MoldBaseModel> moldBase, out List<AbstractCylinderBody> cylinder)
        {
            moldBase = new List<MoldBaseModel>();
            cylinder = new List<AbstractCylinderBody>();
            foreach (Body by in workPart.Bodies)
            {
                MoldBaseModel mm = new MoldBaseModel(by, this.matr);
                if (UMathUtils.IsEqual(mm.CenterPt.X, 0) || UMathUtils.IsEqual(mm.CenterPt.Y, 0))
                {
                    moldBase.Add(mm);
                }
                else
                {
                    BodyCircleFeater bf = new BodyCircleFeater(by);
                    bf.
                }
            }
        }

    }
}
