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
    public class SelectionRuleFaceFactory : AbstractSelectionRuleFactory
    {
        private List<Face> faces = new List<Face>();
        public SelectionRuleFaceFactory(List<TaggedObject> taggedObjects) : base(taggedObjects)
        {
            this.faces = taggedObjects.Where(a => a is Face).Select(a => a as Face).ToList();
        }
        public override List<SelectionIntentRule> GetSelectionRule()
        {
            List<SelectionIntentRule> sir = new List<SelectionIntentRule>();
            if (faces.Count != 0)
            {
                ISelectionRule rule = new SelectionFaceRule(faces);
                try
                {
                    SelectionIntentRule sr = rule.CreateSelectionRule();
                    sir.Add(sr);
                }
                catch
                {
                    ClassItem.WriteLogFile("选择面收集器错误！");
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
