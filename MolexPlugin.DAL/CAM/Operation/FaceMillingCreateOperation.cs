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
    /// 光平面
    /// </summary>
    public class FaceMillingCreateOperation : AbstractCreateOperation
    {
        private List<Face> Conditions = new List<Face>();
        public FaceMillingCreateOperation(int site, string tool) : base(site, tool)
        {
            this.Type = ElectrodeOperationType.FaceMilling;
        }
        public override List<string> CreateOperation()
        {
            List<string> err = new List<string>();
            this.template = new ElectrodeCAMTemplateModel();
            if (this.nameModel == null)
                throw new Exception("请现创建刀具路径所需要的路径！");
            try
            {
                this.operModel = ElectrodeOperationTemplate.CreateOperationOfFaceMilling(this.nameModel, template);
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

            if (Conditions.Count != 0)
            {
                try
                {
                    (this.operModel as FaceMillingModel).SetBoundary(Conditions.ToArray());
                }
                catch (NXException ex)
                {
                    err.Add("设置边界错误！           " + ex.Message);
                }

            }
            try
            {
                this.operModel.SetStock(0.05, -this.Inter);
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
        public void SetBoundary(params Face[] conditions)
        {
            this.Conditions.Clear();
            this.Conditions = conditions.ToList();
        }

        public override void CreateOperationName(int programNumber)
        {
            string preName = "O" + string.Format("{0:D4}", site);
            this.nameModel = ElectrodeCAMNameTemplate.AskOperationNameModelOfFaceMilling(preName, this.toolName, programNumber);
        }

        public override AbstractCreateOperation CopyOperation(int programNumber)
        {
            FaceMillingCreateOperation fo = new FaceMillingCreateOperation(this.site, this.toolName);
            fo.CreateOperationName(programNumber);
            return fo;
        }

        public override void SetOperationData(AbstractElectrodeCAM eleCam)
        {
            Dictionary<double, Face[]> plane = eleCam.GetPlaneFaces();
            plane.OrderBy(a => a.Key);
            for (int k = 0; k < plane.Count; k++)
            {
                if (k == 0)
                {
                    this.Inter = plane.Keys.ToArray()[k];
                    this.SetBoundary(plane[this.Inter]);
                }
                else
                {
                    AbstractCreateOperation oper = this.CopyOperation(99);
                    oper.Inter = plane.Keys.ToArray()[k];
                    (oper as FaceMillingCreateOperation).SetBoundary(plane[this.Inter]);
                }
            }
        }
    }
}
