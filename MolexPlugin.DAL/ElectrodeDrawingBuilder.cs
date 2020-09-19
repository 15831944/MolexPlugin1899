using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;
using MolexPlugin.Model;
using NXOpen.UF;
using NXOpen.Drawings;
using NXOpen.Assemblies;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 电极制图
    /// </summary>
    public class ElectrodeDrawingBuilder
    {
        private ElectrodeDrawingModel model = null;
        private string eleTemplate;
        private ASMModel asm;
        private Point3d disPt;
        private Point3d centerPt;
        private List<Component> setValueHidden;
        private List<Component> eleHidden;
        public ElectrodeDrawingBuilder(ElectrodeDrawingModel model, ASMModel asm)
        {
            this.model = model;
            this.asm = asm;
            GetTemplate();
        }
        private void GetTemplate()
        {
            string dllPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            eleTemplate = dllPath.Replace("application\\", "part\\electrode.prt");
        }
        /// <summary>
        /// 创建图纸part
        /// </summary>
        private void CreatDwgPart()
        {
            foreach (Part part in Session.GetSession().Parts)
            {
                if (part.Name.ToUpper().Equals(model.AssembleName.ToUpper()))
                    part.Close(NXOpen.BasePart.CloseWholeTree.False, NXOpen.BasePart.CloseModified.UseResponses, null);
            }

            model.CreatePart(asm.WorkpieceDirectoryPath);
            model.LoadWork();
            PartUtils.SetPartDisplay(model.PartTag);
            model.GetBoundingBox(out centerPt, out disPt);
            model.GetHidden(out setValueHidden, out eleHidden);
        }
        /// <summary>
        /// 获取设定视图的中心点
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        private Point3d GetSetValueViewOriginPt(double scale)
        {

            double higth = 2 * disPt.Y * scale + 2 * disPt.Z * scale;
            double k = (220 - higth) / 2;
            Point3d temp = new Point3d(0, 0, 0);
            temp.X = 90;
            temp.Y = 230 - (disPt.Y * scale);
            return temp;
        }
        /// <summary>
        /// 获取电极视图的中心点
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        private Point3d GetEleOrigin(double scale)
        {
            int[] pre = model.Info.AllInfo.Preparetion.Preparation;
            Point3d temp = new Point3d(0, 0, 0);
            temp.Y = 230 - (pre[2] * scale + pre[1] * scale / 2);
            temp.X = 270 + (pre[1] * scale + pre[0] * scale / 2);
            return temp;
        }
        /// <summary>
        /// 设置视图显示层
        /// </summary>
        /// <param name="views"></param>
        private void SetViewVisible(params NXOpen.Drawings.DraftingView[] views)
        {
            foreach (DraftingView dv in views)
            {
                Basic.DrawingUtils.SetLayerHidden(dv);
                Basic.DrawingUtils.SetLayerVisible(new int[1] { 20 }, dv);
            }
        }
        /// <summary>
        /// 点排序
        /// </summary>
        /// <param name="points"></param>
        /// <param name="mat"></param>
        /// <param name="axisName"></param>
        private void PointSort(ref List<Point> points, Matrix4 mat, string axisName)
        {
            points.Sort(delegate (Point a, Point b)
            {
                Point3d pt1 = a.Coordinates;
                Point3d pt2 = b.Coordinates;
                mat.ApplyPos(ref pt1);
                mat.ApplyPos(ref pt2);
                if (axisName == "X")
                    return pt1.X.CompareTo(pt2.X);
                if (axisName == "Y")
                    return pt1.Y.CompareTo(pt2.Y);
                if (axisName == "Z")
                    return pt1.Z.CompareTo(pt2.Z);
                return 1;
            });
        }
        /// <summary>
        /// 设置尺寸颜色
        /// </summary>
        /// <param name="dim"></param>
        private void SetDimColor(NXOpen.Annotations.Dimension dim)
        {
            Part workPart = Session.GetSession().Parts.Work;
            NXOpen.Annotations.LinearDimensionBuilder linearDimensionBuilder1;
            linearDimensionBuilder1 = workPart.Dimensions.CreateLinearDimensionBuilder(dim);

            linearDimensionBuilder1.Style.LineArrowStyle.SecondArrowheadColor = workPart.Colors.Find("Magenta");

            linearDimensionBuilder1.Style.LineArrowStyle.FirstExtensionLineColor = workPart.Colors.Find("Magenta");

            linearDimensionBuilder1.Style.LineArrowStyle.SecondExtensionLineColor = workPart.Colors.Find("Magenta");

            linearDimensionBuilder1.Style.LineArrowStyle.FirstArrowheadColor = workPart.Colors.Find("Magenta");

            linearDimensionBuilder1.Style.LineArrowStyle.FirstArrowLineColor = workPart.Colors.Find("Magenta");

            linearDimensionBuilder1.Style.LineArrowStyle.SecondArrowLineColor = workPart.Colors.Find("Magenta");

            linearDimensionBuilder1.Style.LetteringStyle.DimensionTextColor = workPart.Colors.Find("Magenta");

            linearDimensionBuilder1.Style.LetteringStyle.AppendedTextColor = workPart.Colors.Find("Magenta");

            linearDimensionBuilder1.Style.LetteringStyle.ToleranceTextColor = workPart.Colors.Find("Magenta");

            NXOpen.NXObject nXObject1;
            nXObject1 = linearDimensionBuilder1.Commit();
            linearDimensionBuilder1.Destroy();
        }
        /// <summary>
        /// 创建设定图
        /// </summary>
        public void CreateSetValueView()
        {
            double scale = model.GetScale(130.0, 190.0, disPt);
            NXOpen.Drawings.DrawingSheet sheet = Basic.DrawingUtils.DrawingSheet(eleTemplate, 297, 420, model.AssembleName);
            Point3d origin = GetSetValueViewOriginPt(scale);
            Point3d projectedPt = new Point3d(0, 0, 0);
            projectedPt.X = origin.X;
            projectedPt.Y = origin.Y - (disPt.Y + disPt.Z) * scale - 30;
            DraftingView topView = null;
            DraftingView proView = null;
            try
            {
                topView = Basic.DrawingUtils.CreateView("TOP", origin, scale, model.GetWorkMatr(), setValueHidden.ToArray());
                proView = Basic.DrawingUtils.CreateProjectedView(topView, projectedPt, scale);
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile(model.AssembleName + "视图创建错误！" + ex.Message);
            }
            if (topView != null)
                TopDimension(topView, origin, scale, model.GetWorkCompPoint(), model.GetEleSetPoint());
            if (proView != null)
                ProViewDimension(proView, projectedPt, scale, model.GetWorkCompPoint(), model.GetEleSetPoint());
        }
        /// <summary>
        /// 设定视图水平标注
        /// </summary>
        /// <param name="topView"></param>
        /// <param name="originPt"></param>
        /// <param name="scale"></param>
        /// <param name="workPoint"></param>
        /// <param name="elePoint"></param>
        private void TopDimension(DraftingView topView, Point3d originPt, double scale, Point workPoint, List<Point> elePoint)
        {
            string err = "";

            Matrix4 mat = model.GetWorkMatr();
            PointSort(ref elePoint, mat, "X");
            for (int i = 0; i < elePoint.Count; i++)
            {
                Point3d dimPt = new Point3d(originPt.X + 10.0, originPt.Y + disPt.Y * scale + (10 * (i + 1)), 0);
                try
                {
                    NXOpen.Annotations.Dimension dim = Basic.DrawingUtils.DimensionHorizontal(topView, dimPt, workPoint, elePoint[i], ref err);
                    if (dim != null)
                    {
                        Basic.DrawingUtils.AppendedTextDim(dim, "EDM SETTING");
                        SetDimColor(dim);
                    }
                }
                catch (NXException ex)
                {
                    ClassItem.WriteLogFile(model.AssembleName + "设定视图水平标注错误！" + ex.Message);
                }

            }
            PointSort(ref elePoint, mat, "Y");
            for (int i = 0; i < elePoint.Count; i++)
            {
                Point3d dimPt = new Point3d(originPt.X - (this.disPt.X * scale + 8 * (i + 1)), originPt.Y + 10, 0);
                try
                {
                    NXOpen.Annotations.Dimension dim = Basic.DrawingUtils.DimensionVertical(topView, dimPt, workPoint, elePoint[i], ref err);
                    if (dim != null)
                    {
                        Basic.DrawingUtils.AppendedTextDim(dim, "EDM SETTING");
                        SetDimColor(dim);
                    }
                }
                catch (NXException ex)
                {
                    ClassItem.WriteLogFile(model.AssembleName + "设定竖直标注错误！" + ex.Message);
                }

            }

        }
        /// <summary>
        /// 设定视图竖直标注
        /// </summary>
        /// <param name="topView"></param>
        /// <param name="originPt"></param>
        /// <param name="scale"></param>
        /// <param name="workPoint"></param>
        /// <param name="elePoint"></param>
        private void ProViewDimension(DraftingView topView, Point3d originPt, double scale, Point workPoint, List<Point> elePoint)
        {
            string err = "";
            Point3d dimPt = new Point3d(originPt.X - (disPt.X * scale + 10), originPt.Y + (disPt.X * scale), 0);
            try
            {
                NXOpen.Annotations.Dimension dim = Basic.DrawingUtils.DimensionVertical(topView, dimPt, workPoint, elePoint[0], ref err);
                if (dim != null)
                {
                    Basic.DrawingUtils.AppendedTextDim(dim, "EDM SETTING");
                    SetDimColor(dim);
                }
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile(model.AssembleName + "设定视图竖直标注错误！" + ex.Message);
            }

        }
        /// <summary>
        /// 创建电极视图
        /// </summary>
        public void CreateEleView()
        {
            double eleScale = model.GetEleScale(130.0, 150.0);
            int[] pre = model.Info.AllInfo.Preparetion.Preparation;
            Point3d eleOrigin = GetEleOrigin(eleScale);
            Point3d projectedElePt1 = new Point3d(eleOrigin.X, eleOrigin.Y + (pre[1] * eleScale / 2 + pre[2] * eleScale / 2 + 30), 0);
            Point3d projectedElePt2 = new Point3d(projectedElePt1.X - (pre[0] * eleScale / 2 + pre[1] * eleScale / 2 + 30), projectedElePt1.Y, 0);
            DraftingView topEleView = null;
            try
            {
                topEleView = Basic.DrawingUtils.CreateView("TOP", eleOrigin, eleScale, model.GetEleMatr(), eleHidden.ToArray());
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile(model.AssembleName + "电极视图创建错误！" + ex.Message);
                return;
            }
            Basic.DrawingUtils.SetWireframeColorSource(NXOpen.Preferences.GeneralWireframeColorSourceOption.FromFace, topEleView);
            DraftingView proEleView1;
            DraftingView proEleView2;
            try
            {
                proEleView1 = Basic.DrawingUtils.CreateProjectedView(topEleView, projectedElePt1, eleScale);
                proEleView2 = Basic.DrawingUtils.CreateProjectedView(proEleView1, projectedElePt2, eleScale);
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile(model.AssembleName + "电极视图创建错误！" + ex.Message);
                return;
            }
            EleProViewDimension(proEleView2, projectedElePt2, eleScale);

            Matrix4 mat = model.GetEleMatr();
            mat.RolateWithX(-2 * Math.PI / 5);
            mat.RolateWithY(Math.PI / 10);

            DraftingView topEleView2;
            try
            {
                topEleView2 = Basic.DrawingUtils.CreateView("Trimetric", new Point3d(240, 120, 0), eleScale, mat, eleHidden.ToArray());
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile(model.AssembleName + "电极视图创建错误！" + ex.Message);
                return;
            }
            SetViewVisible(new DraftingView[] { topEleView, proEleView1, proEleView2, topEleView2 });
        }

        /// <summary>
        /// 电极竖直标注
        /// </summary>
        /// <param name="topView"></param>
        /// <param name="originPt"></param>
        /// <param name="scale"></param>
        private void EleProViewDimension(DraftingView topView, Point3d originPt, double scale)
        {
            string err = "";
            int[] pre = model.Info.AllInfo.Preparetion.Preparation;
            List<Edge> xEdge = new List<Edge>();
            Point centerPt;
            model.GetEdge(out xEdge, out centerPt);
            Point3d dimPt = new Point3d(originPt.X, originPt.Y - (pre[1] * scale / 2 + 10), 0);
            if (xEdge.Count == 0 || centerPt == null)
                return;
            try
            {
                NXOpen.Annotations.Dimension dim = Basic.DrawingUtils.DimensionVertical(topView, dimPt, xEdge[0], centerPt, ref err);
                if (dim != null)
                    Basic.DrawingUtils.SetDimensionPrecision(dim, 1);
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile(model.AssembleName + "电极视图竖直标注错误！" + ex.Message);
            }
        }
        /// <summary>
        /// 创建特征
        /// </summary>
        public void CreateBulider()
        {
            CreatDwgPart();
            CreateSetValueView();
            CreateEleView();
        }
    }
}
