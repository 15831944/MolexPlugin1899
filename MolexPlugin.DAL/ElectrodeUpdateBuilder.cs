using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;
using MolexPlugin.DAL;
using MolexPlugin.Model;
using System.IO;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 电极更新
    /// </summary>
    public class ElectrodeUpdateBuilder
    {
        private ElectrodeModel eleModel;
        private ElectrodeAllInfo newAllInfo;
        private ElectrodeAllInfo oldAllInfo;
        private Part asm;

        public ElectrodeUpdateBuilder(ElectrodeModel model, ElectrodeAllInfo newAllInfo, Part asm)
        {
            this.eleModel = model;
            this.newAllInfo = newAllInfo;
            this.asm = asm;
            this.oldAllInfo = model.Info.AllInfo.Clone() as ElectrodeAllInfo;
        }
        /// <summary>
        /// 获取布尔和拉伸体
        /// </summary>       
        /// <param name="boolBody">拉伸相加体</param>
        /// <param name="extBools">其他相加特征</param>
        /// <returns></returns>
        private void GetExtrudeBooleanBody(out Body boolBody, out List<NXOpen.Features.BooleanFeature> extBools)
        {
            List<NXOpen.Features.Feature> extrudes = new List<NXOpen.Features.Feature>();
            extBools = new List<NXOpen.Features.BooleanFeature>();
            boolBody = null;
            foreach (NXOpen.Features.Feature ft in eleModel.PartTag.Features.ToArray())
            {
                if (ft.FeatureType.Equals("EXTRUDE", StringComparison.CurrentCultureIgnoreCase))
                {
                    extrudes.Add(ft);
                }
                if (ft.FeatureType.Equals("UNITE", StringComparison.CurrentCultureIgnoreCase))
                {
                    extBools.Add(ft as NXOpen.Features.BooleanFeature);
                }
            }
            foreach (NXOpen.Features.BooleanFeature bf in extBools)
            {
                bool isok = true;
                foreach (NXOpen.Features.Feature ff in bf.GetParents())
                {
                    if (!extrudes.Exists(a => a.Equals(ff)))
                    {
                        isok = false;
                        break;
                    }
                }
                if (isok)
                {
                    boolBody = bf.GetBodies()[0];
                    extBools.Remove(bf);
                    break;
                }

            }
        }
        /// <summary>
        /// 获取特征
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ext"></param>
        /// <param name="move"></param>
        /// <param name="patt"></param>
        private void GetFeature(out NXOpen.Features.ExtractFace ext, out NXOpen.Features.MoveObject move, out NXOpen.Features.PatternGeometry patt)
        {
            ext = null;
            move = null;
            patt = null;
            foreach (NXOpen.Features.Feature ft in eleModel.PartTag.Features.ToArray())
            {
                if (ft.FeatureType.Equals("LINKED_BODY", StringComparison.CurrentCultureIgnoreCase))
                {
                    ext = ft as NXOpen.Features.ExtractFace;
                }
                if (ft.FeatureType.Equals("Pattern Geometry", StringComparison.CurrentCultureIgnoreCase))
                {
                    patt = ft as NXOpen.Features.PatternGeometry;
                }
                if (ft.FeatureType.Equals("MOVE_OBJECT", StringComparison.CurrentCultureIgnoreCase))
                {
                    move = ft as NXOpen.Features.MoveObject;
                }
            }
        }

        /// <summary>
        /// 更新Pitch
        /// </summary>
        private bool UpdatePitch()
        {
            Part workPart = Session.GetSession().Parts.Work;
            NXOpen.Features.ExtractFace ext;
            NXOpen.Features.MoveObject move;
            NXOpen.Features.PatternGeometry patt;
            List<Body> bodys = new List<Body>();
            Body boolBody;
            List<NXOpen.Features.BooleanFeature> extBools;
            GetExtrudeBooleanBody(out boolBody, out extBools);
            GetFeature(out ext, out move, out patt);
            if (ext == null || move == null || patt == null || boolBody == null)
                return false;
            if (extBools.Count > 0)
                DeleteObject.Delete(extBools.ToArray());
            bodys.AddRange(ext.GetBodies());
            bodys.AddRange(patt.GetAssociatedBodies());
            try
            {
                MoveObject.CreateMoveObjToXYZ("moveX", "moveY", "moveZ", move as NXOpen.Features.MoveObject, bodys.ToArray());
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("更新移动错误！" + ex.Message);
                return false;
            }
            List<Body> bys = eleModel.PartTag.Bodies.ToArray().ToList();
            bys.Remove(boolBody);
            SetHeadColour(newAllInfo.Pitch, newAllInfo.GapValue, bys);
            try
            {
                PartUtils.SetPartWork(AssmbliesUtils.GetPartComp(asm, eleModel.PartTag)[0]);
                Body elebody = BooleanUtils.CreateBooleanFeature(boolBody, false, false, NXOpen.Features.Feature.BooleanType.Unite, bys.ToArray()).GetBodies()[0];
                SetEleColor(elebody);
                PartUtils.SetPartWork(null);
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("更新求和错误！" + ex.Message);
                return false;
            }
            return true;

        }
        /// <summary>
        /// 给齿上颜色
        /// </summary>
        private void SetHeadColour(ElectrodePitchInfo pitch, ElectrodeGapValueInfo gapValue, List<Body> bodys)
        {
            Matrix4 matr = new Matrix4();
            matr.Identity();
            Matrix4 inv = matr.GetInversMatrix();
            CartesianCoordinateSystem csys = BoundingBoxUtils.CreateCoordinateSystem(matr, inv);
            var toolhList = bodys.GroupBy(a => AttributeUtils.GetAttrForString(a, "ToolhName"));
            List<ElectrodeToolhInfo[,]> toolhInfos = new List<ElectrodeToolhInfo[,]>();
            try
            {
                foreach (var toolh in toolhList)
                {
                    ElectrodeToolhInfo[,] toolhInfo = pitch.GetToolhInfosForAttribute(toolh.ToList(), matr, csys);
                    toolhInfos.Add(toolhInfo);
                }
                if (toolhInfos.Count != 0)
                    gapValue.SetERToolh(toolhInfos);
            }
            catch (Exception ex)
            {
                ClassItem.WriteLogFile("设置颜色错误！" + ex.Message);
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
        /// 移动体
        /// </summary>
        /// <param name="eleAsmComp"></param>
        /// <returns></returns>
        private bool UpdateMoveComp(NXOpen.Assemblies.Component eleAsmComp)
        {
            ElectrodeAllInfo all = newAllInfo.Clone() as ElectrodeAllInfo;
            ElectrodeSetValueInfo setValueInfo = ElectrodeSetValueInfo.GetAttribute(eleAsmComp);
            Part workPt = eleAsmComp.Parent.Prototype as Part;
            WorkModel work = new WorkModel(workPt);
            ElectrodePitchUpdate pitchUpdate = new ElectrodePitchUpdate(eleModel.Info.AllInfo.Pitch, newAllInfo.Pitch);
            Vector3d temp = pitchUpdate.GetIncrement();
            double[] setValue = pitchUpdate.GetNewSetValue(setValueInfo.EleSetValue);
            setValueInfo.EleSetValue = setValue;
            all.SetValue = setValueInfo;
            try
            {
                PartUtils.SetPartDisplay(asm);
                AssmbliesUtils.MoveCompPart(eleAsmComp, temp, work.Info.Matr);
                NXObject obj = AssmbliesUtils.GetOccOfInstance(eleAsmComp.Tag);
                foreach (NXOpen.Assemblies.Component ct in AssmbliesUtils.GetPartComp(work.PartTag, eleModel.PartTag))
                {
                    NXObject tem = AssmbliesUtils.GetOccOfInstance(ct.Tag);
                    if (tem.Equals(obj))
                    {
                        PartUtils.SetPartDisplay(work.PartTag);
                        AssmbliesUtils.MoveCompPart(ct, temp, work.Info.Matr);
                    }

                }
                PartUtils.SetPartDisplay(asm);
                all.SetAttribute(obj);
                return true;
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("移动组件错误！" + ex.Message);
                return false;
            }

        }

        private bool UpdateAttr(NXOpen.Assemblies.Component eleAsmComp)
        {
            ElectrodeAllInfo all = newAllInfo.Clone() as ElectrodeAllInfo;
            ElectrodeSetValueInfo setValueInfo = ElectrodeSetValueInfo.GetAttribute(eleAsmComp);
            all.SetValue = setValueInfo;
            try
            {
                NXObject obj = AssmbliesUtils.GetOccOfInstance(eleAsmComp.Tag);
                all.SetAttribute(obj);
                return true;
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("移动组件错误！" + ex.Message);
                return false;
            }

        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public bool UpdateEleBuilder()
        {
            newAllInfo.SetAttribute(eleModel.PartTag);
            bool isok = false;
            List<NXOpen.Assemblies.Component> eleCts = AssmbliesUtils.GetPartComp(asm, eleModel.PartTag);
            if (!oldAllInfo.Pitch.IsEquals(newAllInfo.Pitch) || !oldAllInfo.GapValue.IsEquals(newAllInfo.GapValue))
            {
                isok = UpdatePitch();
            }
            foreach (NXOpen.Assemblies.Component ct in eleCts)
            {
                if (!eleModel.Info.AllInfo.Pitch.IsEquals(newAllInfo.Pitch))
                {
                    isok = UpdateMoveComp(ct);
                }
                else
                {
                    isok = UpdateAttr(ct);
                }

            }
            return isok;

        }
        /// <summary>
        /// 更新图纸
        /// </summary>
        public void UpdateDrawing()
        {
            string dwgName = eleModel.Info.AllInfo.Name.EleName + "_dwg";
            string path = eleModel.WorkpieceDirectoryPath + dwgName + ".prt";
            Part dwg = null;
            foreach (Part part in Session.GetSession().Parts)
            {
                if (part.Name.ToUpper().Equals(dwgName.ToUpper()))
                {
                    dwg = part;
                    break;
                }

            }
            if (dwg == null)
            {
                if (File.Exists(path))
                {
                    dwg = PartUtils.OpenPartFile(path);
                }
            }
            if (dwg != null)
            {
                newAllInfo.SetAttribute(dwg);
                string temp = newAllInfo.Preparetion.Preparation[0].ToString() + "*" + newAllInfo.Preparetion.Preparation[1].ToString() + "*" + newAllInfo.Preparetion.Preparation[2].ToString();
                AttributeUtils.AttributeOperation("StrPre", temp, dwg);
                if (!oldAllInfo.Pitch.IsEquals(newAllInfo.Pitch))
                {
                    PartUtils.SetPartDisplay(dwg);
                    LayerUtils.MoveDisplayableObject(201, GetDrawingBody(dwg).ToArray());
                    foreach (NXOpen.Drawings.DrawingSheet sh in dwg.DrawingSheets)
                    {
                        Basic.DrawingUtils.UpdateViews(sh);
                    }
                }
                PartUtils.SetPartDisplay(asm);
            }
        }

        private List<Body> GetDrawingBody(Part draPart)
        {
            List<Body> bodys = new List<Body>();
            Body by = eleModel.PartTag.Bodies.ToArray()[0];
            foreach (NXOpen.Assemblies.Component ct in AssmbliesUtils.GetPartComp(draPart, eleModel.PartTag))
            {
                try
                {
                    bodys.Add(AssmbliesUtils.GetNXObjectOfOcc(ct.Tag, by.Tag) as Body);
                }
                catch
                {
                }
            }
            return bodys;

        }

    }
}
