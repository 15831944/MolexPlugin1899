using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NXOpen;
using NXOpen.CAM;
using Basic;
using MolexPlugin.Model;
using MolexPlugin.DAL;
using System.IO;

namespace MolexPlugin
{
    public partial class PostShopdoc : Form
    {
        private ProgramModel model;
        private List<ProgramModel> models = new List<ProgramModel>();
        private List<NCGroup> groups = new List<NCGroup>();
        public PostShopdoc(List<ProgramModel> models)
        {
            this.models = models;
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            Part workPart = Session.GetSession().Parts.Work;
            if (ParentAssmblieInfo.IsElectrode(workPart))
            {
                this.listBoxPostName.SelectedIndex = 4;
            }
            foreach (ProgramModel np in models)
            {
                groups.Add(np.ProgramGroup);
                ListViewItem lv = new ListViewItem();
                lv.SubItems.Add(np.ProgramGroup.Name);
                lv.Checked = true;
                listViewProgram.Items.Add(lv);
            }
        }

        private void buttCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonPost_Click(object sender, EventArgs e)
        {
            if (buttonPost.Text.Equals("后处理"))
                buttonPost.Text = "不后处理";
            else
                buttonPost.Text = "后处理";
        }

        private void buttonShopdoc_Click(object sender, EventArgs e)
        {
            if (buttonShopdoc.Text.Equals("产生工单"))
                buttonShopdoc.Text = "不产生工单";
            else
                buttonShopdoc.Text = "产生工单";
        }

        private void buttAllSelet_Click(object sender, EventArgs e)
        {
            if (buttAllSelet.Text.Equals("全选"))
            {
                buttAllSelet.Text = "单选";
                for (int i = 0; i < listViewProgram.Items.Count; i++)
                {
                    listViewProgram.Items[i].Checked = false;
                }
            }

            else
            {
                buttAllSelet.Text = "全选";
                for (int i = 0; i < listViewProgram.Items.Count; i++)
                {
                    listViewProgram.Items[i].Checked = true;
                }

            }
        }

        private void buttOk_Click(object sender, EventArgs e)
        {
            Part workPart = Session.GetSession().Parts.Work;
            PartPostBuilder post = new PartPostBuilder(workPart);
            List<NCGroup> postGroup = new List<NCGroup>();
            if(this.listBoxPostName.SelectedItem==null)
            {
                MessageBox.Show("请选择后处理格式。", "提示！", MessageBoxButtons.OK);
                return;
            }        
            if (buttonShopdoc.Text.Equals("产生工单"))
            {
                CreatePostExcelBuilder excel = new CreatePostExcelBuilder(this.models, workPart);
                excel.CreateExcel();
            }
            if (buttonPost.Text == "后处理")
            {
                for (int i = 0; i < listViewProgram.Items.Count; i++)
                {
                    if (listViewProgram.Items[i].Checked)
                    {
                        postGroup.Add(groups[i]);
                    }
                }

                if (this.listBoxPostName.SelectedItem.ToString().Equals("Electrode", StringComparison.CurrentCultureIgnoreCase))
                {
                    string[] name = post.GetElectrodePostName(groups);
                    foreach (string str in name)
                    {
                        post.Post(str, postGroup.ToArray());
                    }
                }
                else
                {
                    post.Post(this.listBoxPostName.SelectedItem.ToString(), postGroup.ToArray());
                }
            }

            this.Close();
        }

    }
}

