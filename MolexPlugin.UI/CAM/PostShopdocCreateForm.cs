using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NXOpen;
using NXOpen.CAM;
using Basic;
using MolexPlugin.Model;
using MolexPlugin.DAL;

namespace MolexPlugin
{
    public class PostShopdocCreateForm
    {
        private static NCGroup group;

        private static void ShowForm(List<ProgramModel> groups)
        {
            PostShopdoc form = new PostShopdoc(groups);
            IntPtr intPtr = NXOpenUI.FormUtilities.GetDefaultParentWindowHandle();
            NXOpenUI.FormUtilities.ReparentForm(form);
            NXOpenUI.FormUtilities.SetApplicationIcon(form);
            Application.Run(form);
            form.Dispose();
        }

        private static bool PartIsAsm(out List<ProgramModel> groups)
        {
            Session theSession = Session.GetSession();
            NXOpen.UI theUI = NXOpen.UI.GetUI();
            groups = new List<ProgramModel>();
            if (!theSession.ApplicationName.Equals("UG_APP_MANUFACTURING", StringComparison.CurrentCultureIgnoreCase))
            {
                theUI.NXMessageBox.Show("错误", NXMessageBox.DialogType.Error, "请切换到加工模块");
                return false;
            }
            group = GetNCGroup();
            if (group == null)
            {
                theUI.NXMessageBox.Show("错误", NXMessageBox.DialogType.Error, "没法找到AAA程序组");
                return false;
            }

            ProgramModel model = new ProgramModel(group);
            if (model.OperData.Count > 0)
            {
                theUI.NXMessageBox.Show("错误", NXMessageBox.DialogType.Error, "程序组错误");
                return false;
            }
            foreach (NCGroup np in model.GetProgram())
            {
                if (np.GetMembers().Length > 0)
                {
                    ProgramModel nc = new ProgramModel(np);
                    if (!nc.IsOperation() || !nc.Estimate)
                    {
                        theUI.NXMessageBox.Show("错误", NXMessageBox.DialogType.Error, np.Name + "错误");
                        return false;
                    }
                    else
                    {
                        groups.Add(nc);
                    }
                }
            }
            List<string> err = new List<string>();
            foreach (ProgramModel pm in groups)
            {
                err.AddRange(pm.Gouged());
            }
            if (err.Count > 0)
                ClassItem.Print(err.ToArray());
            return true;
        }
        public static void Show()
        {
            UserSingleton user = UserSingleton.Instance();
            if (user.UserSucceed && user.Jurisd.GetCAMJurisd())
            {
                List<ProgramModel> groups = new List<ProgramModel>();
                if (PartIsAsm(out groups))
                    ShowForm(groups);
            }
        }

        private static NXOpen.CAM.NCGroup GetNCGroup()
        {
            Part workPart = Session.GetSession().Parts.Work;
            NXOpen.UI theUI = NXOpen.UI.GetUI();
            try
            {
                NXOpen.CAM.NCGroup parent = (NXOpen.CAM.NCGroup)workPart.CAMSetup.CAMGroupCollection.FindObject("AAA");
                return parent;
            }
            catch
            {
                theUI.NXMessageBox.Show("错误", NXMessageBox.DialogType.Error, "无法找到  AAA");
            }
            return null;
        }
    }
}
