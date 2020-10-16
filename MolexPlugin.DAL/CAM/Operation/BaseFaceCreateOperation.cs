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
    /// 光基准面
    /// </summary>
    public class BaseFaceCreateOperation : AbstractCreateOperation
    {
        private Point3d floorPt;
        private List<BoundaryModel> conditions = new List<BoundaryModel>();
        public BaseFaceCreateOperation(int site, string tool) : base(site, tool)
        {
            this.Type = ElectrodeOperationType.BaseFace;
        }
        public override List<string> CreateOperation()
        {
            List<string> err = new List<string>();
            this.template = new ElectrodeCAMTemplateModel();
            if (this.nameModel == null)
                throw new Exception("请现创建刀具路径所需要的路径！");
            try
            {
                this.operModel = ElectrodeOperationTemplate.CreateOperationOfPlanarMilling(this.nameModel, template);
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
            if (conditions.Count > 0)
            {
                try
                {
                    (this.operModel as PlanarMillingModel).SetBoundary(floorPt, conditions.ToArray());
                }
                catch (NXException ex)
                {
                    err.Add("设置边界错误！           " + ex.Message);
                }
            }
            try
            {
                (this.operModel as PlanarMillingModel).SetDepth(0);
                this.operModel.SetStock(-this.Inter, 0);
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
        public void SetBoundary(params Line[] lines)
        {
            this.floorPt = lines[0].StartPoint;
            this.conditions.Clear();
            foreach (Line le in lines)
            {
                List<NXObject> line = new List<NXObject>();
                line.Add(le);
                BoundaryModel boundry = new BoundaryModel()
                {
                    BouudaryPt = le.StartPoint,
                    Curves = line,
                    PlaneTypes = BoundarySet.PlaneTypes.UserDefined,
                    ToolSide = BoundarySet.ToolSideTypes.InsideOrLeft,
                    Types = BoundarySet.BoundaryTypes.Open
                };
                this.conditions.Add(boundry);
            }

        }
        public override void CreateOperationName(int programNumber)
        {
            string preName = "O" + string.Format("{0:D4}", site);
            this.nameModel = ElectrodeCAMNameTemplate.AskOperationNameModelOfBaseStation(preName, this.toolName, programNumber);
        }

        public override AbstractCreateOperation CopyOperation(int programNumber)
        {
            BaseStationCreateOperation ao = new BaseStationCreateOperation(this.site, this.toolName);
            ao.CreateOperationName(programNumber);
            return ao;
        }

        public override void SetOperationData(AbstractElectrodeCAM eleCam)
        {
            this.Inter = eleCam.Inter;
            Line[] lines = eleCam.GetBaseFaceLine();
            if (lines == null || lines.Length == 0)
            {

            }
            else
                SetBoundary(lines);
        }

        public object Clone()
        {
            AbstractCreateOperation ao = new BaseFaceCreateOperation(this.site, this.toolName);
            ao.CreateOperationName(1);
            return ao;

        }

        public override List<string> GetAllToolName()
        {
            return this.GetToolDat("FinishPlaneTool");
        }

        public override List<string> GetRefToolName()
        {
            return new List<string>();
        }
    }
}
