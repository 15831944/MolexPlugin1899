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
    public partial class ElectrodeDrawingForm : Form
    {
        private ASMCollection asmColl;
        ASMModel asm;
        public ElectrodeDrawingForm(ASMModel asm, ASMCollection asmColl)
        {
            InitializeComponent();
            this.asmColl = asmColl;
            this.asm = asm;
            SetListView();
        }
        private void SetListView()
        {
            List<ElectrodeModel> eleModels = new List<ElectrodeModel>();
            foreach (ElectrodeModel em in asmColl.GetElectrodes())
            {
                if (!eleModels.Exists(a => a.Info.AllInfo.Name.EleName.Equals(em.Info.AllInfo.Name.EleName, StringComparison.CurrentCultureIgnoreCase)))
                    eleModels.Add(em);
            }
            eleModels.Sort(delegate (ElectrodeModel a, ElectrodeModel b)
            {
                if (a.Info.AllInfo.Name.EleNumber == b.Info.AllInfo.Name.EleNumber)
                {
                    return a.Info.MoldInfo.WorkpieceNumber.CompareTo(b.Info.MoldInfo.WorkpieceNumber);
                }
                else
                {
                    return a.Info.AllInfo.Name.EleNumber.CompareTo(b.Info.AllInfo.Name.EleNumber);
                }

            });
            foreach (ElectrodeModel em in eleModels)
            {
                ListViewItem lv1 = new ListViewItem();
                lv1.SubItems.Add(em.Info.AllInfo.Name.EleNumber.ToString());
                lv1.SubItems.Add(em.Info.AllInfo.Name.EleName);
                lv1.Checked = true;
                listView.Items.Add(lv1);
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
            UserSingleton user = UserSingleton.Instance();
            List<string> err = new List<string>();
            if (user.UserSucceed && user.Jurisd.GetElectrodeJurisd())
            {
                List<ElectrodeModel> eleModels = asmColl.GetElectrodes();
                for (int i = 0; i < listView.Items.Count; i++)
                {
                    if (listView.Items[i].Checked)
                    {
                        string eleName = listView.Items[i].SubItems[2].Text.ToString();
                        List<ElectrodeModel> models = eleModels.FindAll(a => a.Info.AllInfo.Name.EleName.Equals(eleName, StringComparison.CurrentCultureIgnoreCase));
                        if (models.Count > 0)
                        {
                            try
                            {
                                ElectrodeDrawingModel dra = new ElectrodeDrawingModel(models, asm.PartTag, user.CreatorUser);
                                ElectrodeDrawingBuilder builder = new ElectrodeDrawingBuilder(dra, asm);
                                builder.CreateBulider();
                            }
                            catch (NXException ex)
                            {
                                err.Add(eleName + "电极出图错误！" + ex.Message);
                            }
                        }
                    }
                }
                PartUtils.SetPartDisplay(asm.PartTag);
                Session.GetSession().ApplicationSwitchImmediate("UG_APP_MODELING");
                if (err.Count > 0)
                    ClassItem.Print(err.ToArray());
            }
            this.Close();
        }
    }
}
