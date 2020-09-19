using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;
using NXOpen.Utilities;
using NXOpen.Assemblies;
using Basic;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 电极检查
    /// </summary>
    public class InterferenceElectrodeBuilder
    {
        private WorkModel work;
        private Part hostPart;
        private Component workComp;
        public InterferenceElectrodeBuilder(Component workComp, WorkModel work)
        {
            this.work = work;
            this.hostPart = work.GetHostWorkpiece();
            this.workComp = workComp;
        }

        private List<Component> GetEleAllComp()
        {
            List<Component> eleComps = new List<Component>();
            foreach (Component ct in workComp.GetChildren())
            {
                string partType = AttributeUtils.GetAttrForString(ct, "PartType");
                if (partType.Equals("Electrode", StringComparison.CurrentCultureIgnoreCase))
                    eleComps.Add(ct);
            }
            return eleComps;
        }
        /// <summary>
        /// 检查放电面积
        /// </summary>
        /// <param name="csys"></param>
        /// <param name="eleCt"></param>
        private void AskSetVaule(CartesianCoordinateSystem csys, ElectrodeSetValueInfo setValue, Component eleCt, ref List<string> err)
        {
            Part elePart = eleCt.Prototype as Part;
            Point3d setPoint = new Point3d(setValue.EleSetValue[0], setValue.EleSetValue[1], setValue.EleSetValue[2]);
            Point pt = GetSetPoint(elePart);
            if (pt == null)
                err.Add(elePart.Name + "-" + setValue.Positioning + "               无法找到设定点！");
            else if (!UMathUtils.IsEqual(setPoint, GetSetWorkPoint3d(pt, eleCt)))
                err.Add(elePart.Name + "-" + setValue.Positioning + "                   设定值错误！");
            Body eleBody = GetOccsInBods(eleCt);
            BodyInfo info = GetDischargeFace(csys, eleBody);
            if (info != null)
            {
                double newArea = info.GetProjectedArea(csys, work.Info.Matr);
                if (setValue.ProjectedArea >= 2 * newArea)
                {
                    setValue.ProjectedArea = newArea;
                    setValue.ContactArea = info.ContactArea;
                    setValue.SetAttribute(eleCt);
                }

            }

        }
        /// <summary>
        /// 获取电极中设定点
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        private Point GetSetPoint(Part part)
        {
            Tag pointTag = Tag.Null;
            foreach (Point k in part.Points.ToArray())
            {
                if (k.Name.ToUpper().Equals(("SetValuePoint").ToUpper()))
                    return k;
            }
            UFSession theUFSession = UFSession.GetUFSession();
            theUFSession.Obj.CycleByName("SetValuePoint".ToUpper(), ref pointTag);
            if (pointTag != Tag.Null)
                return NXObjectManager.Get(pointTag) as Point;
            return null;
        }
        /// <summary>
        /// 转换点
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        private Point3d GetSetWorkPoint3d(Point pt, Component eleCt)
        {
            Point compCt = AssmbliesUtils.GetNXObjectOfOcc(eleCt.Tag, pt.Tag) as Point;
            Point3d temp = compCt.Coordinates;
            work.Info.Matr.ApplyPos(ref temp);
            return temp;
        }
        /// <summary>
        /// 分析放电面积
        /// </summary>
        /// <param name="csys"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        private BodyInfo GetDischargeFace(CartesianCoordinateSystem csys, Body eleCtBody)
        {

            Component hostComp = GetPartInOcc(hostPart);
            if (hostComp != null)
            {
                Body workBody = GetOccsInBods(hostComp);
                if (workBody != null)
                {
                    ComputeDischargeFace cp = new ComputeDischargeFace(eleCtBody, workBody, work.Info.Matr, csys);
                    return cp.GetBodyInfoForInterference(false);
                }
            }

            return null;
        }
        /// <summary>
        /// 获取装配下的体
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        private Body GetOccsInBods(Component ct)
        {
            Part ctPart = ct.Prototype as Part;
            Body by = ctPart.Bodies.ToArray()[0];
            try
            {
                return AssmbliesUtils.GetNXObjectOfOcc(ct.Tag, by.Tag) as Body;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 获取Part的OCC
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        private Component GetPartInOcc(Part pt)
        {
            Part workPart = Session.GetSession().Parts.Work;
            try
            {
                List<Component> cts = AssmbliesUtils.GetPartComp(workPart, pt);
                foreach (Component ct in cts)
                {
                    if (ct.Parent.Parent.Equals(workComp))
                        return ct;
                }
                return null;
            }
            catch
            {

                return null;
            }
        }
        /// <summary>
        /// 移动电极
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private bool MoveElePart(Component eleCt, ElectrodePitchInfo pitch, int dir)
        {
            if ((pitch.PitchXNum > 1 && Math.Abs(pitch.PitchX) > 0) ||
                (pitch.PitchYNum > 1 && Math.Abs(pitch.PitchY) > 0))
            {
                Vector3d vec = new Vector3d();
                vec.X = (pitch.PitchXNum - 1) * pitch.PitchX * dir;
                vec.Y = (pitch.PitchYNum - 1) * pitch.PitchY * dir;
                AssmbliesUtils.MoveCompPart(eleCt, vec, this.work.Info.Matr);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 干涉检查
        /// </summary>
        /// <param name="eleCtBody"></param>
        /// <param name="eleName"></param>
        /// <param name="post"></param>
        /// <param name="picth"></param>
        /// <param name="err"></param>
        private void Interference(Component eleCt, string eleName, string post, string picth, ref List<string> err)
        {
            Body eleCtBody = GetOccsInBods(eleCt);
            foreach (Part pt in work.GetAllWorkpiece())
            {
                Component ptCoo = GetPartInOcc(pt);
                if (ptCoo != null)
                {
                    Body by = GetOccsInBods(ptCoo);
                    List<Body> bodys = new List<Body>();
                    if (by != null)
                    {
                        try
                        {
                            NXOpen.GeometricAnalysis.SimpleInterference.Result re = AnalysisUtils.SetInterferenceOutResult(eleCtBody, by, out bodys);
                            if (re == NXOpen.GeometricAnalysis.SimpleInterference.Result.NoInterference)
                                err.Add(eleName + "                     " + pt.Name + picth + "没有干涉！");
                            if (re == NXOpen.GeometricAnalysis.SimpleInterference.Result.InterferenceExists)
                                err.Add(eleName + "                     " + pt.Name + picth + "有干涉！");
                            if (bodys.Count > 0)
                            {
                                foreach (Body body in bodys)
                                {
                                    body.Layer = 252;
                                }
                            }
                        }
                        catch
                        {
                            ClassItem.WriteLogFile("干涉检查错误！");
                        }



                    }
                    else
                    {
                        err.Add(pt.Name + "                   无法找到工件体，请检查引用集！");
                    }

                }
                else
                {
                    err.Add(pt.Name + "                   无法找到组件，请检查引用集！");
                }
            }
        }

        public List<string> InterferenceBulider()
        {
            List<string> err = new List<string>();
            Matrix4 inv = this.work.Info.Matr.GetInversMatrix();
            CartesianCoordinateSystem csys = BoundingBoxUtils.CreateCoordinateSystem(this.work.Info.Matr, inv);
            foreach (Component eleCt in GetEleAllComp())
            {
                ElectrodeInfo info = ElectrodeInfo.GetAttribute(eleCt);
                AskSetVaule(csys, info.AllInfo.SetValue, eleCt, ref err);
                Interference(eleCt, info.AllInfo.Name.EleName, info.AllInfo.SetValue.Positioning, "", ref err);
                if (MoveElePart(eleCt, info.AllInfo.Pitch, -1))
                {
                    Interference(eleCt, info.AllInfo.Name.EleName, info.AllInfo.SetValue.Positioning, "Pitch", ref err);
                    MoveElePart(eleCt, info.AllInfo.Pitch, 1);
                }

            }
            return err;
        }


    }
}
