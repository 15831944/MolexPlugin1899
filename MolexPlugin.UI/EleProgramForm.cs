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
using NXOpen.Utilities;
using NXOpen.UF;
using Basic;
using MolexPlugin.DAL;

namespace MolexPlugin
{
    public partial class EleProgramForm : Form
    {
        private List<string> filePath = new List<string>();
        private ElectrodeCAMFile file = new ElectrodeCAMFile();
        private List<Part> elePart = new List<Part>();
        public EleProgramForm()
        {
            InitializeComponent();
            dataGridViewEle.RowsDefaultCellStyle.BackColor = Color.Bisque;
            dataGridViewEle.AlternatingRowsDefaultCellStyle.BackColor = Color.Beige; //交替行不同颜色
            dataGridViewEle.Columns[1].Frozen = true; //冻结首列
            dataGridViewEle.AutoGenerateColumns = false;
        }

        private void butOpen_Click(object sender, EventArgs e)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            List<string> elePath = file.CopyFile();
            if (elePath.Count > 0)
            {
                foreach (string st in elePath)
                {
                    Tag partTag;
                    UFPart.LoadStatus err;
                    theUFSession.Part.Open(st, out partTag, out err);
                    elePart.Add(NXObjectManager.Get(partTag) as Part);
                }
                SetEleInfo();
            }

        }

        private void but_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
