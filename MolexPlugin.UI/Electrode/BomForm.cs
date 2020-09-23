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
using NXOpen;

namespace MolexPlugin
{
    public partial class BomForm : Form
    {
        private ASMModel asm;
        private ASMCollection asmColl;
        private ElectrodeAllInfo oldEleInfo;
        private object oldValue;
        private DataGridViewTextBoxEditingControl CellEdit = null;
        private List<ElectrodeModel> eleModels = new List<ElectrodeModel>();
        private List<ElectrodeModel> collEle = new List<ElectrodeModel>();

        private List<ElectrodeAllInfo> newEleModel = new List<ElectrodeAllInfo>();
        public BomForm(ASMModel asm, ASMCollection asmColl)
        {
            InitializeComponent();
            this.asm = asm;
            this.asmColl = asmColl;
            eleModels = GetEles();
            Initialize();
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            foreach (ElectrodeAllInfo info in newEleModel)
            {
                List<ElectrodeModel> ems = collEle.FindAll(a => a.Info.AllInfo.Name.EleName.Equals(info.Name.EleName));
                foreach (ElectrodeModel em in ems)
                {
                    ElectrodeUpdateBuilder update = new ElectrodeUpdateBuilder(em, info, asm.PartTag);
                    update.UpdateEleBuilder();
                    update.UpdateDrawing();
                }
            }
            if (!asm.Info.MoldInfo.EditionNumber.Equals(this.textBox_EditionNumber.Text))
            {
                UpdateEditionNumber(this.textBox_EditionNumber.Text);
            }
            this.Close();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void button_OutExcel_Click(object sender, EventArgs e)
        {

        }


        #region //控件过滤
        private void Cells_KeyPress1(object sender, KeyPressEventArgs e) //自定义事件
        {
            if (!((e.KeyChar >= 48 && e.KeyChar <= 57) || e.KeyChar == '.' || e.KeyChar == 8 || e.KeyChar == '-')) // 过滤只能输入double
            {
                e.Handled = true;
            }
        }

        private void Cells_KeyPress2(object sender, KeyPressEventArgs e) //自定义事件
        {
            if (!((e.KeyChar >= 48 && e.KeyChar <= 57) || e.KeyChar == '.' || e.KeyChar == 8)) // 过滤只能输入正double
            {
                e.Handled = true;
            }
        }

        private void Cells_KeyPress3(object sender, KeyPressEventArgs e) //自定义事件
        {
            if (!((e.KeyChar >= 48 && e.KeyChar <= 57) || e.KeyChar == 8)) // 过滤只能输入正整数
            {
                e.Handled = true;
            }
        }


        private void dataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {

            //if (dataGridView.CurrentCellAddress.X == 4 || dataGridView.CurrentCellAddress.X == 6)
            //{
            //    CellEdit = (DataGridViewTextBoxEditingControl)e.Control;
            //    CellEdit.SelectAll();
            //    CellEdit.KeyPress -= Cells_KeyPress1; //移除绑定事件
            //    CellEdit.KeyPress -= Cells_KeyPress2;
            //    CellEdit.KeyPress -= Cells_KeyPress3;
            //    CellEdit.KeyPress += Cells_KeyPress3; //过滤只能输入double
            //}
            int[] column = { 5, 6, 8, 9, 11, 13, 15, 20, 21, 22 };
            if (Array.IndexOf(column, dataGridView.CurrentCellAddress.X) != -1)
            {
                CellEdit = (DataGridViewTextBoxEditingControl)e.Control;
                CellEdit.SelectAll();
                CellEdit.KeyPress -= Cells_KeyPress1; //移除绑定事件
                CellEdit.KeyPress -= Cells_KeyPress2;
                CellEdit.KeyPress -= Cells_KeyPress3;
                CellEdit.KeyPress += Cells_KeyPress3; //过滤只能输入正整数
            }
            int[] column1 = { 4, 7, 10, 12, 14, 23, 24 };
            if (Array.IndexOf(column1, dataGridView.CurrentCellAddress.X) != -1)
            {
                CellEdit = (DataGridViewTextBoxEditingControl)e.Control;
                CellEdit.SelectAll();
                CellEdit.KeyPress -= Cells_KeyPress1; //移除绑定事件
                CellEdit.KeyPress -= Cells_KeyPress2;
                CellEdit.KeyPress -= Cells_KeyPress3;
                CellEdit.KeyPress += Cells_KeyPress2; //绑定正double事件
            }
        }
        #endregion

        /// <summary>
        /// 开始事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            oldValue = this.dataGridView.CurrentCell.Value;
            DataRow dr = (dataGridView.Rows[e.RowIndex].DataBoundItem as DataRowView).Row;
            oldEleInfo = ElectrodeAllInfo.GetInfoForDataRow(dr);
            if (((oldEleInfo.GapValue.CrudeInter != 0 && oldEleInfo.GapValue.CrudeNum == 0) || (oldEleInfo.GapValue.DuringInter != 0 && oldEleInfo.GapValue.DuringNum == 0))
               && (oldEleInfo.GapValue.FineInter != 0 && oldEleInfo.GapValue.FineNum == 1))
            {

            }
            else
            {
                if (e.ColumnIndex == 6 || e.ColumnIndex == 9)
                    e.Cancel = true;
            }
        }
        /// <summary>
        /// 结束事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (oldValue != this.dataGridView.CurrentCell.Value)
            {
                DataRow dr = (dataGridView.Rows[e.RowIndex].DataBoundItem as DataRowView).Row;
                ElectrodeAllInfo newInfo = ElectrodeAllInfo.GetInfoForDataRow(dr);
                if (dataGridView.CurrentCellAddress.X == 4 || dataGridView.CurrentCellAddress.X == 5 || dataGridView.CurrentCellAddress.X == 7
              || dataGridView.CurrentCellAddress.X == 8)
                {

                    ElectrodePitchUpdate pitch = new ElectrodePitchUpdate(oldEleInfo.Pitch, newInfo.Pitch);
                    double[] setValue = pitch.GetNewSetValue(oldEleInfo.SetValue.EleSetValue);

                    dataGridView.Rows[e.RowIndex].Cells[1].Value = setValue[0].ToString("f3");
                    dataGridView.Rows[e.RowIndex].Cells[2].Value = setValue[1].ToString("f3");

                    int[] pre = pitch.GetNewPreparation(oldEleInfo.Preparetion.Preparation, newInfo.Preparetion.Material);

                    dataGridView.Rows[e.RowIndex].Cells[20].Value = pre[0].ToString();
                    dataGridView.Rows[e.RowIndex].Cells[21].Value = pre[1].ToString();
                }
                if (!newEleModel.Exists(a => a.Name.EleName.Equals(newInfo.Name.EleName, StringComparison.CurrentCultureIgnoreCase)))
                    this.newEleModel.Add(newInfo);
                else
                {
                    ElectrodeAllInfo bu = newEleModel.Find(a => a.Name.EleName.Equals(newInfo.Name.EleName, StringComparison.CurrentCultureIgnoreCase));
                    this.newEleModel.Remove(bu);
                    this.newEleModel.Add(newInfo);
                }

            }
        }
    }
}
