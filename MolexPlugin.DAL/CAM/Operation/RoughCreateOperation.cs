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
    /// 开粗
    /// </summary>
    public class RoughCreateOperation : AbstractCreateOperation
    {
        public RoughCreateOperation(int site, string tool) : base(site, tool)
        {
            this.Type = ElectrodeOperationType.RoughCreate;
        }

        public override List<string> CreateOperation()
        {
            List<string> err = new List<string>();
            this.template = new ElectrodeCAMTemplateModel();
            if (this.nameModel == null)
                throw new Exception("请现创建刀具路径所需要的路径！");
            try
            {
                this.operModel = ElectrodeOperationTemplate.CreateOperationOfCavityMilling(this.nameModel, template);
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
            try
            {
                if ((0.05 - this.Inter) > 0)
                {
                    if (0.03 - this.Inter > 0)
                        this.operModel.SetStock(0.05 - this.Inter, 0.03 - this.Inter);
                    else
                        this.operModel.SetStock(0.05 - this.Inter, 0);
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

        public override void CreateOperationName(int programNumber)
        {
            string preName = "O" + string.Format("{0:D4}", site);
            this.nameModel = ElectrodeCAMNameTemplate.AskOperationNameModelOfRough(preName, this.toolName, programNumber);
        }

        public override AbstractCreateOperation CopyOperation(int programNumber)
        {
            RoughCreateOperation ro = new RoughCreateOperation(this.site, this.toolName);
            ro.CreateOperationName(programNumber);
            return ro;
        }

        public override void SetOperationData(AbstractElectrodeCAM eleCam)
        {
            this.Inter = eleCam.Inter;
        }

        public override List<string> GetAllToolName()
        {
            return this.GetToolDat("RoughPlaneTool");
        }

        public override List<string> GetRefToolName()
        {
            return new List<string>();
        }
    }
}
