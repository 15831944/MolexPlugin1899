using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 电极表达式抽象类
    /// </summary>
    public abstract class AbstractElectrodeExpression
    {

        public AbstractElectrodeExpression()
        {

        }
        /// <summary>
        /// 创建表达式
        /// </summary>
        public abstract bool CreateExp(bool zDatum, int[] pre = null);
        /// <summary>
        /// 公共表达式
        /// </summary>
        protected bool CreateDefault()
        {
            try
            {
                ExpressionUtils.SetAttrExp("PitchX", "PitchX", NXObject.AttributeType.Real);
                ExpressionUtils.SetAttrExp("PitchXNum", "PitchXNum", NXObject.AttributeType.Integer);
                ExpressionUtils.SetAttrExp("PitchY", "PitchY", NXObject.AttributeType.Real);
                ExpressionUtils.SetAttrExp("PitchYNum", "PitchYNum", NXObject.AttributeType.Integer);

                ExpressionUtils.SetAttrExp("PreparationX", "Preparation", NXObject.AttributeType.Integer, 0);
                ExpressionUtils.SetAttrExp("PreparationY", "Preparation", NXObject.AttributeType.Integer, 1);
                ExpressionUtils.SetAttrExp("PreparationZ", "Preparation", NXObject.AttributeType.Integer, 2);

                ExpressionUtils.SetAttrExp("DatumWidth", "DatumWidth", NXObject.AttributeType.Real);
                ExpressionUtils.SetAttrExp("DatumHeigth", "DatumHeigth", NXObject.AttributeType.Real);

                ExpressionUtils.CreateExp("moveZ=0", "Number");             
                return true;
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("创建表达式错误！" + ex.Message);
                return false;
            }
        }
    }
}
