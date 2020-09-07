using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;
using MolexPlugin.Model;
using NXOpen.Features;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 预览
    /// </summary>
    public class ElectrodePreveiw
    {

        private List<Body> bodys;
        private Matrix4 matr;
        private PatternGeometry patternFeat;
        public ElectrodePreveiw(List<Body> bodys, Matrix4 matr)
        {
            this.matr = matr;
            this.bodys = bodys;
        }
        /// <summary>
        /// 创建阵列
        /// </summary>
        private void CreatePattern(ElectrodePitchInfo pitch)
        {
            if ((pitch.PitchXNum > 1 && Math.Abs(pitch.PitchX) > 0) || (pitch.PitchYNum > 1 && Math.Abs(pitch.PitchY) > 0))
            {
                CreateExpression(pitch);
                this.patternFeat = PatternUtils.CreatePattern("xNCopies", "xPitchDistance", "yNCopies",
                    "yPitchDistance", this.matr, this.bodys.ToArray());
            }

        }
        /// <summary>
        /// 删除表达式
        /// </summary>
        private void DeleExpression()
        {
            ExpressionUtils.DeteteExp("xPitchDistance");
            ExpressionUtils.DeteteExp("xNCopies");
            ExpressionUtils.DeteteExp("yPitchDistance");
            ExpressionUtils.DeteteExp("yNCopies");
        }

        private void CreateExpression(ElectrodePitchInfo pitch)
        {
            ExpressionUtils.CreateExp("xNCopies=" + pitch.PitchXNum.ToString(), "Number");
            ExpressionUtils.CreateExp("yNCopies=" + pitch.PitchYNum.ToString(), "Number");
            ExpressionUtils.CreateExp("xPitchDistance=" + pitch.PitchX.ToString(), "Number");
            ExpressionUtils.CreateExp("yPitchDistance=" + pitch.PitchY.ToString(), "Number");
        }
        /// <summary>
        /// 更新阵列
        /// </summary>
        /// <param name="x"></param>
        /// <param name="xNumber"></param>
        /// <param name="y"></param>
        /// <param name="yNumber"></param>
        public void UpdatePattern(ElectrodePitchInfo pitch)
        {
            Session theSession = Session.GetSession();
            NXOpen.Session.UndoMarkId markId;
            markId = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "NX update");
            if (this.patternFeat == null)
            {
                CreatePattern(pitch);
                return;
            }       
            ExpressionUtils.UpdateExp("xPitchDistance", pitch.PitchX.ToString());
            ExpressionUtils.UpdateExp("xNCopies", pitch.PitchXNum.ToString());
            ExpressionUtils.UpdateExp("yPitchDistance", pitch.PitchY.ToString());
            ExpressionUtils.UpdateExp("yNCopies", pitch.PitchYNum.ToString());
            DeleteObject.UpdateObject(markId, "NX update");

        }
        /// <summary>
        /// 删除阵列
        /// </summary>
        public void DelePattern()
        {
            Session theSession = Session.GetSession();
            NXOpen.Session.UndoMarkId markId;
            markId = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "NX update");
            if (patternFeat != null)
            {
                DeleteObject.Delete(this.patternFeat);
                DeleExpression();
            }
            DeleteObject.UpdateObject(markId, "NX update");
        }
    }
}

