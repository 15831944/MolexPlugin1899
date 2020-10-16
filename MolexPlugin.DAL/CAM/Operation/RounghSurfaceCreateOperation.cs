using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.CAM;
using Basic;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 固定轮廓铣
    /// </summary>
    public class RounghSurfaceCreateOperation : AbstractCreateOperation
    {
        private List<Face> Faces = new List<Face>();
        public RounghSurfaceCreateOperation(int site, string tool) : base(site, tool)
        {
            this.Type = ElectrodeOperationType.RounghSurface;
        }
        public override List<string> CreateOperation()
        {
            List<string> err = new List<string>();
            this.template = new ElectrodeCAMTemplateModel();
            if (this.nameModel == null)
                throw new Exception("请现创建刀具路径所需要的路径！");
            try
            {
                this.operModel = ElectrodeOperationTemplate.CreateOperationOfSurfaceContour(this.nameModel, template);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            try
            {
                this.operModel.Create(this.nameModel.OperName);

            }
            catch (NXException ex)
            {
                throw ex;
            }
            if (Faces.Count > 0)
            {
                try
                {
                    (this.operModel as SurfaceContourModel).SetGeometry(Faces.ToArray());
                    (this.operModel as SurfaceContourModel).SetDriveMethod(SurfaceContourBuilder.DriveMethodTypes.AreaMilling);
                }
                catch (NXException ex)
                {
                    err.Add("设置加工面错误！           " + ex.Message);
                }
            }
            try
            {
                (this.operModel as SurfaceContourModel).SetDmarea(0.15);
            }
            catch (NXException ex)
            {
                err.Add("设置部距错误！           " + ex.Message);
            }
            try
            {
                if ((0.05 - this.Inter) > 0)
                {

                    this.operModel.SetStock(0.05 - this.Inter, 0.05 - this.Inter);
                }
                else
                {
                    this.operModel.SetStock(0, 0);
                }
            }
            catch (NXException ex)
            {
                err.Add("设置余量错误！            " + ex.Message);
            }

            return err;
        }
        /// <summary>
        /// 设置边界
        /// </summary>
        /// <param name="floorPt"></param>
        /// <param name="conditions"></param>
        public void SetFaces(params Face[] faces)
        {
            this.Faces.Clear();
            this.Faces = faces.ToList();
        }
        public override void CreateOperationName(int programNumber)
        {
            string preName = "O" + string.Format("{0:D4}", site);
            this.nameModel = ElectrodeCAMNameTemplate.AskOperationNameModelOfSurfaceContour(preName, this.toolName, programNumber);
        }

        public override AbstractCreateOperation CopyOperation(int programNumber)
        {
            FlowCutCreateOperation fo = new FlowCutCreateOperation(this.site, this.toolName);
            fo.CreateOperationName(programNumber);
            return fo;
        }

        public override void SetOperationData(AbstractElectrodeCAM eleCam)
        {
            double minDia;
            this.Inter = eleCam.Inter;
            eleCam.Analysis.GetFlatFaces(out this.Faces, out minDia);
        }

        public override List<string> GetAllToolName()
        {
            return GetToolDat("RoughPlaneTool");
        }

        public override List<string> GetRefToolName()
        {
            return new List<string>();
        }
    }
}
