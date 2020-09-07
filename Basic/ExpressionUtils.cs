using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;


namespace Basic
{
    /// <summary>
    /// 表达式
    /// </summary>
    public class ExpressionUtils : ClassItem
    {
        /// <summary>
        /// 创建表达式
        /// </summary>
        /// <param name="expAndName">如 x = 1.2 * y + z</param>
        /// <param name="expressionType">表达式类型</param>
        /// <returns></returns>
        public static Expression CreateExp(string expAndName, string expressionType)
        {
            Part workPart = theSession.Parts.Work;
            try
            {
                Expression exp = workPart.Expressions.CreateExpression(expressionType, expAndName);
                return exp;
            }
            catch (NXException ex)
            {

                LogMgr.WriteLog("ExpressionUtils:CreateExp:表达式" + expAndName + "          " + ex.Message);
                throw ex;
            }

        }

        /// <summary>
        /// 更新表达式
        /// </summary>
        /// <param name="expName">表达式名</param>
        /// <param name="expNum">表达式值</param>
        public static bool UpdateExp(string expName, string expNum)
        {
            Part workPart = theSession.Parts.Work;
            theSession.Preferences.Modeling.UpdatePending = false;

            try
            {
                Expression exp = workPart.Expressions.FindObject(expName);
                if (exp == null)
                    return false;
                workPart.Expressions.Edit(exp, expNum);  //编辑表达式
                return true;
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("ExpressionUtils:UpdateExp:表达式" + expName + "            " + ex.Message);
                return false;
            }


        }
        /// <summary>
        /// 删除表达式
        /// </summary>
        /// <param name="expName"></param>
        public static bool DeteteExp(string expName)
        {
            Part workPart = theSession.Parts.Work;
            theSession.Preferences.Modeling.UpdatePending = false;

            try
            {
                Expression exp = workPart.Expressions.FindObject(expName);
                if (exp == null)
                    return false;
                workPart.Expressions.Delete(exp);
                return true;
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("ExpressionUtils:DeteteExp:表达式" + expName + "                 " + ex.Message);
                return false;
            }


        }
        /// <summary>
        /// 设置属性表达式
        /// </summary>
        /// <param name="expName"></param>
        /// <param name="attrName"></param>
        /// <param name="type"></param>
        public static Expression SetAttrExp(string expName, string attrName, NXObject.AttributeType type)
        {
            Part workPart = theSession.Parts.Work;
            try
            {
                Expression exp = workPart.Expressions.GetAttributeExpression(workPart, attrName, type, -1);
                if (!expName.Equals(attrName, StringComparison.CurrentCultureIgnoreCase))
                    exp.SetName(expName);
                return exp;
            }
            catch (NXException ex)
            {
                LogMgr.WriteLog("ExpressionUtils:SetAttrExp:表达式" + expName + "               " + ex.Message);
                throw ex;
            }

        }
        /// <summary>
        /// 设置属性表达式
        /// </summary>
        /// <param name="expName"></param>
        /// <param name="attrName"></param>
        /// <param name="type"></param>
        /// <param name="attNumber">属性组号</param>
        public static Expression SetAttrExp(string expName, string attrName, NXObject.AttributeType type, int attNumber)
        {
            Part workPart = theSession.Parts.Work;
            try
            {
                Expression exp = workPart.Expressions.GetAttributeExpression(workPart, attrName, type, attNumber);
                exp.SetName(expName);
                return exp;
            }
            catch (NXException ex)
            {
                LogMgr.WriteLog("ExpressionUtils:SetAttrExp:表达式" + expName + "               " + ex.Message);
                throw ex;
            }

        }
        /// <summary>
        /// 获取工作部件表达式
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Expression GetExpByName(string name)
        {
            Part workPart = theSession.Parts.Work;
            try
            {
                return workPart.Expressions.FindObject(name);
            }
            catch (NXException ex)
            {
                LogMgr.WriteLog("ExpressionUtils:GetExpByName:表达式 " + name + "              " + ex.Message);
                throw ex;
            }

        }
        /// <summary>
        /// 编辑表达式
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="expValue"></param>
        public static bool EditExp(Expression exp, string expValue)
        {
            Part workPart = theSession.Parts.Work;
            try
            {
                workPart.Expressions.Edit(exp, expValue);
                return true;
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("ExpressionUtils:EditExp:表达式" + exp.Name + "             " + ex.Message);
                return false;
            }

        }




    }
}
