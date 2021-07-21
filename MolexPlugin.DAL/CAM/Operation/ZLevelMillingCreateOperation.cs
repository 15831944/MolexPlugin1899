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
    /// 等高
    /// </summary>
    public class ZLevelMillingCreateOperation : AbstractCreateOperation
    {
        private List<Face> Faces = new List<Face>();
        private Point3d levelPoint = new Point3d();
        private bool steep;
        private bool isAll;
        public ZLevelMillingCreateOperation(int site, string tool) : base(site, tool)
        {
            this.Type = ElectrodeOperationType.ZLevelMilling;
        }
        public override List<string> CreateOperation()
        {
            List<string> err = new List<string>();
            this.template = new ElectrodeCAMTemplateModel();
            if (this.nameModel == null)
                throw new Exception("请现创建刀具路径所需要的路径！");
            try
            {
                this.operModel = ElectrodeOperationTemplate.CreateOperationOfZLevelMilling(this.nameModel, template);
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
                    (this.operModel as ZLevelMillingModel).SetGeometry(Faces.ToArray());
                }
                catch (NXException ex)
                {
                    err.Add("设置加工面错误！           " + ex.Message);
                }
            }

            if (UMathUtils.IsEqual(levelPoint, new Point3d()))
            {
                try
                {
                    (this.operModel as ZLevelMillingModel).SetCutLevel(levelPoint);
                }
                catch (NXException ex)
                {
                    err.Add("设置加工深度错误！           " + ex.Message);
                }
            }

            if (steep)
            {
                try
                {
                    (this.operModel as ZLevelMillingModel).SetSteep();
                }
                catch (NXException ex)
                {
                    err.Add("设置加工陡峭角错误！           " + ex.Message);
                }
            }
            try
            {
                this.operModel.SetStock(-this.Inter, -this.Inter);
            }
            catch (NXException ex)
            {
                err.Add("设置加工余量错误！           " + ex.Message);
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
        /// 设置切削层底
        /// </summary>
        /// <param name="level"></param>
        public void SetCutLevel(Point3d level)
        {
            this.levelPoint = level;
        }
        public override void CreateOperationName(int programNumber)
        {
            string preName = "O" + string.Format("{0:D4}", site);
            this.nameModel = ElectrodeCAMNameTemplate.AskOperationNameModelOfZLevelMilling(preName, this.toolName, programNumber);
        }

        public override AbstractCreateOperation CopyOperation(int programNumber)
        {
            ZLevelMillingCreateOperation zo = new ZLevelMillingCreateOperation(this.site, this.toolName);
            zo.CreateOperationName(programNumber);
            // zo.SetFaces(this.faces.ToArray());
            //  zo.SetCutLevel(this.level);
            return zo;
        }
        /// <summary>
        /// 设置陡峭角
        /// </summary>
        /// <param name="steep"></param>
        public void SetSteep(bool steep, bool isAll)
        {
            this.steep = steep;
            this.isAll = isAll;
        }
        public override void SetOperationData(AbstractElectrodeCAM eleCam)
        {
            Dictionary<double, Face[]> slope = new Dictionary<double, Face[]>();
            if ((steep && isAll) || (!steep && isAll))
            {
                slope = eleCam.GetAllFaces();
            }
            else if (steep && !isAll)
            {
                slope = eleCam.GetSteepFaces();
            }
            else if (!steep && !isAll)
            {
                slope = eleCam.GetSlopeFaces();
            }

            slope.OrderBy(a => a.Key);
            for (int k = 0; k < slope.Count; k++)
            {
                if (k == 0)
                {
                    this.Inter = slope.Keys.ToArray()[k];
                    this.SetFaces(slope[this.Inter]);
                    this.SetCutLevel(eleCam.Analysis.BaseFace.BoxMinCorner);
                }
                else
                {
                    AbstractCreateOperation oper = this.CopyOperation(99);
                    oper.Inter = slope.Keys.ToArray()[k];
                    (oper as ZLevelMillingCreateOperation).SetFaces(slope[this.Inter]);
                    (oper as ZLevelMillingCreateOperation).SetCutLevel(eleCam.Analysis.BaseFace.BoxMinCorner);
                }
            }
        }

        public override List<string> GetAllToolName()
        {
            return GetToolDat("FinishZLevelTool");
        }

        public override List<string> GetRefToolName()
        {
            return new List<string>();
        }
    }
}
