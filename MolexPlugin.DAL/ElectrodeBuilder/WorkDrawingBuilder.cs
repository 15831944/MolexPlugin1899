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
    public class WorkDrawingBuilder
    {
        private WorkModel work;
        private WorkDrawingModel workDra;
        private string dllPath;
        private string workpieceTablePath;
        private string plistPath;
        private string workpieceDrawTemplate;
        private Point originPoint;
        private WorkpieceDrawingModel hostDraw;

        public WorkDrawingBuilder(WorkModel model)
        {
            this.work = model;

            Part workPart = Session.GetSession().Parts.Work;
            if (!model.PartTag.Equals(workPart))
            {
                PartUtils.SetPartDisplay(this.work.PartTag);
            }

            this.workDra = new WorkDrawingModel(model);
            hostDraw = workDra.GetHostWorkpieceDrawingModel();
            GetTemplate();
        }
        public void CreateDrawing()
        {
            double scale = hostDraw.GetScale(120, 140);
            this.originPoint = PointUtils.CreatePointFeature(this.work.Info.Matr.GetCenter());
            this.workDra.CreateCenterLine(hostDraw.CenterPt, hostDraw.DisPt);
            HostWorkpieceDrawing(scale);
            OtherWorkpieceDrawing(scale);
            LayerUtils.SetLayerSelectable(false, 201);
        }
        /// <summary> 
        /// 获取模板位置
        /// </summary>
        private void GetTemplate()
        {
            dllPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            workpieceTablePath = dllPath.Replace("application\\", "part\\workpieceTable.prt");
            plistPath = dllPath.Replace("application\\", "part\\eleTable.prt");
            workpieceDrawTemplate = dllPath.Replace("application\\", "part\\EDM.prt");
        }

        /// <summary>
        /// 主工件图
        /// </summary>
        /// <param name="workpiecePart"></param>
        private void HostWorkpieceDrawing(double scale)
        {

            WorkpieceDrawing dra = new WorkpieceDrawing(hostDraw, work, workDra, this.originPoint);
            NXOpen.Drawings.DrawingSheet sheet = Basic.DrawingUtils.DrawingSheetByName(workDra.HostComp[0].Name);
            if (sheet != null)
            {
                DeleteObject.Delete(sheet);
            }
            try
            {
                sheet = Basic.DrawingUtils.DrawingSheet(workpieceDrawTemplate, 297, 420, workDra.HostComp[0].Name);
                dra.CreateView(scale, GetFirstPoint(hostDraw, scale), this.workpieceTablePath);
                double[] plistOrigin = { 20, 70, 0 };
                Basic.DrawingUtils.CreatePlist(plistPath, plistOrigin);
                Basic.DrawingUtils.UpdateViews(sheet);
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile(hostDraw.WorkpiecePart.Name + "创建图纸错误！         " + ex.Message);
                return;
            }


        }
        /// <summary>
        /// 创建一张其他工件图
        /// </summary>
        /// <param name="other"></param>
        /// <param name="scale"></param>
        private void OtherWorkpieceView(List<WorkpieceDrawingModel> other, double scale)
        {

            Point3d firstPt = GetFirstPoint(other[0], scale);
            double length = 0;
            foreach (WorkpieceDrawingModel wk in other)
            {
                length += 2 * wk.DisPt.X * scale;
            }
            NXOpen.Drawings.DrawingSheet sheet = Basic.DrawingUtils.DrawingSheetByName(other[0].WorkpiecePart.Name);
            if (sheet != null)
            {
                DeleteObject.Delete(sheet);
            }
            int k = 0;
            if (other.Count == 1)
            {
                k = 0;
            }
            else
            {
                k = (int)Math.Floor(300 - length) / (other.Count - 1);
            }
            sheet = Basic.DrawingUtils.DrawingSheet(workpieceDrawTemplate, 297, 420, other[0].WorkpiecePart.Name);
            for (int i = 0; i < other.Count; i++)
            {
                Point3d temp;
                if (i == 0)
                    temp = firstPt;
                else
                {
                    double x = other[i - 1].DisPt.X * scale + other[i].DisPt.X * scale + i * k;
                    temp = new Point3d(firstPt.X + x, firstPt.Y, firstPt.Z);
                }
                WorkpieceDrawing wd = new WorkpieceDrawing(other[i], this.work, workDra, this.originPoint);
                wd.CreateView(scale, temp, this.workpieceTablePath);
            }
            Basic.DrawingUtils.UpdateViews(sheet);
        }
        /// <summary>
        /// 创建全部其他工件图
        /// </summary>
        /// <param name="scale"></param>
        private void OtherWorkpieceDrawing(double scale)
        {
            int count = (int)Math.Floor(340 / (2 * hostDraw.DisPt.X * scale + 40));
            List<WorkpieceDrawingModel> other = this.workDra.GetOtherWorkpieceDrawingModel();
            int temp = 0;
            for (int i = 0; i < other.Count; i++)
            {
                List<WorkpieceDrawingModel> infos = new List<WorkpieceDrawingModel>();
                for (int k = 0; k < count; k++)
                {
                    if (k + i < other.Count)
                    {
                        infos.Add(other[k + i]);
                    }
                    else
                    {
                        break;
                    }
                }
                temp++;
                OtherWorkpieceView(infos, scale);
                i = temp * count - 1;
            }
        }
        /// <summary>
        /// 获取第一个设定点
        /// </summary>
        /// <param name="info"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        private Point3d GetFirstPoint(WorkpieceDrawingModel dram, double scale)
        {
            Point3d pt = new Point3d(0, 0, 0);
            pt.X = 50 + (dram.DisPt.X) * scale;
            pt.Y = 250 - (dram.DisPt.Y) * scale;
            return pt;
        }

    }
}
