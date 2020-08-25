using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;


namespace Basic
{
    /// <summary>
    /// 边选择工厂
    /// </summary>
    public class SelectionRuleEdgeFactory : AbstractSelectionRuleFactory
    {
        private List<Edge> edges = new List<Edge>();
        public SelectionRuleEdgeFactory(List<TaggedObject> taggedObjects) : base(taggedObjects)
        {
            this.edges = taggedObjects.Where(a => a is Edge).Select(a => a as Edge).ToList();
        }
        public override List<SelectionIntentRule> GetSelectionRule()
        {
            List<SelectionIntentRule> sir = new List<SelectionIntentRule>();
            if (edges.Count != 0)
            {
                ISelectionRule rule = new SelectionEdgeRule(edges);
                try
                {
                    SelectionIntentRule sr = rule.CreateSelectionRule();
                    sir.Add(sr);
                }
                catch
                {
                    ClassItem.WriteLogFile("选择边收集器错误！");
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
