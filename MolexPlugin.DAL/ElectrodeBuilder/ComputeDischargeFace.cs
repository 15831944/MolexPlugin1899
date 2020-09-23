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
    /// 计算放电面积
    /// </summary>
    public class ComputeDischargeFace
    {
        private Matrix4 matr;
        private CartesianCoordinateSystem csys;
        private Body eleBody;
        private Body toolBody;

        public ComputeDischargeFace(Body eleBody, Body toolBody, Matrix4 matr, CartesianCoordinateSystem csys)
        {
            this.eleBody = eleBody;
            this.toolBody = toolBody;
            this.matr = matr;
            this.csys = csys;
        }
        /// <summary>
        ///检查体
        /// </summary>
        /// <returns></returns>
        public BodyInfo GetBodyInfoForInterference(bool extract)
        {
            List<Face> dischargeFace = new List<Face>();
            List<Face> temp = new List<Face>();
            List<Body> bodys = new List<Body>();
            try
            {
                AnalysisUtils.SetInterferenceOutFace(this.eleBody, this.toolBody, out temp, out bodys);
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("干涉检查错误！" + ex.Message);
            }
            if (bodys.Count > 0)
                LayerUtils.MoveDisplayableObject(252, bodys.ToArray());
            dischargeFace = temp.Where(a => a.GetBody().Equals((this.eleBody))).Distinct().ToList(); //过滤电极面
            List<Face> tt = temp.Where(a => a.GetBody().Equals((this.toolBody))).Distinct().ToList();
            List<Face> faces = new List<Face>();
            foreach (Face fe in dischargeFace)
            {
                FaceData data1 = FaceUtils.AskFaceData(fe);
                foreach (Face fa in tt)
                {
                    FaceData fd = FaceUtils.AskFaceData(fa);
                    if (UMathUtils.SelfDis(data1.Dir) == 0)
                    {
                        if (data1.FaceType == fd.FaceType && data1.IntNorm == -fd.IntNorm)
                        {
                            faces.Add(fe);
                            break;
                        }
                    }
                    else
                    {
                        double anlge = UMathUtils.Angle(data1.Dir, fd.Dir);
                        if (data1.FaceType == fd.FaceType && data1.IntNorm == -fd.IntNorm && UMathUtils.IsEqual(anlge, Math.PI))
                        {
                            faces.Add(fe);
                            break;
                        }
                    }
                }
            }
            if (extract)
                ExtractFace(this.toolBody, faces.ToArray());
            BodyInfo info = new BodyInfo(eleBody, faces);
            info.SetAttribute(csys, matr);
            return info;
        }
        private void ExtractFace(Body toolBody, params Face[] face)
        {
            List<Body> bodys = new List<Body>();
            try
            {
                bodys.AddRange(ExtraetUtils.ExtractFaceBuilder(face));
            }
            catch
            {
                foreach (Face fe in face)
                {
                    bodys.Add(ExtraetUtils.ExtraetFace(fe));
                }
            }
            foreach (Body by in bodys)
            {
                try
                {
                    by.Color = 6;
                    by.Layer = 251;
                    DeleteObject.DeleteParms(BooleanUtils.CreateBooleanFeature(by, false, true, NXOpen.Features.Feature.BooleanType.Intersect, toolBody).GetBodies());
                }
                catch
                {

                }
            }
        }
    }
}
