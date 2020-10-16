using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 计算刀具
    /// </summary>
    public class CompterToolName
    {
        private AnalysisElectrodeBody analysisEle;
        private double eleMinDis;
        private double baseFaceDia;
        private double minDis = 0;
        public double[] twice = new double[] { 4, 3, 2, 1.5, 1, 0.8, 0.6, 0.5, 0.4, 0.3, 0.2 };
        public CompterToolName(AnalysisElectrodeBody analysisEle, double eleMinDis)
        {
            this.analysisEle = analysisEle;
            this.eleMinDis = eleMinDis;
            try
            {
                this.baseFaceDia = analysisEle.GetBaseMinDia(analysisEle.BaseFace.Face) * 2;
                if (eleMinDis > baseFaceDia)
                {
                    minDis = baseFaceDia;
                }
                else
                {
                    minDis = eleMinDis;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 获得开粗刀具
        /// </summary>
        /// <returns></returns>
        public string GetRoughTool()
        {

            if (minDis > 8.2)
                return "EM8";
            if (minDis > 6.2 && minDis <= 8.2)
                return "EM6";
            if (minDis <= 6.2)
                return "EM8";
            return "EM8";
        }
        /// <summary>
        /// 获得光基准框刀具
        /// </summary>
        /// <returns></returns>
        public string GetBaseStationTool()
        {
            if (minDis > 8.2)
                return "EM7.98";
            if (minDis > 6.2 && minDis <= 8.2)
                return "EM5.98";
            if (minDis <= 6.2)
                return "EM7.98";
            return "EM7.98";
        }

        /// <summary>
        /// 获得二次开粗刀具
        /// </summary>
        /// <returns></returns>
        public List<string> GetTwiceRoughTool()
        {
            List<string> twice = new List<string>();

            if (minDis >= 6.0)
                return twice;
            double min = 4;
            foreach (double k in this.twice)
            {
                if ((k) <= minDis)
                {
                    min = k;
                    break;
                }

            }
            if (min >= 0.8)
            {
                twice.Add("EM" + min.ToString());
            }
            else
            {
                twice.Add("EM1");
                twice.Add("EM" + min.ToString());
            }
            return twice;
        }

        /// <summary>
        /// 获取精加工平刀
        /// </summary>
        /// <returns></returns>
        public string GetFinishFlatTool()
        {
            foreach (double k in twice)
            {
                if ((k) <= minDis)
                    return "EM" + (k - 0.02).ToString();
            }
            return "EM2.98";
        }
        /// <summary>
        /// 获取精加工球刀
        /// </summary>
        /// <returns></returns>
        public string GetFinishBallTool()
        {
            List<Face> slope = new List<Face>();
            double min = 9999;
            this.analysisEle.GetSlopeFaces(out slope, out min);
            foreach (double k in twice)
            {
                if ((k) <= min)
                {
                    min = k;
                    break;
                }
            }
            if (min >= 2)
                return "BN1.98";
            else
                return "BN" + (min - 0.02).ToString();

        }
        /// <summary>
        /// 获取光基准台面
        /// </summary>
        /// <returns></returns>
        public string GetBaseFaceTool()
        {
            double min = 0;
            foreach (double k in twice)
            {
                if ((k) <= minDis)
                {
                    min = k;
                    break;
                }
            }
            if (min >= 1.5)
            {
                return "EM" + (min - 0.02).ToString();
            }
            else
            {
                return "EM2.98";
            }

        }
    }
}
