using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NXOpen;
using Basic;
using MolexPlugin.Model;
using MolexPlugin.DAL;

namespace MolexPlugin
{
    public class WorkpieceDrawingCreateForm
    {
        private ASMCollection asmColl;
        private void ShowForm()
        {
            WorkpieceDrawingForm form = new WorkpieceDrawingForm(asmColl);
            IntPtr intPtr = NXOpenUI.FormUtilities.GetDefaultParentWindowHandle();
            NXOpenUI.FormUtilities.ReparentForm(form);
            NXOpenUI.FormUtilities.SetApplicationIcon(form);
            Application.Run(form);
            form.Dispose();
        }

        private bool PartIsAsm()
        {
            Part workPart = Session.GetSession().Parts.Work;
            ASMModel asm = null;
            if (!ASMModel.IsAsm(workPart))
            {
                asm = ASMCollection.GetAsmModel(workPart);
                if (asm == null)
                {
                    UI.GetUI().NXMessageBox.Show("提示", NXMessageBox.DialogType.Error, "无法通过工作部件找到ASM");
                    return false;
                }
                PartUtils.SetPartDisplay(asm.PartTag);
            }
            else
            {
                asm = new ASMModel(workPart);
            }
            asmColl = new ASMCollection(asm);
            foreach (WorkModel wk in asmColl.GetWorks())
            {
                bool isInter = AttributeUtils.GetAttrForBool(wk.PartTag, "Interference");
                if (!isInter)
                {
                    UI.GetUI().NXMessageBox.Show("提示", NXMessageBox.DialogType.Error, wk.AssembleName + "没有检查电极");
                    return false;
                }

            }
            return true;
        }
        public void Show()
        {
            UserSingleton user = UserSingleton.Instance();
            if (PartIsAsm() && user.UserSucceed && user.Jurisd.GetElectrodeJurisd())
                ShowForm();
        }
    }
}
