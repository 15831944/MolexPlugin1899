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
using NXOpen.UF;
using NXOpen.BlockStyler;
using MolexPlugin.DAL;
using MolexPlugin.Model;
using Basic;
using System.Diagnostics;

namespace MolexPlugin
{
    public partial class ExportEleCamForm : Form
    {
        private List<Part> elePart = new List<Part>();
        public ExportEleCamForm()
        {
            InitializeComponent();
            this.elePart = GetElePartForCamInfo();
            foreach (Part pt in this.elePart)
            {
                ListViewItem lv = new ListViewItem();
                lv.SubItems.Add(pt.Name);
                lv.Checked = true;
                listViewEleInfo.Items.Add(lv);
            }
        }
        /// <summary>
        ///获取有电极程序模板的电极
        /// </summary>
        /// <returns></returns>
        private List<Part> GetElePartForCamInfo()
        {
            List<Part> elePart = new List<Part>();

            Session theSession = Session.GetSession();
            foreach (Part pt in theSession.Parts)
            {
                if (ParentAssmblieInfo.IsElectrode(pt))
                {
                    ElectrodeCAMInfo cam = ElectrodeCAMInfo.GetAttribute(pt);
                    if (cam.CamTemplate == null || cam.CamTemplate != "")
                        elePart.Add(pt);
                }

            }
            return elePart;
        }

        private void butSave_Click(object sender, EventArgs e)
        {
            new ElectrodeCAMFile().SaveFilePath();
        }

        private void but_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void butOK_Click(object sender, EventArgs e)
        {
            string path = "";
            for (int i = 0; i < listViewEleInfo.Items.Count; i++)
            {
                if (listViewEleInfo.Items[i].Checked)
                {
                    path += "\"" + this.elePart[i].FullPath + "\" ";
                }
            }
            //ProcessStartInfo startInfo = new ProcessStartInfo("C:\\Program Files\\Siemens\\NX1899\\NXBIN\\BatchElectrodeOperation.exe", path);
            ////设置不在新窗口中启动新的进程
            //startInfo.CreateNoWindow = false;
            ////不使用操作系统使用的shell启动进程
            //startInfo.UseShellExecute = false;
            ////将输出信息重定向
            //startInfo.RedirectStandardOutput = true;
            //Process process = Process.Start(startInfo); ;
            //process.WaitForExit();
            Process process = Process.Start("C:\\Program Files\\Siemens\\NX1899\\NXBIN\\BatchElectrodeOperation.exe", path);
            process.WaitForExit(1);
             
            this.Close();
        }
    }
}
