using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;


namespace Basic
{
    /// <summary>
    /// 曲线上的点选择工厂
    /// </summary>
    public class SelectionRuleCurveFromPointFactory : AbstractSelectionRuleFactory
    {
        private List<Point> points = new List<Point>();
        public SelectionRuleCurveFromPointFactory(List<TaggedObject> taggedObjects) : base(taggedObjects)
        {
            this.points = taggedObjects.Where(a => a is Point).Select(a => a as Point).ToList();
        }
        public override List<SelectionIntentRule> GetSelectionRule()
        {
            List<SelectionIntentRule> sir = new List<SelectionIntentRule>();
            if (points.Count != 0)
            {
                ISelectionRule rule = new SelectionCurveFromPointRule(points);
                try
                {
                    SelectionIntentRule sr = rule.CreateSelectionRule();
                    sir.Add(sr);
                }
                catch
                {
                    ClassItem.WriteLogFile("选择线上点收集器错误！");
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
