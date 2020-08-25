using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolexPlugin.DAL
{
    public class FaceSlopeColourBuilder
    {
        /// <summary>
        /// 创建面颜色
        /// </summary>
        /// <param name="analy"></param>
        /// <returns></returns>
        public static AbstractFaceSlopeColour Build(AnalysisFaceSlopeAndDia analy)
        {
            AbstractFaceSlopeColour plane = new PlaneFaceSlopeColour(analy); 
            AbstractFaceSlopeColour vert = new VerticalFaceSlopeColour(analy);
            AbstractFaceSlopeColour slant = new SlantFaceSlopeColour(analy);
            AbstractFaceSlopeColour back = new BackOffFaceSlopeColour(analy);
            plane.SetNext(vert);
            vert.SetNext(slant);
            slant.SetNext(back);
            return plane;
        }
    }
}
