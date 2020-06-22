using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.BlockStyler;
using Basic;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 孔特征抽象类
    /// </summary>
    public abstract class AbstractHoleFeater : IDisplayObject
    {
        /// <summary>
        /// 起点
        /// </summary>
        public Point3d StratPt { get; protected set; }
        /// <summary>
        /// 终点
        /// </summary>
        public Point3d EndPt { get; protected set; }
        /// <summary>
        /// 方向
        /// </summary>
        public Vector3d Direction { get; protected set; }

        public double Length
        {
            get
            {
                return UMathUtils.GetDis(this.StratPt, this.EndPt);
            }
        }
        /// <summary>
        /// 孔类型
        /// </summary>
        public HoleType Type { get; protected set; }
        /// <summary>
        /// 孔特征
        /// </summary>
        public List<CylinderFeater> CylinderFeater { get; private set; }

        public AbstractHoleFeater(List<CylinderFeater> cylFeat)
        {
            this.CylinderFeater = cylFeat;
        }
        /// <summary>
        /// 判断是否是盲孔
        /// </summary>
        /// <returns></returns>
        protected bool GetAskBlindHole()
        {
            Vector3d vec1 = this.CylinderFeater[0].Direction;
            Vector3d vec2 = new Vector3d(-vec1.X, -vec1.Y, -vec1.Z);
            int k1 = TraceARay.AskTraceARay(this.CylinderFeater[0].Cylinder.Data.Face.GetBody(), this.CylinderFeater[0].StartPt, vec1);
            int k2 = TraceARay.AskTraceARay(this.CylinderFeater[0].Cylinder.Data.Face.GetBody(), this.CylinderFeater[0].StartPt, vec1);
            return k1 != 0 || k2 != 0;
        }
        public void Highlight(bool highlight)
        {
            foreach (CylinderFeater cf in CylinderFeater)
            {
                cf.Highlight(highlight);
            }
        }
    }
    public enum HoleType
    {
        OnlyBlindHole = 1,             //单一盲孔
        OnlyThroughHole = 2,           //单一通孔
        StepBlindHole = 3,            // 台阶盲孔
        StepHole = 4,                 //平底盲孔
        StepThroughHole = 5           //台阶通孔
    }
}
