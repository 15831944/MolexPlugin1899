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
        private double min = 0;
        private List<AnalysisFaceSlopeAndDia> anaFace = new List<AnalysisFaceSlopeAndDia>();
        /// <summary>
        /// 最小内R角
        /// </summary>
        public double MinDia
        {
            get
            {
                if (this.anaFace.Count == 0)
                {
                    Analysis();
                }
                return min;
            }
        }

        public AnalysisBodySlopeAndMinDia(Vector3d vec, Body body)
        {
            this.body = body;
            this.vec = vec;
            user = UserSingleton.Instance();
        }
        /// <summary>
        /// 分析
        /// </summary>
        private void Analysis()
        {
            double min = 9999;
            if (user.UserSucceed && user.Jurisd.GetComm())
            {
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
            }
            this.min = min;
        }
        /// <summary>
        /// 设置颜色
        /// </summary>
        public void SetColour()
        {
            if (this.anaFace.Count == 0)
            {
                Analysis();
            }
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
            if (this.anaFace.Count == 0)
            {
                Analysis();
            }
            anaFace.Sort();
            for (int i = 1; i < anaFace.Count; i++)
            {
                return anaFace[i].isBackOffFace();
            }
            return false;
        }

    }
}
