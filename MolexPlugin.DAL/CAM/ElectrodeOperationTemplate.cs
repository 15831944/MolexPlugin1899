using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.CAM;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 电极模板
    /// </summary>
    public class ElectrodeOperationTemplate
    {

        private static NCGroupModel GetNcGroupModelOfName(OperationNameModel model, ElectrodeCAMTemplateModel template)
        {
            Part workPart = Session.GetSession().Parts.Work;
            NCGroupModel group = new NCGroupModel()
            {

                GeometryGroup = template.FindGeometry("WORKPIECE"),
                MethodGroup = template.MethodGroup.Find(a => a.Name.ToUpper().Equals("CU")),
                ToolGroup = template.FindTool(model.ToolName)
            };
            if (group.GeometryGroup == null)
                throw new Exception("无法获取加工模板WORKPIECE！");
            if (group.MethodGroup == null)
                throw new Exception("无法获取加工模板加工方法！");
            if (group.ToolGroup == null)
                throw new Exception("无法获取加工模板刀具！");
            NCGroup temp = template.FindProgram(model.ProgramName);
            if (temp == null)
            {
                NXOpen.CAM.NCGroup nCGroup1 = (NXOpen.CAM.NCGroup)workPart.CAMSetup.CAMGroupCollection.FindObject("AAA");
                NXOpen.CAM.NCGroup nCGroup2 = workPart.CAMSetup.CAMGroupCollection.CreateProgram(nCGroup1, "electrode", "AAA_1", NXOpen.CAM.NCGroupCollection.UseDefaultName.True, model.ProgramName);
                group.ProgramGroup = nCGroup2;
            }
            else
                group.ProgramGroup = temp;
            return group;
        }

        /// <summary>
        /// 创建型腔铣
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cam"></param>
        /// <returns></returns>
        public static AbstractOperationModel CreateOperationOfCavityMilling(OperationNameModel model, ElectrodeCAMTemplateModel template)
        {
            return new CavityMillingModel(GetNcGroupModelOfName(model, template), model.templateName, model.templateOperName);
        }

        /// <summary>
        /// 创建面铣
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cam"></param>
        /// <returns></returns>
        public static AbstractOperationModel CreateOperationOfFaceMilling(OperationNameModel model, ElectrodeCAMTemplateModel template)
        {
            NCGroupModel group = GetNcGroupModelOfName(model, template);
            return new FaceMillingModel(group, model.templateName, model.templateOperName);
        }

        /// <summary>
        /// 创建平面铣
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cam"></param>
        /// <returns></returns>
        public static AbstractOperationModel CreateOperationOfPlanarMilling(OperationNameModel model, ElectrodeCAMTemplateModel template)
        {
            NCGroupModel group = GetNcGroupModelOfName(model, template);
            return new PlanarMillingModel(group, model.templateName, model.templateOperName);
        }

        /// <summary>
        /// 创建固定轮廓铣
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cam"></param>
        /// <returns></returns>
        public static AbstractOperationModel CreateOperationOfSurfaceContour(OperationNameModel model, ElectrodeCAMTemplateModel template)
        {
            NCGroupModel group = GetNcGroupModelOfName(model, template);
            SurfaceContourModel sur = new SurfaceContourModel(group, model.templateName, model.templateOperName);
            return sur;
        }

        /// <summary>
        /// 创建等高铣
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cam"></param>
        /// <returns></returns>
        public static AbstractOperationModel CreateOperationOfZLevelMilling(OperationNameModel model, ElectrodeCAMTemplateModel template)
        {
            NCGroupModel group = GetNcGroupModelOfName(model, template);
            return new ZLevelMillingModel(group, model.templateName, model.templateOperName);
        }


    }
}
