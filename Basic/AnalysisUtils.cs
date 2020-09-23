using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;
using Basic;

namespace Basic
{
    public class AnalysisUtils
    {
        /// <summary>
        /// 干涉体
        /// </summary>
        /// <param name="body1"></param>
        /// <param name="body2"></param>
        /// <param name="bodys"></param>
        /// <returns></returns>
        public static NXOpen.GeometricAnalysis.SimpleInterference.Result SetInterferenceOutResult(Body body1, Body body2, out List<Body> bodys)
        {
            bodys = new List<Body>();
            Part workPart = Session.GetSession().Parts.Work;
            NXOpen.GeometricAnalysis.SimpleInterference simpleInterference1;
            simpleInterference1 = workPart.AnalysisManager.CreateSimpleInterferenceObject();
            simpleInterference1.InterferenceType = NXOpen.GeometricAnalysis.SimpleInterference.InterferenceMethod.InterferenceSolid;
            simpleInterference1.FaceInterferenceType = NXOpen.GeometricAnalysis.SimpleInterference.FaceInterferenceMethod.AllPairs;
            simpleInterference1.FirstBody.Value = body1;
            simpleInterference1.SecondBody.Value = body2;
            NXOpen.GeometricAnalysis.SimpleInterference.Result result1;
            result1 = simpleInterference1.PerformCheck();
            if (result1 == NXOpen.GeometricAnalysis.SimpleInterference.Result.InterferenceExists)
            {
                NXObject[] objs = simpleInterference1.GetInterferenceResults();

                foreach (NXObject obj in objs)
                {
                    bodys.Add(obj as Body);
                }
            }
            try
            {
                NXOpen.NXObject nXObject1;
                nXObject1 = simpleInterference1.Commit();
                return result1;
            }
            catch (NXException ex)
            {
                LogMgr.WriteLog("Basic.AnalysisUtils.SetInterferenceOutResult:错误：" + ex.Message);
                throw ex;
            }
            finally
            {
                simpleInterference1.Destroy();
            }
        }
        /// <summary>
        /// 干涉面
        /// </summary>
        /// <param name="body1"></param>
        /// <param name="body2"></param>
        /// <returns></returns>
        public static void SetInterferenceOutFace(Body body1, Body body2, out List<Face> faces, out List<Body> bodys)
        {
            faces = new List<Face>();
            bodys = new List<Body>();
            Part workPart = Session.GetSession().Parts.Work;
            NXOpen.GeometricAnalysis.SimpleInterference simpleInterference1;
            simpleInterference1 = workPart.AnalysisManager.CreateSimpleInterferenceObject();
            simpleInterference1.InterferenceType = NXOpen.GeometricAnalysis.SimpleInterference.InterferenceMethod.InterferingFaces;
            simpleInterference1.FaceInterferenceType = NXOpen.GeometricAnalysis.SimpleInterference.FaceInterferenceMethod.AllPairs;
            simpleInterference1.FirstBody.Value = body1;
            simpleInterference1.SecondBody.Value = body2;
            NXOpen.GeometricAnalysis.SimpleInterference.Result result1;
            result1 = simpleInterference1.PerformCheck();
            try
            {
                NXObject[] objs = simpleInterference1.GetInterferenceResults();
                foreach (NXObject obj in objs)
                {
                    faces.Add(obj as Face);
                }

                simpleInterference1.InterferenceType = NXOpen.GeometricAnalysis.SimpleInterference.InterferenceMethod.InterferenceSolid;
                NXOpen.GeometricAnalysis.SimpleInterference.Result result2 = simpleInterference1.PerformCheck();
                if (result2 == NXOpen.GeometricAnalysis.SimpleInterference.Result.InterferenceExists)
                {
                    NXObject[] by = simpleInterference1.GetInterferenceResults();
                    foreach (NXObject obj in by)
                    {
                        bodys.Add(obj as Body);
                    }
                }

            }
            catch (NXException ex)
            {
                LogMgr.WriteLog("Basic.AnalysisUtils.SetInterferenceOutFace:错误：" + ex.Message);
                throw ex;
            }
            finally
            {
                simpleInterference1.Destroy();
            }
        }
        /// <summary>
        /// 分析最小距离
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <param name="ptOnObj1">obj1上的点</param>
        /// <param name="ptOnObj2">obj2上的点</param>
        /// <returns></returns>
        public static double AskMinimumDist(Tag obj1, Tag obj2, out double[] ptOnObj1, out double[] ptOnObj2)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            double[] guess1 = new double[3];
            double[] guess2 = new double[3];
            double minDist = -1;
            ptOnObj1 = new double[3];
            ptOnObj2 = new double[3];
            double accuracy;
            try
            {
                theUFSession.Modl.AskMinimumDist2(obj1, obj2, 0, guess1, 0, guess2, out minDist, ptOnObj1, ptOnObj2, out accuracy);
                return minDist;
            }
            catch (NXException ex)
            {
                LogMgr.WriteLog("Basic.AnalysisUtils.AskMinimumDist:错误：" + ex.Message);
                throw ex;
            }

        }

    }
}
