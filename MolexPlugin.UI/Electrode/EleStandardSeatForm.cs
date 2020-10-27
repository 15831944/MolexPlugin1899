using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NXOpen;
using NXOpen.UF;
using MolexPlugin.Model;
using MolexPlugin.DAL;
using Basic;

namespace MolexPlugin
{
    public partial class EleStandardSeatForm : Form
    {
        private EletrodePreparation pre;
        private ElectrodeCreateCondition condition;
        private ParentAssmblieInfo parent;
        private WorkModel work;
        private ElectrodePreveiw preveiw;
        private int[] er = new int[2];


        public EleStandardSeatForm(ElectrodeCreateCondition condition, WorkModel work, ParentAssmblieInfo parent)
        {
            this.condition = condition;
            this.parent = parent;
            this.work = work;
            preveiw = new ElectrodePreveiw(condition.HeadBodys, work.Info.Matr);
            InitializeComponent();
            InitializeForm();
        }


        #region 过滤
        private void InputPlusInt(KeyPressEventArgs e)
        {
            if (!((e.KeyChar >= 48 && e.KeyChar <= 57) || e.KeyChar == 8)) // 过滤只能输入正整数
            {
                e.Handled = true;
            }
        }

        private void InputPlusDouble(KeyPressEventArgs e)
        {
            if (!((e.KeyChar >= 48 && e.KeyChar <= 57) || e.KeyChar == '.' || e.KeyChar == 8)) // 过滤只能输入正double
            {
                e.Handled = true;
            }
        }

        private void InputDouble(KeyPressEventArgs e)
        {
            if (!((e.KeyChar >= 48 && e.KeyChar <= 57) || e.KeyChar == '.' || e.KeyChar == 8 || e.KeyChar == '-')) // 过滤只能输入double
            {
                e.Handled = true;
            }
        }

        private void textBox_pitchX_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputPlusDouble(e);
        }

