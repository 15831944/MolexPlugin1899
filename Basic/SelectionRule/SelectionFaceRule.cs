using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace Basic
{
    /// <summary>
    /// 选择面
    /// </summary>
    public class SelectionFaceRule:ClassItem,ISelectionRule
    {
        private List<Face> faces = new List<Face>();

        public SelectionFaceRule(List<Face> face)
        {
            this.faces = face;
        }
        public SelectionIntentRule CreateSelectionRule()
        {
            Part workPart = Session.GetSession().Parts.Work;
            try
            {
                return workPart.ScRuleFactory.CreateRuleFaceDumb(faces.ToArray());
            }

            catch (NXException ex)
            {
                LogMgr.WriteLog("Basic.SelectionFaceRule.CreateSelectionRule:错误：" + ex.Message);
                throw ex;
            }     
        }
    }
}
