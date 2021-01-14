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
    /// 电极表达式
    /// </summary>
    public class ElectrodeExpression
    {

        public ElectrodeExpression()
        {

        }
        /// <summary>
        /// 创建表达式
        /// </summary>
        /// <param name="zDatum"></param>
        /// <param name="pre"></param>
        /// <returns></returns>
        public bool CreateExp(bool zDatum, int[] pre)
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
                ExpressionUtils.SetAttrExp("EleHeight", "EleHeight", NXObject.AttributeType.Real);

                ExpressionUtils.CreateExp("moveZ=0", "Number");

                ExpressionUtils.CreateExp("xNCopies=PitchXNum", "Number");
                ExpressionUtils.CreateExp("yNCopies=PitchYNum", "Number");
                ExpressionUtils.CreateExp("xPitchDistance=PitchX", "Number");
                ExpressionUtils.CreateExp("yPitchDistance=-PitchY", "Number");
                ExpressionUtils.CreateExp("extrudePreparation=ceiling(PreparationZ-EleHeight-2)", "Number");
                if (zDatum)
                {
                    ExpressionUtils.CreateExp("moveBoxZ=0", "Number");
                    if (pre[0] >= pre[1])
                    {
                        ExpressionUtils.CreateExp("moveY=-(yNCopies-1)*yPitchDistance/2", "Number");
                        ExpressionUtils.CreateExp("moveX=-(xNCopies-2)*xPitchDistance/2", "Number");
                        ExpressionUtils.CreateExp("moveBoxX=-(xNCopies)*xPitchDistance/2", "Number");
                        ExpressionUtils.CreateExp("moveBoxY=0", "Number");
                    }
                    else
                    {
                        ExpressionUtils.CreateExp("moveX=-(xNCopies-1)*xPitchDistance/2", "Number");
                        ExpressionUtils.CreateExp("moveY=-(yNCopies-2)*yPitchDistance/2", "Number");
                        ExpressionUtils.CreateExp("moveBoxY=-(yNCopies)*yPitchDistance/2", "Number");
                        ExpressionUtils.CreateExp("moveBoxX=0", "Number");
                    }

                }
                else
                {
                    ExpressionUtils.CreateExp("moveX=-(xNCopies-1)*xPitchDistance/2", "Number");
                    ExpressionUtils.CreateExp("moveY=-(yNCopies-1)*yPitchDistance/2", "Number");
                }
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
