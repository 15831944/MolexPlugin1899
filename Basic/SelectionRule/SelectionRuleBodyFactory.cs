using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;


namespace Basic
{
    /// <summary>
    /// 体选择工厂
    /// </summary>
    public class SelectionRuleBodyFactory : AbstractSelectionRuleFactory
    {
        private List<Body> bodys = new List<Body>();
        public SelectionRuleBodyFactory(List<TaggedObject> taggedObjects) : base(taggedObjects)
        {
            this.bodys = taggedObjects.Where(a => a is Body).Select(a => a as Body).ToList();
        }
        public override List<SelectionIntentRule> GetSelectionRule()
        {
            List<SelectionIntentRule> sir = new List<SelectionIntentRule>();
            if (bodys.Count != 0)
            {
                ISelectionRule rule = new SelectionBodyRule(bodys);
                try
                {
                    SelectionIntentRule sr = rule.CreateSelectionRule();
                    sir.Add(sr);
                }
                catch
                {
                    ClassItem.WriteLogFile("选择体收集器错误！");
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
