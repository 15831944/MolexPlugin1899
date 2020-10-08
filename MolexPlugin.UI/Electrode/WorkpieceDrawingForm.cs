using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MolexPlugin.Model;
using MolexPlugin.DAL;
using Basic;
using NXOpen;

namespace MolexPlugin
{
    public partial class WorkpieceDrawingForm : Form
    {
        ASMCollection asmColl;
        public WorkpieceDrawingForm(ASMCollection asmColl)
        {
            InitializeComponent();
            this.asmColl = asmColl;
            List<WorkModel> work = asmColl.GetWorks();
            work.Sort(delegate (WorkModel a, WorkModel b)
            {
                return a.AssembleName.CompareTo(b.AssembleName);
            }
            );
            foreach (WorkModel wk in work)
            {
                ListViewItem lv = new ListViewItem();
                lv.SubItems.Add(wk.AssembleName);
                // lv.SubItems.Add(work.WorkNumber.ToString());
                lv.Checked = true;
                listView.Items.Add(lv);
            }
        }

        private void buttCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttAllSelet_Click(object sender, EventArgs e)
        {
            if (buttAllSelet.Text == "全选")
            {
                buttAllSelet.Text = "单选";

                for (int i = 0; i < listView.Items.Count; i++)
                {
                    listView.Items[i].Checked = false;
                }
            }
            else
            {
                buttAllSelet.Text = "全选";

                for (int i = 0; i < listView.Items.Count; i++)
                {
                    listView.Items[i].Checked = true;
                }
            }
        }

        private void buttOk_Click(object sender, EventArgs e)
        {
            Part workPart = Session.GetSession().Parts.Work;
            List<string> err = new List<string>();
            for (int i = 0; i < listView.Items.Count; i++)
            {
                if (listView.Items[i].Checked)
                {
                    string workName = listView.Items[i].SubItems[1].Text.ToString();
                    foreach (WorkModel wm in this.asmColl.GetWorks())
                    {
                        if (wm.AssembleName.Equals(workName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            try
                            {
                                WorkDrawingBuilder builder = new WorkDrawingBuilder(wm);
                                builder.CreateDrawing();
                            }
                            catch (Exception ex)
                            {
                                err.Add(workName + ex.Message + "    无法找到主工件，请检查工件属性！");
                            }

                        }
                    }
                }
            }
            ClassItem.theSession.ApplicationSwitchImmediate("UG_APP_MODELING");
            PartUtils.SetPartDisplay(workPart);
            if (err.Count > 0)
                ClassItem.Print(err.ToArray());
            this.Close();
        }
    }
}
