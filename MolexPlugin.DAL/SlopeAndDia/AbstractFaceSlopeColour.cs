using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 面斜率颜色虚类
    /// </summary>
    public abstract class AbstractFaceSlopeColour
    {
        protected AnalysisFaceSlopeAndDia analy;
        protected AbstractFaceSlopeColour colour;

        public AbstractFaceSlopeColour(AnalysisFaceSlopeAndDia analy)
        {
            this.analy = analy;
        }
        /// <summary>
        /// 设置颜色
        /// </summary>
        public abstract int FaceColour();
        /// <summary>
        /// 获取面类型
        /// </summary>
        /// <returns></returns>
        public abstract string GetFaceType();
        public void SetNext(AbstractFaceSlopeColour colour)
        {
            this.colour = colour;
        }
        protected int NextColour()
        {
            if (colour != null)
                return colour.FaceColour();
            return 0;
        }
        protected string NextFacetype()
        {
            if (colour != null)
                return colour.GetFaceType();
            return "";
        }
    }
}
