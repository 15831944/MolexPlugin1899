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
        public HoleBuilder Builder { get; private set; }

        public AbstractHoleFeater(HoleBuilder builder)
        {
            this.Builder = builder;
            GetDirection();
            GetStartAndEndPt();
        }

        public void Highlight(bool highlight)
        {
            this.Builder.Highlight(highlight);
        }
        /// <summary>
        /// 获取起点和终点
        /// </summary>
        protected abstract void GetStartAndEndPt();
        /// <summary>
        /// 获取顶边
        /// </summary>
        /// <returns></returns>
        public abstract ArcEdgeData GetTopEdge();
        /// <summary>
        /// 获取轴向方向
        /// </summary>
        protected abstract void GetDirection();
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
