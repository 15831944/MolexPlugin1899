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
    /// <summary>
    /// 程序扩展类
    /// 
    /// </summary>
    public partial class EleProgramCreateForm
    {

        private ASMCollection asmColl;
        private ASMModel asm = null;
        private void ShowForm()
        {
            UserSingleton user = UserSingleton.Instance();
            if (user.UserSucceed && user.Jurisd.GetCAMJurisd())
            {
                EleProgramForm form = new EleProgramForm(user.CreatorUser);
                IntPtr intPtr = NXOpenUI.FormUtilities.GetDefaultParentWindowHandle();
                NXOpenUI.FormUtilities.ReparentForm(form);
                NXOpenUI.FormUtilities.SetApplicationIcon(form);
                Application.Run(form);
                form.Dispose();
            }

        }

        public void Show()
        {
            ShowForm();
        }

    }
}
