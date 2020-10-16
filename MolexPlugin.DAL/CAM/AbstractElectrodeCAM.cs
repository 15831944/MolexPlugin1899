using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;
using NXOpen.UF;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 电极加工
    /// </summary>
    public abstract class AbstractElectrodeCAM
    {
        protected Part pt;
        protected UserModel user;
        protected AnalysisElectrodeBody analysis;
        protected bool IsOffsetInter = false;
  
        protected List<Face> allFace = new List<Face>();
      
        protected OffsetBodyGapVaule offser;
        public AnalysisElectrodeBody Analysis { get { return analysis; } }
        public double Inter { get; protected set; }
        /// <summary>
        /// 是否可以计算刀路
        /// </summary>
        public bool IsCompute { get; protected set; }
        public AbstractElectrodeCAM(Part pt, UserModel user)
        {
            if (pt == null)
                throw new Exception("传入电极为空！");
            this.pt = pt;
            this.user = user;
            offser = new OffsetBodyGapVaule(pt, user);
            Body[] bodys = pt.Bodies.ToArray();
            if (bodys == null || bodys.Length == 0 || bodys.Length > 1)
            {
                throw new Exception("传入电极错误！");
            }
            else
            {
                analysis = new AnalysisElectrodeBody(pt.Bodies.ToArray()[0]);
            }

        }
        /// <summary>
        /// 获取刀具
        /// </summary>
        /// <returns></returns>
        public abstract CompterToolName GetTool();
        /// <summary>
        /// 扣间隙
        /// </summary>
        /// <returns></returns>
        public abstract bool CreateOffsetInter();
        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <returns></returns>
        public abstract bool CreateNewFile(string filePath);

        /// <summary>
        /// 获取基准框边界和地面点
        /// </summary>
        /// <returns></returns>
        public virtual void GetBaseStationBoundary(out BoundaryModel boundary, out Point3d floorPt)
        {
            double blank;
            PlanarBoundary pl = new PlanarBoundary(analysis.BaseFace);
            pl.GetPeripheralBoundary(out boundary, out blank);
            boundary.ToolSide = NXOpen.CAM.BoundarySet.ToolSideTypes.OutsideOrRight;
            boundary.BouudaryPt = analysis.BaseFace.BoxMinCorner;
          //  boundary.PlaneTypes = NXOpen.CAM.BoundarySet.PlaneTypes.Automatic;
            floorPt = this.analysis.BaseSubfaceFace.BoxMinCorner;
        }

        /// <summary>
        /// 获取基准面边界和间隙
        /// </summary>
        /// <returns></returns>
        public abstract Dictionary<double, BoundaryModel[]> GetBaseFaceBoundary();
        /// <summary>
        /// 获取平坦面和间隙
        /// </summary>
        /// <returns></returns>
        public abstract Dictionary<double, Face[]> GetFlatFaces();
        /// <summary>
        /// 获取陡峭和间隙
        /// </summary>
        /// <returns></returns>
        public abstract Dictionary<double, Face[]> GetSteepFaces();
        /// <summary>
        /// 获取斜度面和间隙
        /// </summary>
        /// <returns></returns>
        public abstract Dictionary<double, Face[]> GetSlopeFaces();
        /// <summary>
        /// 获取平面和间隙
        /// </summary>
        /// <returns></returns>
        public abstract Dictionary<double, Face[]> GetPlaneFaces();
        /// <summary>
        /// 获取全部和间隙
        /// </summary>
        /// <returns></returns>
        public abstract Dictionary<double, Face[]> GetAllFaces();
        /// <summary>
        /// 获取基准面线
        /// </summary>
        /// <returns></returns>
        public abstract Line[] GetBaseFaceLine();


    }
}
