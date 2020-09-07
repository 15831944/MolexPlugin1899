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
    /// Y向电极表达式
    /// </summary>
    public class YElectrodeExpression : AbstractElectrodeExpression
    {
      
        public override bool CreateExp(bool zDatum, int[] pre = null)
        {
            bool isok = base.CreateDefault();
            try
            {
                ExpressionUtils.CreateExp("xNCopies=PitchYNum", "Number");
                ExpressionUtils.CreateExp("xPitchDistance=Pitchx", "Number");
                if (zDatum)
                {
                    ExpressionUtils.CreateExp("moveX=-(xNCopies-2)*xPitchDistance/2", "Number");
                    ExpressionUtils.CreateExp("moveBoxX=-(xNCopies)*xPitchDistance/2", "Number");                   
                    ExpressionUtils.CreateExp("moveBoxY=0", "Number");
                    ExpressionUtils.CreateExp("moveBoxZ=0", "Number");
                }
                else
                {
                    ExpressionUtils.CreateExp("moveX=-(xNCopies-1)*xPitchDistance/2", "Number");
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
