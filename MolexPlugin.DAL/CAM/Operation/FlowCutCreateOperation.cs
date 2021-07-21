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
    /// 清根
    /// </summary>
    public class FlowCutCreateOperation : AbstractCreateOperation
    {
        private List<Face> Faces = new List<Face>();

        public string ReferencetoolName { get; private set; } = "";
        public FlowCutCreateOperation(int site, string tool) : base(site, tool)
        {
            this.ReferencetoolName = "BN0.98";
            this.Type = ElectrodeOperationType.FlowCut;
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
                }
                catch (NXException ex)
                {
                    err.Add("设置加工错误！           " + ex.Message);
                }
            }

            if (ReferencetoolName != "")
            {
                try
                {
                    NCGroup toolGroup = template.FindTool(ReferencetoolName);
                    if (toolGroup == null)
                    {
                        err.Add("设置参考刀具错误！           " + "无法在模板中找到参考刀具");
                    }
                    else
                    {
                        Tool tool = toolGroup as Tool;
                        (this.operModel as SurfaceContourModel).SetReferenceTool(tool);
                    }
                }
                catch (NXException ex)
                {
                    err.Add("设置参考刀具错误！           " + ex.Message);
                }

            }
            try
            {
                this.operModel.SetStock(-this.Inter, -this.Inter);
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
        /// <summary>
        /// 设置参考刀具
        /// </summary>
        /// <param name="toolName"></param>
        public void SetReferencetool(string toolName)
        {
            this.ReferencetoolName = toolName;
        }
        public override void CreateOperationName(int programNumber)
        {
            string preName = "O" + string.Format("{0:D4}", site);
            this.nameModel = ElectrodeCAMNameTemplate.AskOperationNameModelOfFlowCut(preName, this.toolName, programNumber);
        }

        public override AbstractCreateOperation CopyOperation(int programNumber)
        {
            FlowCutCreateOperation fo = new FlowCutCreateOperation(this.site, this.toolName);
            fo.CreateOperationName(programNumber);
            return fo;
        }

        public override void SetOperationData(AbstractElectrodeCAM eleCam)
        {
            Dictionary<double, Face[]> slope = eleCam.GetSlopeFaces();
            slope.OrderBy(a => a.Key);
            for (int k = 0; k < slope.Count; k++)
            {
                if (k == 0)
                {
                    this.Inter = slope.Keys.ToArray()[k];
                    this.SetFaces(slope[this.Inter]);
                }
                else
                {
                    AbstractCreateOperation oper = this.CopyOperation(99);
                    oper.Inter = slope.Keys.ToArray()[k];
                    (oper as FlowCutCreateOperation).SetFaces(slope[this.Inter]);
                }
            }
        }

        public override List<string> GetAllToolName()
        {
            return this.GetToolDat("FinishBallTool");
        }

        public override List<string> GetRefToolName()
        {
            return this.GetToolDat("FinishBallTool");
        }
    }
}
