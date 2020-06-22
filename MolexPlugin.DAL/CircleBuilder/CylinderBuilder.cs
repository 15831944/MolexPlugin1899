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
    /// 圆柱形
    /// </summary>
    public class CylinderBuilder
    {
        /// <summary>
        /// 获取圆柱特征
        /// </summary>
        /// <param name="circle"></param>
        /// <param name="cyl"></param>
        /// <returns></returns>
        public static CylinderFeater GetCylinderFeater(List<AbstractCircleFace> circle, CylinderFace cyl)
        {
            List<AbstractCircleFace> cylinder = new List<AbstractCircleFace>();
            cylinder.Add(cyl);
            int index = circle.IndexOf(cyl);
            if (index != -1)
            {
                for (int i = index - 1; i < 0; i--)
                {
                    if (!(circle[1] is CylinderFace) || !(circle[1] is CircleAnnylusFace))
                    {
                        cylinder.Add(cyl);
                    }
                }
                for (int i = index + 1; i < circle.Count; i++)
                {
                    if (!(circle[1] is CylinderFace) || !(circle[1] is CircleAnnylusFace))
                    {
                        cylinder.Add(cyl);
                    }
                }
            }
            return new CylinderFeater(cylinder, cyl);
        }

    }
}
