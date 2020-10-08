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
    /// 创建刀路抽象类
    /// </summary>
    public abstract class AbstractCreateOperation
    {
        protected int site;
        protected string toolName;
        protected ElectrodeCAMTemplateModel template;
        protected OperationNameModel nameModel = null;
        protected AbstractOperationModel operModel = null;
        /// <summary>
        /// 刀具名称
        /// </summary>
        public string ToolName { get { return toolName; } }

        /// <summary>
        /// 刀具路径
        /// </summary>
        public AbstractOperationModel OperModel { get { return operModel; } }
        /// <summary>
        /// 刀具名字
        /// </summary>
        public OperationNameModel NameModel { get { return nameModel; } }
        /// <summary>
        /// 间隙
        /// </summary>
        public double Inter { get; set; }

        public ElectrodeOperationType Type { get; protected set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="site">程序条数</param>
        /// <param name="programNumber">在程序组第几条</param>
        /// <param name="tool"></param>
        public AbstractCreateOperation(int site, string tool)
        {
            this.site = site;
            this.toolName = tool;
            template = new ElectrodeCAMTemplateModel();
        }
        /// <summary>
        /// 创建刀路名
        /// </summary>
        /// <param name="tool"></param>
        public abstract void CreateOperationName(int programNumber);
        /// <summary>
        /// 创建刀路
        /// </summary>
        /// <param name="programNumber">程序组号</param>
        /// <param name="inter">电极间隙</param>
        public abstract List<string> CreateOperation();
        /// <summary>
        /// 拷贝刀路
        /// </summary>
        /// <param name="programNumber">程序组号</param>
        /// <returns></returns>
        public abstract AbstractCreateOperation CopyOperation(int programNumber);
        /// <summary>
        /// 设置刀具名
        /// </summary>
        /// <param name="tool"></param>
        public void SetToolName(string tool)
        {
            this.toolName = tool;
            if (operModel != null)
            {
                if (template == null)
                    template = new ElectrodeCAMTemplateModel();
                NCGroup toolGroup = template.FindTool(tool);
                if (toolGroup == null)
                    throw new Exception("无法找到" + tool + "刀具！");
                operModel.MoveOperationToTool(toolGroup);
            }


        }
        /// <summary>
        /// 设置程序名
        /// </summary>
        /// <param name="program"></param>
        public void SetProgramName(int site)
        {
            string preName = "O" + string.Format("{0:D4}", site);
            this.site = site;
            this.nameModel.ProgramName = preName;
            if (operModel != null)
            {
                if (template == null)
                    template = new ElectrodeCAMTemplateModel();
                NCGroup preGroup = template.FindTool(preName);
                if (preGroup == null)
                {
                    preGroup = template.CreateProgram(preName);
                }
                operModel.MoveOpreationToProgram(preGroup);
            }

        }
        /// <summary>
        /// 设置程序名
        /// </summary>
        /// <param name="program"></param>
        public void SetProgramName(string program)
        {
            this.nameModel.ProgramName = program;
            if (operModel != null)
            {
                if (template == null)
                    template = new ElectrodeCAMTemplateModel();
                NCGroup preGroup = template.FindTool(program);
                if (preGroup == null)
                {
                    preGroup = template.CreateProgram(program);
                }
                operModel.MoveOpreationToProgram(preGroup);
            }

        }
        /// <summary>
        /// 设置刀具路径比要数据
        /// </summary>
        /// <param name="eleCam"></param>
        public abstract void SetOperationData(AbstractElectrodeCAM eleCam);
        public int CompareTo(AbstractCreateOperation other)
        {
            return this.site.CompareTo(other.site);

        }
    }

    public enum ElectrodeOperationType
    {
        /// <summary>
        /// 开粗
        /// </summary>
        RoughCreate = 1,
        /// <summary>
        /// 二次开粗
        /// </summary>
        TwiceRough,
        /// <summary>
        /// 光平面
        /// </summary>
        FaceMilling,
        /// <summary>
        /// 光侧面
        /// </summary>
        PlanarMilling,
        /// <summary>
        /// 固定轮廓铣
        /// </summary>
        SurfaceContour,
        /// <summary>
        /// 等高
        /// </summary>
        ZLevelMilling,
        /// <summary>
        /// 清根
        /// </summary>
        FlowCut,
        /// <summary>
        /// 基准面
        /// </summary>
        BaseFace,
        /// <summary>
        /// 基准框
        /// </summary>
        BaseStation,
    }
}
