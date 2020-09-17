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
    public class BomCreateForm
    {

        private ASMCollection asmColl;
        private ASMModel asm = null;
        private void ShowForm()
        {
            BomForm form = new BomForm(assemble);
            IntPtr intPtr = NXOpenUI.FormUtilities.GetDefaultParentWindowHandle();
            NXOpenUI.FormUtilities.ReparentForm(form);
            NXOpenUI.FormUtilities.SetApplicationIcon(form);
            Application.Run(form);
            form.Dispose();
        }

        private bool PartIsAsm()
        {
            Part workPart = Session.GetSession().Parts.Work;
            if (!ASMModel.IsAsm(workPart))
            {
                asm = ASMCollection.GetAsmModel(workPart);
                if (asm == null)
                {
                    ClassItem.MessageBox("无法通过工作部件找到ASM！", NXMessageBox.DialogType.Error);
                    return false;
                }

                PartUtils.SetPartDisplay(asm.PartTag);
            }
            asm = new ASMModel(workPart);
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
            if (PartIsAsm())
                ShowForm();
        }

    }
}
