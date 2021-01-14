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
    /// 光基准框
    /// </summary>
    public class BaseStationCreateOperation : AbstractCreateOperation
    {
        private Point3d floorPt;
        private List<BoundaryModel> conditions = new List<BoundaryModel>();
        public BaseStationCreateOperation(int site, string tool) : base(site, tool)
        {
            this.Type = ElectrodeOperationType.BaseStation;
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
                this.operModel.SetStock(-this.Inter, 0.05);
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
        public void SetBoundary(Point3d floorPt, params BoundaryModel[] conditions)
        {
            this.floorPt = floorPt;
            this.conditions.Clear();
            this.conditions = conditions.ToList();
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
            //this.Inter = eleCam.Inter;
            this.Inter = 0;
            Point3d floorPt;
            BoundaryModel conditions;
            eleCam.GetBaseStationBoundary(out conditions, out floorPt);
            SetBoundary(floorPt, conditions);
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
