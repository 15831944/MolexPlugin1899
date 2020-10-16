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
    public class SurfaceContourCreateOperation : AbstractCreateOperation
    {
        private List<Face> Faces = new List<Face>();
        private bool flat;
        private bool isAll;
        public SurfaceContourCreateOperation(int site, string tool) : base(site, tool)
        {
            this.Type = ElectrodeOperationType.SurfaceContour;
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
                    err.Add("设置加工错误！           " + ex.Message);
                }
            }
            if(this.flat)
            {
                try
                {
                    (this.operModel as SurfaceContourModel).SetSteep(60.0);
                }
                catch (NXException ex)
                {
                    err.Add("设置陡峭角错误！           " + ex.Message);
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
        /// <summary>
        /// 设置陡峭角
        /// </summary>
        /// <param name="flat"></param>
        ///  /// <param name="isAll"></param>
        public void SetFlat(bool flat, bool isAll)
        {
            this.flat = flat;
            this.isAll = isAll;
        }
        public override void SetOperationData(AbstractElectrodeCAM eleCam)
        {
            Dictionary<double, Face[]> slope = new Dictionary<double, Face[]>();
            if (flat && isAll)
            {
                slope = eleCam.GetAllFaces();
            }
            else if (flat)
            {
                slope = eleCam.GetSlopeFaces();
            }
            else
            {
                slope = eleCam.GetFlatFaces();
            }
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
            return GetToolDat("FinishBallTool");
        }

        public override List<string> GetRefToolName()
        {
            return new List<string>();
        }
    }
}
