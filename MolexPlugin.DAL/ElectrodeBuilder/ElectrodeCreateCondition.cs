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
    /// 创建电极条件
    /// </summary>
    public class ElectrodeCreateCondition
    {


        private Part workpiece;
        private WorkModel work;
        private List<ElectrodeToolhInfo> toolhInfo = new List<ElectrodeToolhInfo>();

        public List<ElectrodeToolhInfo> ToolhInfo
        {
            get
            {
                return toolhInfo;
            }
        }

        public List<Body> HeadBodys { get; private set; }
        public WorkModel Work { get { return work; } }
        public ElectrodeCreateExpAndMatr ExpAndMatr { get; private set; }
        public ElectrodeCreateCondition(ElectrodeCreateExpAndMatr expAndMatr, List<Body> headBodys, WorkModel work, Part workpiece)
        {
            this.ExpAndMatr = expAndMatr;
            this.HeadBodys = headBodys;
            this.workpiece = workpiece;
            this.work = work;
            Initialize();
            toolhInfo = ToolhClassify(HeadBodys);
        }

        private void Initialize()
        {
            this.ExpAndMatr.Matr.Initialinze(work.Info.Matr, HeadBodys, workpiece);
            AskComputeDischargeFace(HeadBodys, workpiece);
        }
        /// <summary>
        /// 获取电极齿信息
        /// </summary>
        /// <param name="bodys"></param>
        /// <returns></returns>
        private List<ElectrodeToolhInfo> GetToolhInfo(List<Body> bodys)
        {
            List<ElectrodeToolhInfo> toolhs = new List<ElectrodeToolhInfo>();
            // var toolhNameList = bodys.GroupBy(a => AttributeUtils.GetAttrForString(a, "ToolhName"));
            var toolhNameList = bodys.GroupBy(delegate (Body a)
            {
                string temp = AttributeUtils.GetAttrForString(a, "ToolhName");
                char result;
                double tp = 0;
                if (Char.TryParse(temp, out result))
                    return ((int)result);
                else
                {
                    if (Double.TryParse(temp, out tp))
                        return tp;
                }
                return 0;
            });
            List<Double> keys = new List<Double>();
            foreach (IGrouping<Double, Body> group in toolhNameList)
            {
                keys.Add(group.Key);
            }
            keys.Sort();
            toolhNameList.OrderByDescending(a => a.Key);
            int num = 65;
            for (int j = 0; j < keys.Count; j++)
            {
                foreach (var te in toolhNameList)
                {
                    if (te.Key == keys[j])
                    {
                        ElectrodeToolhInfo toolh = ElectrodeToolhInfo.GetToolhInfoForAttribute(te.ToArray());
                        char k = (char)num;
                        toolh.SetToolhName(k.ToString());
                        toolhs.Add(toolh);
                        num++;
                        break;
                    }

                }

            }
            return toolhs;
        }

        private List<ElectrodeToolhInfo> ToolhClassify(List<Body> bodys)
        {
            int num = 65;
            List<ElectrodeToolhInfo> toolhs = new List<ElectrodeToolhInfo>();
            List<ElectrodeToolhClassify> classify = ElectrodeToolhClassify.Classify(bodys);
            foreach (ElectrodeToolhClassify ey in classify)
            {
                ElectrodeToolhInfo toolh = ElectrodeToolhInfo.GetToolhInfoForAttribute(ey.ToolhBodys.ToArray());
                char k = (char)num;
                toolh.SetToolhName(k.ToString());
                toolhs.Add(toolh);
                num++;
            }
            return toolhs;
        }
        /// <summary>
        /// 得到放电面积
        /// </summary>
        /// <param name="bodys"></param>
        /// <param name="workpiece"></param>
        private void AskComputeDischargeFace(List<Body> bodys, Part workpiece)
        {
            List<string> err = new List<string>();
            Body workpieceBody = workpiece.Bodies.ToArray()[0];
            Matrix4 inv = this.work.Info.Matr.GetInversMatrix();
            CartesianCoordinateSystem csys = BoundingBoxUtils.CreateCoordinateSystem(this.work.Info.Matr, inv);
            foreach (Body by in bodys)
            {
                if (!BodyInfo.IsContactArea(by))
                {
                    ComputeDischargeFace cdf = new ComputeDischargeFace(by, workpieceBody, this.work.Info.Matr, csys);
                    cdf.GetBodyInfoForInterference(false, out err);
                }
            }
        }
        /// <summary>
        /// 获得齿最小距离
        /// </summary>
        /// <returns></returns>
        public double AskMinDim()
        {
            double min = 9999;
            double[] pt1 = new double[3];
            double[] pt2 = new double[3];
            for (int i = 0; i < this.HeadBodys.Count - 1; i++)
            {
                for (int j = i + 1; j < this.HeadBodys.Count; j++)
                {
                    double temp = AnalysisUtils.AskMinimumDist(this.HeadBodys[i].Tag, this.HeadBodys[j].Tag, out pt1, out pt2);
                    temp = Math.Round(temp, 3);
                    if (min >= temp)
                        min = temp;
                }
            }
            return min;
        }

    }
}
