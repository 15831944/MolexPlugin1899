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
    /// 二次开粗
    /// </summary>
    public class TwiceRoughCreateOperation : AbstractCreateOperation
    {
        public string ReferenceTool { get; private set; } = "";
        public TwiceRoughCreateOperation(int site, string tool) : base(site, tool)
        {
            this.Type = ElectrodeOperationType.TwiceRough;
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

                NCGroup toolGroup = template.FindTool(ReferenceTool);
                if (toolGroup == null)
                {
                    err.Add("设置参考刀具错误！           " + "无法在模板中找到参考刀具");
                }
                else
                {
                    (this.operModel as CavityMillingModel).SetReferenceTool(toolGroup as Tool);
                }
            }
            catch (NXException ex)
            {
                err.Add("设置参考刀具错误！           " + ex.Message);
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

        /// <summary>
        /// 设置参考刀具
        /// </summary>
        /// <param name="toolName"></param>
        public void SetReferencetool(string toolName)
        {
            this.ReferenceTool = toolName;
        }

        public override void CreateOperationName(int programNumber)
        {
            string preName = "O" + string.Format("{0:D4}", site);
            this.nameModel = ElectrodeCAMNameTemplate.AskOperationNameModelOfTwiceRough(preName, this.toolName, programNumber);
        }

        public override AbstractCreateOperation CopyOperation(int programNumber)
        {
            TwiceRoughCreateOperation to = new TwiceRoughCreateOperation(this.site, this.toolName);
            to.CreateOperationName(programNumber);
            to.SetReferencetool(this.ReferenceTool);
            return to;
        }

        public override void SetOperationData(AbstractElectrodeCAM eleCam)
        {
            this.Inter = eleCam.Inter;
        }
    }
}
