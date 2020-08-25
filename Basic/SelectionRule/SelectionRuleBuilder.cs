using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;


namespace Basic
{
    /// <summary>
    /// 选择器
    /// </summary>
    public class SelectionRuleBuilder
    {
        /// <summary>
        /// 获得选择器工厂
        /// </summary>
        /// <param name="taggedObjects"></param>
        /// <returns></returns>
        public static AbstractSelectionRuleFactory GetAbstractSelectionRuleFactoryBuilder(List<TaggedObject> taggedObjects)
        {
            AbstractSelectionRuleFactory bodyRule = new SelectionRuleBodyFactory(taggedObjects);
            AbstractSelectionRuleFactory pointRule = new SelectionRuleCurveFromPointFactory(taggedObjects);
            AbstractSelectionRuleFactory curveRule = new SelectionRuleCurveFactory(taggedObjects);
            AbstractSelectionRuleFactory edgeRule = new SelectionRuleEdgeFactory(taggedObjects);
            AbstractSelectionRuleFactory faceRule = new SelectionRuleFaceFactory(taggedObjects);

            bodyRule.SetNextSelection(pointRule);
            pointRule.SetNextSelection(curveRule);
            curveRule.SetNextSelection(edgeRule);
            curveRule.SetNextSelection(faceRule);

            return bodyRule;
        }


    }
}
