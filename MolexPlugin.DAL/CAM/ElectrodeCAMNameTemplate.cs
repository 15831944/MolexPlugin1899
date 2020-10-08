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
    public class ElectrodeCAMNameTemplate
    {
        /// <summary>
        /// 开粗名字
        /// </summary>
        /// <param name="program"></param>
        /// <param name="tool"></param>
        /// <param name="operNumber"></param>
        /// <returns></returns>
        public static OperationNameModel AskOperationNameModelOfRough(string program, string tool, int operNumber)
        {
            int k = program.LastIndexOf("0");
            string temp = "1";
            if (k != -1)
            {
                temp = program.Substring((k + 1), (program.Length - 1));
            }
            OperationNameModel model = new OperationNameModel()
            {
                templateName = "electrode",
                templateOperName = "R8",
                ProgramName = program,
                OperName = tool + "-R-" + temp + "-" + operNumber.ToString(),
                PngName = "260.bmp",
                ToolName = tool,
                OperType = OperationType.CavityMilling
            };
            return model;
        }
        /// <summary>
        /// 二次开粗名字
        /// </summary>
        /// <param name="program"></param>
        /// <param name="tool"></param>
        ///  /// <param name="operNumber"></param>
        /// <returns></returns>
        public static OperationNameModel AskOperationNameModelOfTwiceRough(string program, string tool, int operNumber)
        {
            int k = program.LastIndexOf("0");
            string temp = "1";
            if (k != -1)
            {
                temp = program.Substring((k + 1), (program.Length - 1));
            }
            OperationNameModel model = new OperationNameModel()
            {
                templateName = "electrode",
                templateOperName = "R3",
                ProgramName = program,
                OperName = tool + "-CR-" + temp + "-" + operNumber.ToString(),
                PngName = "260.bmp",
                ToolName = tool,
                OperType = OperationType.CavityMilling
            };
            return model;
        }

        /// <summary>
        /// 光基准台名字
        /// </summary>
        /// <param name="program"></param>
        /// <param name="tool"></param>
        /// <returns></returns>
        public static OperationNameModel AskOperationNameModelOfBaseStation(string program, string tool, int operNumber)
        {
            int k = program.LastIndexOf("0");
            string temp = "1";
            if (k != -1)
            {
                temp = program.Substring((k + 1), (program.Length - 1));
            }
            OperationNameModel model = new OperationNameModel()
            {
                templateName = "electrode",
                templateOperName = "J",
                ProgramName = program,
                OperName = tool + "-F-" + temp + "-" + operNumber.ToString(),
                PngName = "110.bmp",
                ToolName = tool,
                OperType = OperationType.PlanarMillingBase
            };
            return model;
        }
        /// <summary>
        /// 光面名字
        /// </summary>
        /// <param name="program"></param>
        /// <param name="tool"></param>
        /// <returns></returns>
        public static OperationNameModel AskOperationNameModelOfFaceMilling(string program, string tool, int operNumber)
        {
            int k = program.LastIndexOf("0");
            string temp = "1";
            if (k != -1)
            {
                temp = program.Substring((k + 1), (program.Length - 1));
            }
            OperationNameModel model = new OperationNameModel()
            {
                templateName = "electrode",
                templateOperName = "TOP",
                ProgramName = program,
                OperName = tool + "-FL-" + temp + "-" + operNumber.ToString(),
                PngName = "261.bmp",
                ToolName = tool,
                OperType = OperationType.FaceMilling
            };
            return model;
        }
        /// <summary>
        /// 平面铣名字
        /// </summary>
        /// <param name="program"></param>
        /// <param name="tool"></param>
        /// <returns></returns>
        public static OperationNameModel AskOperationNameModelOfPlanarMilling(string program, string tool, int operNumber)
        {
            int k = program.LastIndexOf("0");
            string temp = "1";
            if (k != -1)
            {
                temp = program.Substring((k + 1), (program.Length - 1));
            }
            OperationNameModel model = new OperationNameModel()
            {
                templateName = "electrode",
                templateOperName = "P2.98",
                ProgramName = program,
                OperName = tool + "-F-" + temp + "-" + operNumber.ToString(),
                PngName = "110.bmp",
                ToolName = tool,
                OperType = OperationType.PlanarMilling
            };
            return model;
        }
        /// <summary>
        /// 等高铣名字
        /// </summary>
        /// <param name="program"></param>
        /// <param name="tool"></param>
        /// <returns></returns>
        public static OperationNameModel AskOperationNameModelOfZLevelMilling(string program, string tool, int operNumber)
        {
            int k = program.LastIndexOf("0");
            string temp = "1";
            if (k != -1)
            {
                temp = program.Substring((k + 1), (program.Length - 1));
            }
            OperationNameModel model = new OperationNameModel()
            {
                templateName = "electrode",
                templateOperName = "Z1",
                ProgramName = program,
                OperName = tool + "-F-" + temp + "-" + operNumber.ToString(),
                PngName = "263.bmp",
                ToolName = tool,
                OperType = OperationType.ZLevelMilling
            };
            return model;
        }
        /// <summary>
        /// 固定轮廓铣名字
        /// </summary>
        /// <param name="program"></param>
        /// <param name="tool"></param>
        /// <returns></returns>
        public static OperationNameModel AskOperationNameModelOfSurfaceContour(string program, string tool, int operNumber)
        {
            int k = program.LastIndexOf("0");
            string temp = "1";
            if (k != -1)
            {
                temp = program.Substring((k + 1), (program.Length - 1));
            }
            OperationNameModel model = new OperationNameModel()
            {
                templateName = "electrode",
                templateOperName = "C1",
                ProgramName = program,
                OperName = tool + "-F-" + temp + "-" + operNumber.ToString(),
                PngName = "210.bmp",
                ToolName = tool,
                OperType = OperationType.SurfaceContour
            };
            return model;
        }
        /// <summary>
        /// 清根铣名字
        /// </summary>
        /// <param name="program"></param>
        /// <param name="tool"></param>
        /// <returns></returns>
        public static OperationNameModel AskOperationNameModelOfFlowCut(string program, string tool, int operNumber)
        {
            int k = program.LastIndexOf("0");
            string temp = "1";
            if (k != -1)
            {
                temp = program.Substring((k + 1), (program.Length - 1));
            }
            OperationNameModel model = new OperationNameModel()
            {
                templateName = "electrode",
                templateOperName = "CQG",
                ProgramName = program,
                OperName = tool + "-F-" + temp + "-" + operNumber.ToString(),
                PngName = "210.bmp",
                ToolName = tool,
                OperType = OperationType.SurfaceContour
            };
            return model;
        }
        /// <summary>
        /// 钻孔名字
        /// </summary>
        /// <param name="program"></param>
        /// <param name="tool"></param>
        /// <returns></returns>
        public static OperationNameModel AskOperationNameModelOfPointToPoint(string program, string tool, int operNumber)
        {
            int k = program.LastIndexOf("0");
            string temp = "1";
            if (k != -1)
            {
                temp = program.Substring((k + 1), (program.Length - 1));
            }
            OperationNameModel model = new OperationNameModel()
            {
                templateName = "electrode",
                templateOperName = "CK",
                ProgramName = program,
                OperName = tool + "-DR-" + temp + "-" + operNumber.ToString(),
                PngName = "450.bmp",
                ToolName = tool,
                OperType = OperationType.SurfaceContour
            };
            return model;
        }

    }
}
