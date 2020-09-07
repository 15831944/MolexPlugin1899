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
    /// Z向电极表达式
    /// </summary>
    public class ZElectrodeExpression : AbstractElectrodeExpression
    {

        public override bool CreateExp(bool zDatum, int[] pre = null)
        {
            bool isok = base.CreateDefault();
            if (!isok)
                return false;
            try
            {
                ExpressionUtils.CreateExp("xNCopies=PitchXNum", "Number");
                ExpressionUtils.CreateExp("yNCopies=PitchYNum", "Number");
                ExpressionUtils.CreateExp("xPitchDistance=PitchX", "Number");
                ExpressionUtils.CreateExp("yPitchDistance=-PitchY", "Number");
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
                return isok;
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("创建表达式错误！" + ex.Message);
                return false;
            }
        }



    }
}
