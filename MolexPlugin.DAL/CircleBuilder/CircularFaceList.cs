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
    public class CircularFaceList
    {
        public List<AbstractCircleFace> CircleFaceList { get; private set; } = new List<AbstractCircleFace>();
        /// <summary>
        /// 判断是否是同一个孔并添加
        /// </summary>
        /// <param name="cf"></param>
        /// <returns></returns>
        public bool IsInThisHole(AbstractCircleFace cf)
        {
            if (this.CircleFaceList.Count == 0 || this.CircleFaceList == null)
            {
                this.CircleFaceList.Add(cf);
                return true;
            }
            else
            {
                if (this.CircleFaceList[0].IsTheSameHole(cf))
                {
                    this.CircleFaceList.Add(cf);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 判断是否是一个凸台并添加
        /// </summary>
        /// <param name="cf"></param>
        /// <returns></returns>
        public bool IsInThisStep(AbstractCircleFace cf)
        {
            if (this.CircleFaceList.Count == 0 || this.CircleFaceList == null)
            {
                this.CircleFaceList.Add(cf);
                return true;
            }
            else
            {
                if (this.CircleFaceList[0].IsCircleStep(cf))
                {
                    this.CircleFaceList.Add(cf);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 获取圆柱特征
        /// </summary>
        /// <returns></returns>
        public List<CylinderFeater> GetCylinderFeaters()
        {
            List<CylinderFace> cyls = CircleFaceList.FindAll(a => a is CylinderFace).Select(a => a as CylinderFace).ToList();
            List<CylinderFeater> featers = new List<CylinderFeater>();
            foreach (CylinderFace cy in cyls)
            {                              
                featers.Add(CylinderBuilder.GetCylinderFeater(this.CircleFaceList, cy));
            }
            return featers;
        }
    }
}
