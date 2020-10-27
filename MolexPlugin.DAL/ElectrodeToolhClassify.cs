using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 电极头分类
    /// </summary>
    public class ElectrodeToolhClassify : IComparable<ElectrodeToolhClassify>
    {
        /// <summary>
        /// X 向跑位
        /// </summary>
        public double X { get; private set; }
        /// <summary>
        /// Y向跑位
        /// </summary>
        public double Y { get; private set; }

        public List<Body> ToolhBodys { get; private set; } = new List<Body>();

        public int CompareTo(ElectrodeToolhClassify other)
        {
            if (UMathUtils.IsEqual(this.X, other.X))
            {
                return this.Y.CompareTo(other.Y);
            }
            else
                return this.X.CompareTo(other.X);
        }
        /// <summary>
        /// 电极头分类
        /// </summary>
        /// <param name="bodys"></param>
        /// <returns></returns>
        public static List<ElectrodeToolhClassify> Classify(List<Body> bodys)
        {
            List<ElectrodeToolhClassify> toolh = new List<ElectrodeToolhClassify>();
            foreach (Body by in bodys)
            {
                double x = AttributeUtils.GetAttrForDouble(by, "Offset", 0);
                double y = AttributeUtils.GetAttrForDouble(by, "Offset", 1);
                ElectrodeToolhClassify temp = toolh.Find(a => UMathUtils.IsEqual(a.X, x) && UMathUtils.IsEqual(a.Y, y));
                if (temp != null)
                {
                    temp.ToolhBodys.Add(by);
                }
                else
                {
                    ElectrodeToolhClassify ey = new ElectrodeToolhClassify();
                    ey.X = x;
                    ey.Y = y;
                    ey.ToolhBodys.Add(by);
                    toolh.Add(ey);
                }
            }
            return toolh;
        }
    }
}
