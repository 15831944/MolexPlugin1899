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
using NXOpen;

namespace MolexPlugin
{
    public partial class ElectrodeERForm : Form
    {
        int[] er;
        ElectrodePitchInfo pitch;
        public ElectrodeERForm(int[] er, ElectrodePitchInfo pitch)
        {
            this.er = er;
            this.pitch = pitch;
            InitializeComponent();
            this.textBoxXNumber.Text = "1";
            this.textBox_YNumber.Text = "1";
        }

        private void buttOK_Click(object sender, EventArgs e)
        {
            Geter();
            if (er[0] > pitch.PitchXNum || er[1] > pitch.PitchYNum)
            {
                this.textBoxXNumber.Text = "1";
                this.textBox_YNumber.Text = "1";
                NXOpen.UI.GetUI().NXMessageBox.Show("错误！", NXMessageBox.DialogType.Error, "输入错误！");
                return;
            }
            this.Close();
        }

        private void buttCancel_Click(object sender, EventArgs e)
        {
            er[0] = 0;
            er[0] = 0;
            this.Close();
        }
        private void Geter()
        {
            er[0] = int.Parse(this.textBoxXNumber.Text);
            er[1] = int.Parse(this.textBox_YNumber.Text);
        }
    }
}
