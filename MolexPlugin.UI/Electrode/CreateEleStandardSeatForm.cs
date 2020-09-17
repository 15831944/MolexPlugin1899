using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;
using Basic;
using MolexPlugin.DAL;
using MolexPlugin.Model;
using System.Windows.Forms;
using NXOpen.Utilities;
using System.Runtime.InteropServices;

namespace MolexPlugin
{
    /// <summary>
    /// 创建基准台对话框
    /// </summary>
    public class CreateEleStandardSeatForm
    {
        private ElectrodeCreateExpAndMatr expAndMatr;
        private List<Body> headBodys = new List<Body>();
        private static UFSession theUFSession = UFSession.GetUFSession();
        private Part workPart;
        private UserSingleton user;
        public CreateEleStandardSeatForm(ElectrodeCreateExpAndMatr expAndMatr)
        {
            this.expAndMatr = expAndMatr;
            workPart = Session.GetSession().Parts.Work;
            user = UserSingleton.Instance();
        }

        /// <summary>
        /// 弹出对话框
        /// </summary>
        private void ShowForm(ElectrodeCreateCondition condition, ParentAssmblieInfo parent, WorkModel work)
        {
            EleStandardSeatForm form = new EleStandardSeatForm(condition, work, parent);
            IntPtr intPtr = NXOpenUI.FormUtilities.GetDefaultParentWindowHandle();
            NXOpenUI.FormUtilities.ReparentForm(form);
            NXOpenUI.FormUtilities.SetApplicationIcon(form);
            // form.Show();
            Application.Run(form);
            form.Dispose();
        }

        public void Show()
        {
            if (user.UserSucceed && user.Jurisd.GetElectrodeJurisd())
            {
                Session.UndoMarkId markId;
                markId = Session.GetSession().SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "基准台");
                if (!WorkModel.IsWork(workPart))
                {
                    UI.GetUI().NXMessageBox.Show("错误", NXMessageBox.DialogType.Error, "请设置WORK为工作部件");
                    return;
                }
                WorkModel work = new WorkModel(workPart);
                Matrix4 mat = (work.Info).Matr;
                Part workpiece = work.GetHostWorkpiece();
                if (workpiece == null)
                {
                    UI.GetUI().NXMessageBox.Show("错误", NXMessageBox.DialogType.Error, "无法找到主件");
                    return;
                }
                List<Body> headBodys = SelectObject();
                if (headBodys == null || headBodys.Count == 0)
                    return;
                ElectrodeCreateCondition condition = new ElectrodeCreateCondition(expAndMatr, headBodys, work, workpiece);
                if (expAndMatr.Matr.AnalyeBackOffFace())
                {
                    int ok = UI.GetUI().NXMessageBox.Show("错误", NXMessageBox.DialogType.Question, "电极有倒扣！");
                    if (ok != 1)
                        return;
                }
                ParentAssmblieInfo parent = new ParentAssmblieInfo(work.Info.MoldInfo, user.CreatorUser);
                ShowForm(condition, parent, work);
            }

        }

        /// <summary>
        /// 选择对话框
        /// </summary>
        private List<Body> SelectObject()
        {
            List<Body> bodys = new List<Body>();
            string msg = "请选择电极头";
            string title = "电极基座";
            int res = 0;
            int connt = 0;
            IntPtr usr_data = IntPtr.Zero;
            Tag[] bodyObj;
            theUFSession.Ui.SelectWithClassDialog(msg, title, UFConstants.UF_UI_SEL_SCOPE_WORK_PART, sele_in_proc, usr_data, out res, out connt, out bodyObj);
            if (bodyObj.Length == 0)
            {
                return null;
            }

            for (int i = 0; i < bodyObj.Length; i++)
            {
                Body body = NXObjectManager.Get(bodyObj[i]) as Body;
                bodys.Add(body);
                body.Unhighlight();
            }
            return bodys;
        }

        /// <summary>
        /// 过滤选择
        /// </summary>
        /// <param name="select"></param>
        /// <param name="user_data"></param>
        /// <returns></returns>
        private static int sele_in_proc(IntPtr select, IntPtr user_data)
        {
            int num_triples = 1;
            UFUi.Mask[] triples = new UFUi.Mask[1];
            triples[0].object_type = 70;
            triples[0].solid_type = 0;
            triples[0].object_subtype = 0;
            theUFSession.Ui.SetSelMask(select, UFUi.SelMaskAction.SelMaskClearAndEnableSpecific, num_triples, triples);
            return UFConstants.UF_UI_SEL_SUCCESS;
        }

    }



}
