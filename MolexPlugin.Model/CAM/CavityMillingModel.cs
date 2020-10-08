using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basic;
using NXOpen;
using NXOpen.CAM;

namespace MolexPlugin.Model
{
    /// <summary>
    /// 型腔铣
    /// </summary>
    public class CavityMillingModel : AbstractOperationModel
    {
        private string templateOperName;

        public CavityMillingModel(NXOpen.CAM.Operation oper) : base(oper)
        {

        }
        public CavityMillingModel(NCGroupModel model, string templateName, string templateOperName) : base(model, templateName)
        {
            this.templateOperName = templateOperName;
        }

        public override void Create(string name)
        {
            base.CreateOperation(this.templateOperName, name, this.GroupModel);
            NXOpen.CAM.CavityMillingBuilder builder1 = null;
            try
            {
                builder1 = workPart.CAMSetup.CAMOperationCollection.CreateCavityMillingBuilder(this.Oper);
                builder1.FeedsBuilder.SetMachiningData();
                builder1.Commit();
            }
            catch (NXException ex)
            {
                throw ex;
            }
            finally
            {
                if (builder1 != null)
                    builder1.Destroy();
            }

        }

        public override OperationData GetOperationData()
        {
            OperationData data = base.GetOperationData();
            NXOpen.CAM.CavityMillingBuilder operBuilder = null;
            try
            {
                operBuilder = workPart.CAMSetup.CAMOperationCollection.CreateCavityMillingBuilder(this.Oper);
                if (operBuilder.CutParameters.FloorSameAsPartStock)
                {
                    data.FloorStock = operBuilder.CutParameters.PartStock.Value;
                    data.SideStock = operBuilder.CutParameters.PartStock.Value;
                }
                else
                {
                    data.SideStock = operBuilder.CutParameters.PartStock.Value;
                    data.FloorStock = operBuilder.CutParameters.FloorStock.Value;
                }
                data.Depth = operBuilder.CutLevel.GlobalDepthPerCut.DistanceBuilder.Value;
                if (operBuilder.BndStepover.DistanceBuilder.Intent == NXOpen.CAM.ParamValueIntent.ToolDep)
                {
                    double toolDia = data.Tool.ToolDia;
                    double dep = operBuilder.BndStepover.DistanceBuilder.Value;
                    data.Stepover = toolDia * dep / 100;
                }
                else
                {
                    data.Stepover = operBuilder.BndStepover.DistanceBuilder.Value;
                }

                data.Speed = operBuilder.FeedsBuilder.SpindleRpmBuilder.Value;
                data.Feed = operBuilder.FeedsBuilder.FeedCutBuilder.Value;
                return data;
            }
            catch (NXException ex)
            {
                throw ex;
            }
            finally
            {
                if (operBuilder != null)
                    operBuilder.Destroy();
            }


        }
        /// <summary>
        /// 设置区域起点
        /// </summary>
        /// <param name="pt"></param>
        public override void SetRegionStartPoints(params Point3d[] pt)
        {
            NXOpen.CAM.CavityMillingBuilder builder1;
            try
            {
                builder1 = workPart.CAMSetup.CAMOperationCollection.CreateCavityMillingBuilder(this.Oper);
                List<Point> point = new List<Point>();
                foreach (Point3d p in pt)
                {
                    point.Add(workPart.Points.CreatePoint(p));
                }
                builder1.NonCuttingBuilder.SetRegionStartPoints(point.ToArray());
                NXOpen.NXObject nXObject1;
                nXObject1 = builder1.Commit();
                builder1.Destroy();
            }
            catch (NXException ex)
            {
                throw ex;
            }

        }
        /// <summary>
        /// 设置余量
        /// </summary>
        /// <param name="partStock">部件余量</param>
        /// <param name="floorStock">底部余量</param>
        public override void SetStock(double partStock, double floorStock)
        {
            NXOpen.CAM.CavityMillingBuilder builder1;
            try
            {
                builder1 = workPart.CAMSetup.CAMOperationCollection.CreateCavityMillingBuilder(this.Oper);
                builder1.CutParameters.PartStock.Value = partStock;
                builder1.CutParameters.FloorStock.Value = floorStock;
                NXOpen.NXObject nXObject1;
                nXObject1 = builder1.Commit();
                builder1.Destroy();
            }
            catch (NXException ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 设置参考刀具
        /// </summary>
        /// <param name="tool"></param>
        public void SetReferenceTool(Tool tool)
        {
            NXOpen.CAM.CavityMillingBuilder builder1;
            try
            {
                builder1 = workPart.CAMSetup.CAMOperationCollection.CreateCavityMillingBuilder(this.Oper);
                builder1.ReferenceTool = tool;
                NXOpen.NXObject nXObject1;
                nXObject1 = builder1.Commit();
                builder1.Destroy();
            }
            catch (NXException ex)
            {
                throw ex;
            }
        }
    }

}

