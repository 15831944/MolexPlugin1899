using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;
using MolexPlugin.Model;
using NXOpen.UF;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 创建电极
    /// </summary>
    public class CreateElectrode
    {
        private ElectrodeAllInfo allInfo;
        private ElectrodeCreateCondition condition;
        private ParentAssmblieInfo parent;
        public bool zDatum;

        public CreateElectrode(ElectrodeAllInfo allInfo, ParentAssmblieInfo parent, ElectrodeCreateCondition condition, bool zDatum)
        {
            this.allInfo = allInfo;
            this.parent = parent;
            this.condition = condition;
            this.zDatum = zDatum;
        }

        /// <summary>
        /// 反求单齿设置值
        /// </summary>
        /// <param name="setValue"></param>
        /// <param name="pitch"></param>
        /// <param name="pre"></param>
        /// <param name="zDatum"></param>
        /// <returns></returns>
        private Point3d GetSingleHeadSetValue()
        {
            Point3d temp = new Point3d(allInfo.SetValue.EleSetValue[0], allInfo.SetValue.EleSetValue[1], allInfo.SetValue.EleSetValue[2]);
            double x1 = temp.X - (allInfo.Pitch.PitchXNum - 1) * allInfo.Pitch.PitchX / 2;
            double y1 = temp.Y - (allInfo.Pitch.PitchYNum - 1) * allInfo.Pitch.PitchY / 2;
            if (zDatum)
            {
                if (allInfo.Preparetion.Preparation[0] > allInfo.Preparetion.Preparation[1])
                {
                    x1 = temp.X - (allInfo.Pitch.PitchXNum - 2) * allInfo.Pitch.PitchX / 2;
                }
                else
                {
                    y1 = temp.Y - (allInfo.Pitch.PitchYNum - 2) * allInfo.Pitch.PitchY / 2;
                }
            }
            Point3d setValue = new Point3d(x1, y1, temp.Z);
            Matrix4 inv = condition.Work.Info.Matr.GetInversMatrix();
            inv.ApplyPos(ref setValue);
            return setValue;
        }
        /// <summary>
        /// 创建电极Model
        /// </summary>
        /// <returns></returns>
        private ElectrodeInfo GetEleInfo()
        {
            condition.ExpAndMatr.Matr.SetMatrixOrigin(allInfo.Preparetion.Preparation, GetSingleHeadSetValue()); //设置矩阵中心点
            Matrix4 eleMat = condition.ExpAndMatr.Matr.EleMatr;
            Matrix4Info matInfo = new Matrix4Info(eleMat);
            return new ElectrodeInfo(parent.MoldInfo, parent.UserModel, allInfo, matInfo);
        }

        public List<string> CreateBuider()
        {
            List<string> err = new List<string>();
            ElectrodePartBuilder part = new ElectrodePartBuilder(GetEleInfo(), condition.Work.WorkpieceDirectoryPath);
            bool ptOk = part.CreatPart();
            if (ptOk)
            {
                List<Body> bodys = new List<Body>();
                List<Body> headBodys = new List<Body>();
                bool isok = false;
                ElectrodeDatumBuilder datum = null;
                try
                {
                    headBodys = part.WaveEleHeadBody(condition.HeadBodys);
                }
                catch (NXException ex)
                {
                    err.Add("连接体错！             " + ex.Message);
                    return err;
                }
                try
                {
                    isok = condition.ExpAndMatr.Exp.CreateExp(zDatum, allInfo.Preparetion.Preparation);
                }
                catch (NXException ex)
                {
                    err.Add("创建表达式错误！         " + ex.Message);
                    return err;
                }
                try
                {
                    ElectrodeSketchBuilder sketch = new ElectrodeSketchBuilder(allInfo.Preparetion.Preparation[0], allInfo.Preparetion.Preparation[1], -allInfo.Datum.EleHeight);
                    datum = new ElectrodeDatumBuilder(sketch);
                    ElectrodeMoveBuilder move = new ElectrodeMoveBuilder(headBodys, allInfo.Datum, allInfo.GapValue, allInfo.Pitch);
                    datum.SetParentBuilder(sketch);
                    sketch.SetParentBuilder(move);
                    sketch.CreateBuilder();
                    move.CreateBuilder();
                    datum.CreateBuilder();
                    bodys.AddRange(move.AllBodys);
                    if (zDatum)
                    {
                        ElectrodeFeelerBuilder feeler = new ElectrodeFeelerBuilder(sketch, this.allInfo.Datum);
                        feeler.CreateBuilder();
                        bodys.Add(feeler.FeelerBody);
                    }
                }
                catch (NXException ex)
                {
                    err.Add("创建电极特征错误！          " + ex.Message);
                    return err;
                }
                try
                {
                    string errs;
                    Body by = CreateUnite(datum.DatumBody, bodys, out errs);
                    SetEleColor(by);
                    if (errs != "")
                    {
                        err.Add(errs);
                    }
                }
                catch (NXException ex)
                {
                    err.Add("电极求和错误！          " + ex.Message);
                }
                CreateCenterPoint(part, zDatum);
                PartUtils.SetPartWork(null);
                part.MoveEleComp(condition.Work.Info.Matr, part.GetMove(zDatum));
                MoveHeadTolayer();
                MoveEleComp(part.EleComp);
            }
            else
            {
                err.Add("创建电极档错误！");
            }

            return err;

        }
        /// <summary>
        /// 求和
        /// </summary>
        /// <param name="body1"></param>
        /// <param name="bodys"></param>
        public Body CreateUnite(Body body1, List<Body> bodys, out string errs)
        {
            errs = "";
            string[] err = null;
            NXOpen.Features.BooleanFeature bf = null;
            try
            {
                bf = BooleanUtils.CreateBooleanFeature(body1, false, false, NXOpen.Features.Feature.BooleanType.Unite, bodys.ToArray());
                err = bf.GetFeatureWarningMessages();
            }
            catch (NXException ex)
            {
                throw ex;
            }
            if (err != null)
            {
                foreach (string str in err)
                {
                    if (str.Equals("One or more tools did not intersect the target.", StringComparison.CurrentCultureIgnoreCase))
                    {

                        errs = ("一个或多个工具与目标不相交。");
                    }

                }
            }
            return bf.GetBodies()[0];
        }
        /// <summary>
        /// 移动层
        /// </summary>
        public void MoveHeadTolayer()
        {
            foreach (Body by in condition.HeadBodys)
            {
                if (allInfo.Name.EleNumber >= 100)
                {
                    int temp = allInfo.Name.EleNumber - allInfo.Name.EleNumber / 100 * 100;
                    by.Layer = 100 + temp;
                }
                else
                {
                    by.Layer = allInfo.Name.EleNumber + 100;
                }

            }
        }
        /// <summary>
        /// AB齿跑位
        /// </summary>
        /// <param name="eleComp"></param>
        public void MoveEleComp(NXOpen.Assemblies.Component eleComp)
        {
            for (int i = 1; i < condition.ToolhInfo.Count; i++)
            {
                Vector3d temp = new Vector3d();
                temp.X = -condition.ToolhInfo[i].Offset[0];
                temp.Y = -condition.ToolhInfo[i].Offset[1];
                temp.Z = 0;
                NXOpen.Assemblies.Component ct = AssmbliesUtils.MoveCompCopyPart(eleComp, temp, condition.Work.Info.Matr);
                NXObject instance = AssmbliesUtils.GetOccOfInstance(ct.Tag);
                ElectrodeSetValueInfo setValue = allInfo.SetValue.Clone() as ElectrodeSetValueInfo;
                setValue.EleSetValue[0] = setValue.EleSetValue[0] + temp.X;
                setValue.EleSetValue[1] = setValue.EleSetValue[1] + temp.Y;

                setValue.ContactArea = condition.ToolhInfo[i].GetAllContactArea();
                setValue.ProjectedArea = condition.ToolhInfo[i].GetProjectedArea(condition.Work.Info.Matr);
                setValue.Positioning = condition.ToolhInfo[i].ToolhName;
                setValue.SetAttribute(instance);
            }
        }
        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="by"></param>
        private void SetEleColor(Body by)
        {
            foreach (Face fe in by.GetFaces())
            {
                string gap = AttributeUtils.GetAttrForString(fe, "ToolhGapValue");
                if (gap.Equals("ER", StringComparison.CurrentCultureIgnoreCase))
                    fe.Color = 6;
                if (gap.Equals("EF", StringComparison.CurrentCultureIgnoreCase))
                    fe.Color = 108;
            }
        }

        /// <summary>
        /// 创建中心点
        /// </summary>
        private void CreateCenterPoint(ElectrodePartBuilder part, bool zDatum)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            Point3d center = part.GetSetValuePoint(zDatum);
            Matrix4 inver = condition.Work.Info.Matr.GetInversMatrix();
            inver.ApplyPos(ref center);
            this.condition.ExpAndMatr.Matr.EleMatr.ApplyPos(ref center);
            Point pt = PointUtils.CreatePointFeature(center);
            pt.Color = 186;
            pt.Layer = 254;
            theUFSession.Obj.SetName(pt.Tag, "SetValuePoint");

        }
    }
}
