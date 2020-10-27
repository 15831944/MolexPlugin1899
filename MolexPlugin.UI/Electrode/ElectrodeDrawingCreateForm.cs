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
    public class ElectrodeDrawingCreateForm
    {
        private ASMCollection asmColl;
        private ASMModel asm = null;
        private void ShowForm()
        {
            ElectrodeDrawingForm form = new ElectrodeDrawingForm(asm, asmColl);
            IntPtr intPtr = NXOpenUI.FormUtilities.GetDefaultParentWindowHandle();
            NXOpenUI.FormUtilities.ReparentForm(form);
            NXOpenUI.FormUtilities.SetApplicationIcon(form);
            Application.Run(form);
            form.Dispose();
        }

        private bool PartIsAsm()
        {
            Part workPart = Session.GetSession().Parts.Work;
            if (!ParentAssmblieInfo.IsAsm(workPart))
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
                    NXOpen.UI.GetUI().NXMessageBox.Show("提示", NXMessageBox.DialogType.Error, wk.AssembleName + "没有检查电极");
                    return false;
                }

            }
            return true;
        }
        public void Show()
        {
            UserSingleton user = UserSingleton.Instance();
            if (user.UserSucceed && user.Jurisd.GetElectrodeJurisd())
            {
                if (PartIsAsm())
                {
                    bool anyPartsModified1;
                    NXOpen.PartSaveStatus partSaveStatus1;
                    Session.GetSession().Parts.SaveAll(out anyPartsModified1, out partSaveStatus1);
                    ShowForm();
                }
            }
        }
    }
}
