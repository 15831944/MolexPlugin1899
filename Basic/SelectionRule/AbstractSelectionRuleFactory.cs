using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace Basic
{
    /// <summary>
    /// 选择器工厂虚拟类
    /// </summary>
    public abstract class AbstractSelectionRuleFactory
    {
        private List<TaggedObject> selectionObj;
        protected AbstractSelectionRuleFactory nextSelection = null;
        public AbstractSelectionRuleFactory(List<TaggedObject> selectionObj)
        {
            this.selectionObj = selectionObj;
        }
        public abstract List<SelectionIntentRule> GetSelectionRule();
        /// <summary>
        /// 设置下一个选择器
        /// </summary>
        /// <param name="next"></param>
        public void SetNextSelection(AbstractSelectionRuleFactory next)
        {
            this.nextSelection = next;
        }

    }
}
