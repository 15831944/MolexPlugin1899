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

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 工件图纸
    /// </summary>
    public class WorkpieceDrawing
    {

        private WorkpieceDrawingModel drawModel;
        private WorkModel workModel;
        private Point originPoint;
        private WorkDrawingModel workDra;
        public WorkpieceDrawing(WorkpieceDrawingModel drawModel, WorkModel workModel, WorkDrawingModel workDra, Point originPoint)
        {
            this.drawModel = drawModel;
            this.workModel = workModel;
            this.originPoint = originPoint;
            this.workDra = workDra;
        }
        /// <summary>
        /// 获取最小设定值
        /// </summary>
        /// <returns></returns>
        private Point3d GetMinSetValue()
        {
            Point3d min = new Point3d();
            Point3d minPt = drawModel.MinPt.Coordinates;
            Point3d maxPt = drawModel.MaxPt.Coordinates;
            this.workModel.Info.Matr.ApplyPos(ref minPt);
            this.workModel.Info.Matr.ApplyPos(ref maxPt);
            //if (Math.Abs(minPt.X) > Math.Abs(maxPt.X))
            //    min.X = maxPt.X;
            //else
            //    min.X = minPt.X;
            //if (Math.Abs(minPt.Y) > Math.Abs(maxPt.Y))
            //    min.Y = maxPt.Y;
            //else
            //    min.Y = minPt.Y;
            //if (Math.Abs(minPt.Z) > Math.Abs(maxPt.Z))
            //    min.Z = maxPt.Z;
            //else
            //    min.Z = minPt.Z;

            min.X = minPt.X;
            min.Y = maxPt.Y;
            min.Z = maxPt.Z;
            return min;
        }

        /// <summary>
        /// 设置表格注释
        /// </summary>
        /// <param name="tablePath"></param>
        /// <param name="origin"></param>
        private void SetTable(string tablePath, double[] origin)
        {
            Tag rowTag = Tag.Null;
            Tag oldrowTag = Tag.Null;
            Tag[] columnsTags = new Tag[5];
            Tag[] cellTag = new Tag[5];
            Tag[] oldcellTag = new Tag[5];
            UFSession theUFSession = UFSession.GetUFSession();
            Point3d min = GetMinSetValue();
            Tag tableTag = Basic.DrawingUtils.CreateTable(tablePath, origin, 1);
            theUFSession.Tabnot.AskNthRow(tableTag, 0, out oldrowTag);
            theUFSession.Tabnot.AskNthRow(tableTag, 1, out rowTag);
            for (int i = 0; i < 5; i++)
            {
                UFTabnot.CellPrefs cellPrefs = new UFTabnot.CellPrefs();
                theUFSession.Tabnot.AskNthColumn(tableTag, i, out columnsTags[i]);
                theUFSession.Tabnot.AskCellAtRowCol(oldrowTag, columnsTags[i], out oldcellTag[i]);
                theUFSession.Tabnot.AskCellPrefs(oldcellTag[i], out cellPrefs);
                theUFSession.Tabnot.AskCellAtRowCol(rowTag, columnsTags[i], out cellTag[i]);
                theUFSession.Tabnot.SetCellPrefs(cellTag[i], ref cellPrefs);
            }
            theUFSession.Tabnot.SetCellText(cellTag[0], (1).ToString());
            theUFSession.Tabnot.SetCellText(cellTag[1], drawModel.WorkpiecePart.Name);
            theUFSession.Tabnot.SetCellText(cellTag[2], Math.Round(-min.X, 3).ToString());
            theUFSession.Tabnot.SetCellText(cellTag[3], Math.Round(-min.Y, 3).ToString());
            theUFSession.Tabnot.SetCellText(cellTag[4], Math.Round(-min.Z, 3).ToString());
        }
        /// <summary>
        /// 设置层显示
        /// </summary>
        /// <param name="view"></param>
        private void SetViewVisibleLayer(DraftingView view)
        {
            Basic.DrawingUtils.SetLayerHidden(view);
            Basic.DrawingUtils.SetLayerVisible(new int[1] { 201 }, view);
        }
        /// <summary>
        /// 主视图标注
        /// </summary>
        /// <param name="topView"></param>
        /// <param name="originPt"></param>
        private void TopDimension(DraftingView topView, Point3d originPt, double scale)
        {
            string err = "";
            Point3d minPt = drawModel.MinPt.Coordinates;
            Point3d maxPt = drawModel.MaxPt.Coordinates;

            this.workModel.Info.Matr.ApplyPos(ref minPt);
            this.workModel.Info.Matr.ApplyPos(ref maxPt);
            if (!UMathUtils.IsEqual(minPt.X, 0))
            {
                Point3d dimPt = new Point3d(0, 0, 0);
                dimPt.X = originPt.X - this.drawModel.DisPt.X * scale - 10.0;
                dimPt.Y = originPt.Y + this.drawModel.DisPt.Y * scale + 10.0;
                try
                {
                    Basic.DrawingUtils.DimensionHorizontal(topView, dimPt, this.originPoint, drawModel.MinPt, ref err);
                }
                catch (NXException ex)
                {
                    ClassItem.WriteLogFile(this.drawModel.WorkpiecePart.Name + "投影标准错误！         " + ex.Message);
                }
            }
            if (!UMathUtils.IsEqual(minPt.Y, 0))
            {
                Point3d dimPt = new Point3d(0, 0, 0);
                dimPt.X = originPt.X - this.drawModel.DisPt.X * scale - 10.0;
                dimPt.Y = originPt.Y - this.drawModel.DisPt.Y * scale - 10.0;
                try
                {
                    Basic.DrawingUtils.DimensionVertical(topView, dimPt, this.originPoint, drawModel.MinPt, ref err);
                }
                catch (NXException ex)
                {
                    ClassItem.WriteLogFile(this.drawModel.WorkpiecePart.Name + "投影标准错误！         " + ex.Message);
                }

            }
            if (!UMathUtils.IsEqual(maxPt.X, 0))
            {
                Point3d dimPt = new Point3d(0, 0, 0);
                dimPt.X = originPt.X + this.drawModel.DisPt.X * scale + 10.0;
                dimPt.Y = originPt.Y + this.drawModel.DisPt.Y * scale + 10.0;
                try
                {
                    Basic.DrawingUtils.DimensionHorizontal(topView, dimPt, this.originPoint, drawModel.MaxPt, ref err);
                }
                catch (NXException ex)
                {
                    ClassItem.WriteLogFile(this.drawModel.WorkpiecePart.Name + "投影标准错误！         " + ex.Message);
                }

            }
            if (!UMathUtils.IsEqual(maxPt.Y, 0))
            {
                Point3d dimPt = new Point3d(0, 0, 0);
                dimPt.X = originPt.X + this.drawModel.DisPt.X * scale + 10.0;
                dimPt.Y = originPt.Y - this.drawModel.DisPt.Y * scale - 10.0;
                try
                {
                    Basic.DrawingUtils.DimensionVertical(topView, dimPt, this.originPoint, drawModel.MaxPt, ref err);
                }
                catch (NXException ex)
                {
                    ClassItem.WriteLogFile(this.drawModel.WorkpiecePart.Name + "投影标准错误！         " + ex.Message);
                }

            }

        }
        /// <summary>
        /// 投影视图标注
        /// </summary>
        /// <param name="topView"></param>
        /// <param name="originPt"></param>
        private void ProjectedDimension(DraftingView topView, Point3d originPt, double scale)
        {
            string err = "";

            Point3d minPt = drawModel.MinPt.Coordinates;
            Point3d maxPt = drawModel.MaxPt.Coordinates;
            this.workModel.Info.Matr.ApplyPos(ref minPt);
            this.workModel.Info.Matr.ApplyPos(ref maxPt);

            if (!UMathUtils.IsEqual(minPt.Z, 0))
            {
                Point3d dimPt = new Point3d(0, 0, 0);
                dimPt.X = originPt.X - this.drawModel.DisPt.X * scale - 10.0;
                dimPt.Y = originPt.Y - this.drawModel.DisPt.Z * scale - 10.0;
                try
                {
                    Basic.DrawingUtils.DimensionVertical(topView, dimPt, this.originPoint, drawModel.MinPt, ref err);
                }
                catch (NXException ex)
                {
                    ClassItem.WriteLogFile(this.drawModel.WorkpiecePart.Name + "投影标准错误！         " + ex.Message);
                }

            }
            if (!UMathUtils.IsEqual(maxPt.Z, 0))
            {
                Point3d dimPt = new Point3d(0, 0, 0);
                dimPt.X = originPt.X + this.drawModel.DisPt.X * scale + 10.0;
                dimPt.Y = originPt.Y + this.drawModel.DisPt.Z * scale + 10.0;
                try
                {
                    Basic.DrawingUtils.DimensionVertical(topView, dimPt, this.originPoint, drawModel.MaxPt, ref err);
                }
                catch (NXException ex)
                {
                    ClassItem.WriteLogFile(this.drawModel.WorkpiecePart.Name + "投影标准错误！         " + ex.Message);
                }
            }
        }
        /// <summary>
        /// 创建视图
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="originPt"></param>
        /// <param name="tablePath"></param>
        public void CreateView(double scale, Point3d originPt, string tablePath)
        {
            Point3d projectedPt = new Point3d();
            projectedPt.X = originPt.X;
            projectedPt.Y = originPt.Y - this.drawModel.DisPt.Y * scale - this.drawModel.DisPt.Z * scale - 30;
            double[] tableOrigin = new double[3] { originPt.X - 35, projectedPt.Y - this.drawModel.DisPt.Z * scale - 15, 0 };
            List<NXOpen.Assemblies.Component> allComp = new List<NXOpen.Assemblies.Component>();
            allComp.AddRange(workDra.HostComp);
            allComp.AddRange(workDra.OtherComp);
            List<NXOpen.Assemblies.Component> hiddenComd = this.drawModel.GetHiddenObjects(allComp);
            hiddenComd.AddRange(workDra.GetEleComp());
            try
            {

                DraftingView topView = Basic.DrawingUtils.CreateView("top", originPt, scale, this.workModel.Info.Matr, hiddenComd.ToArray());

                DraftingView proView = Basic.DrawingUtils.CreateProjectedView(topView, projectedPt, scale);

                SetViewVisibleLayer(topView);
                SetViewVisibleLayer(proView);
                TopDimension(topView, originPt, scale);
                ProjectedDimension(proView, projectedPt, scale);
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile(this.drawModel.WorkpiecePart.Name + "创建视图！         " + ex.Message);
            }

            SetTable(tablePath, tableOrigin);
        }
    }
}
