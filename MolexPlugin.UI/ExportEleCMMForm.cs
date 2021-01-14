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
using System.IO;

namespace MolexPlugin
{
    public partial class ExportEleCMMForm : Form
    {
        private List<Part> elePart = new List<Part>();
        public ExportEleCMMForm()
        {
            InitializeComponent();
            this.elePart = GetElePartForCamInfo();
            this.elePart.Sort(delegate (Part a, Part b)
            {
                return a.Name.CompareTo(b.Name);

            });
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
            List<string> err = new List<string>();
            string ftp = @"\\10.221.167.49\cmm_cyc_root\";
            for (int i = 0; i < listViewEleInfo.Items.Count; i++)
            {
                if (listViewEleInfo.Items[i].Checked)
                {
                    string oldPath = this.elePart[i].FullPath;
                    string newPath = ftp + GetEleNameForCMM(this.elePart[i]) + ".prt";
                    try
                    {
                        File.Copy(oldPath, newPath, true);
                    }
                    catch (Exception ex)
                    {
                        err.Add(ex.Message);
                    }
                }
            }
            if (err.Count != 0)
            {
                ClassItem.Print(err.ToArray());
            }
            this.Close();
        }

        private string GetEleNameForCMM(Part ele)
        {
            ElectrodeNameInfo nameInfo = ElectrodeNameInfo.GetAttribute(ele);
            MoldInfo moldInfo = MoldInfo.GetAttribute(ele);
            return moldInfo.MoldNumber + "-" + moldInfo.WorkpieceNumber + "-E" + nameInfo.EleNumber.ToString();
        }
    }
}
