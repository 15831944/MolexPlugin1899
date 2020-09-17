using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace Basic
{
    public class ViewUtils
    {
        /// <summary>
        /// 创建视图
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="matrix3x3"></param>
        /// <returns></returns>
        public static ModelingView CreateView(string viewName, Matrix3x3 matrix3x3)
        {
            Part workPart = Session.GetSession().Parts.Work;
            try
            {
                workPart.ModelingViews.WorkView.Orient(matrix3x3);
                return workPart.Views.SaveAsPreservingCase(workPart.ModelingViews.WorkView, viewName, false, false) as ModelingView;
            }
            catch (NXException ex)
            {
                LogMgr.WriteLog("LayerUtils.CreateView            " + ex.Message);
                throw ex;
            }

        }
        /// <summary>
        /// 以名字设置工作视图
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public static ModelingView SetWorkViewForName(string viewName)
        {
            Part workPart = Session.GetSession().Parts.Work;
            NXOpen.Layout layout1 = ((NXOpen.Layout)workPart.Layouts.FindObject("L1"));
            NXOpen.ModelingView modelingView1 = null;
            try
            {
                modelingView1 = workPart.ModelingViews.FindObject(viewName) as ModelingView;
            }
            catch
            {

            }
            if (modelingView1 == null)
                return null;
            try
            {
                layout1.ReplaceView(workPart.ModelingViews.WorkView, modelingView1, true);
                return modelingView1;
            }
            catch (NXException ex)
            {
                LogMgr.WriteLog("LayerUtils.SetWorkViewForName            " + ex.Message);
                throw ex;
            }

        }
    }
}
