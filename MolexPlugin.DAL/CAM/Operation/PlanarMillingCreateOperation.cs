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
    /// 2D 光侧面
    /// </summary>
    public class PlanarMillingCreateOperation : AbstractCreateOperation
    {
        private Point3d floorPt = new Point3d(0, 0, 0);
        private List<BoundaryModel> conditions = new List<BoundaryModel>();
        private bool burring;
        public PlanarMillingCreateOperation(int site, string tool) : base(site, tool)
        {
            this.Type = ElectrodeOperationType.PlanarMilling;
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
            if (conditions.Count != 0)
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
            if (burring)
            {
                try
                {
                    (this.operModel as PlanarMillingModel).SetBurringDepth();
                }
                catch (NXException ex)
                {
                    err.Add("设置下刀量错误！           " + ex.Message);
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
            this.nameModel = ElectrodeCAMNameTemplate.AskOperationNameModelOfPlanarMilling(preName, this.toolName, programNumber);
        }
        /// <summary>
        /// 设置去毛刺下刀量
        /// </summary>
        /// <param name="burring"></param>
        public void SetBurringBool(bool burring)
        {
            this.burring = burring;
        }

        public override AbstractCreateOperation CopyOperation(int programNumber)
        {
            PlanarMillingCreateOperation po = new PlanarMillingCreateOperation(this.site, this.toolName);
            po.CreateOperationName(programNumber);
            return po;
        }

        public override void SetOperationData(AbstractElectrodeCAM eleCam)
        {
            Dictionary<double, BoundaryModel[]> boun = eleCam.GetBaseFaceBoundary();
            boun.OrderBy(a => a.Key);
            for (int k = 0; k < boun.Count; k++)
            {
                if (k == 0)
                {
                    this.Inter = boun.Keys.ToArray()[k];
                    this.SetBoundary(eleCam.Analysis.BaseFace.BoxMinCorner, boun[this.Inter]);
                }
                else
                {
                    AbstractCreateOperation oper = this.CopyOperation(99);
                    oper.Inter = boun.Keys.ToArray()[k];
                    (oper as PlanarMillingCreateOperation).SetBoundary(eleCam.Analysis.BaseFace.BoxMinCorner, boun[this.Inter]);
                }
            }
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
