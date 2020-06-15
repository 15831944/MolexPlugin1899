using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 分析体
    /// </summary>
    public class AnalysisBodySlopeAndMinDia
    {
        private Vector3d vec;
        private Body body;
        private UserSingleton user;
        private List<AnalysisFaceSlopeAndDia> anaFace = new List<AnalysisFaceSlopeAndDia>();
        /// <summary>
        /// 最小内R角
        /// </summary>
        public double MinDia { get; private set; }

        public AnalysisBodySlopeAndMinDia(Vector3d vec, Body body)
        {
            this.body = body;
            this.vec = vec;
            user = UserSingleton.Instance();
        }
        /// <summary>
        /// 分析
        /// </summary>
        public void Analysis()
        {
            double min = 9999;
            if (!user.Jurisd.GetComm())
                return;

            foreach (Face face in body.GetFaces())
            {
                AnalysisFaceSlopeAndDia afs = new AnalysisFaceSlopeAndDia(face, vec);
                if (afs.Data.IntNorm == -1 && afs.MinDia != 0)
                {
                    if (min > afs.MinDia)
                        min = afs.MinDia;
                }
                anaFace.Add(afs);
            }
            this.MinDia = min;
        }
        /// <summary>
        /// 设置颜色
        /// </summary>
        public void SetColour()
        {
            foreach (AnalysisFaceSlopeAndDia an in anaFace)
            {
                an.SetColour();
            }
        }
        /// <summary>
        /// 判断是否有倒扣面
        /// </summary>
        /// <returns></returns>
        public bool AskBackOffFace()
        {
            anaFace.Sort();
            for (int i = 1; i < anaFace.Count; i++)
            {
                return anaFace[i].isBackOffFace();
            }
            return false;
        }

    }
}
