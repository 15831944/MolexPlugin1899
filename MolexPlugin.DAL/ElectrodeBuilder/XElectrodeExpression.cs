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
    /// X向电极表达式
    /// </summary>
    public class XElectrodeExpression : AbstractElectrodeExpression
    {
        public override bool CreateExp(bool zDatum, int[] pre = null)
        {
            bool isok = base.CreateDefault();
            try
            {
                ExpressionUtils.CreateExp("yNCopies=PitchYNum", "Number");
                ExpressionUtils.CreateExp("yPitchDistance=PitchY", "Number");
                if (zDatum)
                {
                    ExpressionUtils.CreateExp("moveY=-(yNCopies-2)*yPitchDistance/2", "Number");
                    ExpressionUtils.CreateExp("moveBoxY=-(yNCopies)*yPitchDistance/2", "Number");
                    ExpressionUtils.CreateExp("moveBoxX=0", "Number");              
                    ExpressionUtils.CreateExp("moveBoxZ=0", "Number");
                }
                else
                {
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