        private void textBox_pitchY_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputDouble(e);
        }

        private void textBox_eleX_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputPlusDouble(e);
        }

        private void textBox_eleY_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputDouble(e);
        }

        private void textBox_eleZ_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputDouble(e);
        }

        private void textBox_pitchXNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputPlusInt(e);
        }

        private void textBox_pitchYNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputPlusInt(e);
        }

        private void textBox_preparationX_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputPlusInt(e);
        }

        private void textBox_preparationY_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputPlusInt(e);
        }

        private void textBox_preparationZ_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputPlusInt(e);
        }

        private void comboBox_crudeNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputPlusInt(e);
        }

        private void comboBox_duringNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputPlusInt(e);
        }

        private void comboBox_fineNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputPlusInt(e);
        }

        private void textBox_CH_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputPlusInt(e);
        }

        private void comboBox_crudeInter_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputPlusDouble(e);
        }

        private void comboBox_duringInter_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputPlusDouble(e);
        }

        private void comboBox_fineInter_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputPlusDouble(e);
        }

        private void textBox_Ext_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputDouble(e);
        }
        #endregion



        #region 预览
        private void textBox_pitchX_Leave(object sender, EventArgs e)
        {
            CreatePreview();
        }


        private void textBox_pitchXNum_Leave(object sender, EventArgs e)
        {
            CreatePreview();
        }

        private void textBox_pitchY_Leave(object sender, EventArgs e)
        {
            CreatePreview();
        }

        private void textBox_pitchYNum_Leave(object sender, EventArgs e)
        {
            CreatePreview();
        }

        private void button_X_Click(object sender, EventArgs e)
        {
            double temp = Convert.ToDouble(this.textBox_pitchX.Text);
            this.textBox_pitchX.Text = (-temp).ToString();
            CreatePreview();
        }

        private void button_Y_Click(object sender, EventArgs e)
        {
            double temp = Convert.ToDouble(this.textBox_pitchY.Text);
            this.textBox_pitchY.Text = (-temp).ToString();
            CreatePreview();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CreatePreview();
        }
        #endregion

        /// <summary>
        /// 设置PICH控件显示
        /// </summary>
        private void SetPichContrShow()
        {

            if (condition.ExpAndMatr.Matr is XNegativeElectrodeMatrix || condition.ExpAndMatr.Matr is XPositiveElectrodeMatrix)
            {
                this.textBox_pitchX.Enabled = false;
                this.textBox_pitchXNum.Enabled = false;
            }
            if (condition.ExpAndMatr.Matr is YNegativeElectrodeMatrix || condition.ExpAndMatr.Matr is YPositiveElectrodeMatrix)
            {
                this.textBox_pitchY.Enabled = false;
                this.textBox_pitchYNum.Enabled = false;
            }
        }
        /// <summary>
        /// 获取属性
        /// </summary>
        private ElectrodeAllInfo GetEleInfo()
        {
            ElectrodeAllInfo all = new ElectrodeAllInfo()
            {
                CAM = GetEleCamInfo(),
                Datum = GetEleDatumInfo(),
                GapValue = GetEleGapValue(),
                Name = GetEleNameInfo(),
                Pitch = GetElePitchInfo(),
                Preparetion = GetElePre(),
                Remarks = GetEleRemarksInfo(),
                SetValue = GetEleSetValue()
            };
            return all;
        }

        private void buttOK_Click(object sender, EventArgs e)
        {
            preveiw.DelePattern();
            if (comboBox_eleType.Text == null || comboBox_eleType.Text == "")
            {
                NXOpen.UI.GetUI().NXMessageBox.Show("错误！", NXMessageBox.DialogType.Error, "请选择电极类型！");
                return;
            }
            ElectrodeAllInfo all = GetEleInfo();
            GetERNumber(all.Pitch);
            CreateElectrode create = new CreateElectrode(all, parent, condition, this.checkBox1.Checked);
            List<string> err = create.CreateBuider();
            condition.Work.SetInterference(false);
            Session.GetSession().Parts.Work.ModelingViews.WorkView.Regenerate();
            this.Close();
            if (err.Count > 0)
            {
                ClassItem.Print(err.ToArray());
            }
        }

        private void buttCancel_Click(object sender, EventArgs e)
        {
            preveiw.DelePattern();
            Session theSession = Session.GetSession();
            bool marksRecycled1;
            bool undoUnavailable1;
            theSession.UndoLastNVisibleMarks(1, out marksRecycled1, out undoUnavailable1);
            this.Close();
        }

        private void checkBox_crude_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_crude.Checked)
            {
                this.comboBox_crudeInter.Enabled = true;
                this.comboBox_crudeNum.Enabled = true;
            }
            else
            {
                this.comboBox_crudeInter.Enabled = false;
                this.comboBox_crudeNum.Enabled = false;
            }

        }

        private void checkBox_during_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_during.Checked)
            {
                this.comboBox_duringInter.Enabled = true;
                this.comboBox_duringNum.Enabled = true;
            }
            else
            {
                this.comboBox_duringInter.Enabled = false;
                this.comboBox_duringNum.Enabled = false;
            }

        }

        private void checkBox_fine_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_fine.Checked)
            {
                this.comboBox_fineInter.Enabled = true;
                this.comboBox_fineNum.Enabled = true;
            }
            else
            {
                this.comboBox_fineInter.Enabled = false;
                this.comboBox_fineNum.Enabled = false;
            }


        }
        /// <summary>
        /// 跳标准料
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_eleType_Leave(object sender, EventArgs e)
        {

            if (this.comboBox_material.Text.Equals("紫铜"))
            {
                pre = new EletrodePreparation("CuLength", "CuWidth");
            }
            else
            {
                pre = new EletrodePreparation("WuLength", "WuWidth");
            }
            int x = int.Parse(this.textBox_preparationX.Text);
            int y = int.Parse(this.textBox_preparationY.Text);
            int z = int.Parse(this.textBox_preparationZ.Text);
            int[] temp = new int[2] { x, y };
            pre.GetPreparation(ref temp);

            this.textBox_preparationX.Text = temp[0].ToString();
            this.textBox_preparationY.Text = temp[1].ToString();
        }

        private void EleStandardSeatForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27)
            {
                this.buttCancel_Click(sender, e);
            }
        }

    }
}
