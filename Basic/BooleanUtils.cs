using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;


namespace Basic
{
    public class BooleanUtils : ClassItem
    {
        /// <summary>
        /// 布尔操作
        /// </summary>
        /// <param name="targetBody">目标体</param>
        /// <param name="toolBody">工具体</param>
        /// <param name="copyTools">复制工具</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static NXOpen.Features.BooleanFeature CreateBooleanFeature(Body targetBody, bool copyTargets, bool copyTools, NXOpen.Features.Feature.BooleanType type, params Body[] toolBody)
        {
            Part workPart = theSession.Parts.Work;
            NXOpen.Features.BooleanFeature nullNXOpen_Features_BooleanFeature = null;
            NXOpen.Features.BooleanBuilder booleanBuilder1 = workPart.Features.CreateBooleanBuilderUsingCollector(nullNXOpen_Features_BooleanFeature);        
            booleanBuilder1.CopyTargets = copyTargets;
            booleanBuilder1.CopyTools = copyTools;
            booleanBuilder1.Operation = type;

            bool added1 = booleanBuilder1.Targets.Add(targetBody);

            NXOpen.ScCollector scCollector = workPart.ScCollectors.CreateCollector();
            ISelectionRule rule = new SelectionBodyRule(toolBody.ToList());
            SelectionIntentRule[] rules = { rule.CreateSelectionRule() };
            scCollector.ReplaceRules(rules, false);
            booleanBuilder1.ToolBodyCollector = scCollector;
            try
            {
                NXOpen.Features.Feature boolFeature = booleanBuilder1.CommitFeature();
                return boolFeature as NXOpen.Features.BooleanFeature;
            }
            catch (NXException ex)
            {
                LogMgr.WriteLog("Basic.BooleanUtils.CreateBooleanFeature:错误：" + ex.Message);
                throw ex;
            }
            finally
            {
                booleanBuilder1.Destroy();
            }

        }
    }
}
