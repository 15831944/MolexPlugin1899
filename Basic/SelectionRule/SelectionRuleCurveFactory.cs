using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;


namespace Basic
{
    /// <summary>
    /// 曲线选择工厂
    /// </summary>
    public class SelectionRuleCurveFactory : AbstractSelectionRuleFactory
    {
        private List<Curve> curves = new List<Curve>();
        public SelectionRuleCurveFactory(List<TaggedObject> taggedObjects) : base(taggedObjects)
        {
            this.curves = taggedObjects.Where(a => a is Curve).Select(a => a as Curve).ToList();
        }
        public override List<SelectionIntentRule> GetSelectionRule()
        {
            List<SelectionIntentRule> sir = new List<SelectionIntentRule>();
            if (curves.Count != 0)
            {
                ISelectionRule rule = new SelectionCurveRule(curves);
                try
                {
                    SelectionIntentRule sr = rule.CreateSelectionRule();
                    sir.Add(sr);
                }
                catch
                {
                    ClassItem.WriteLogFile("选择线收集器错误！");
                    return null;
                }
            }
            if (this.nextSelection != null)
            {
                List<SelectionIntentRule> tmpe = this.nextSelection.GetSelectionRule();
                if (tmpe.Count != 0)
                    sir.AddRange(tmpe);
            }
            return sir;
        }
    }
}
