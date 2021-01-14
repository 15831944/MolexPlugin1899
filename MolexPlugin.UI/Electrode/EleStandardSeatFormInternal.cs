using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;
using NXOpen.BlockStyler;
using NXOpen.Utilities;
using Basic;
using MolexPlugin.DAL;
using MolexPlugin.Model;
using System.IO;
using System.Windows.Forms;

namespace MolexPlugin
{
    public partial class EleStandardSeatForm
    {
        /// <summary>
        /// 预览
        /// </summary>
        private void CreatePreview()
        {
            ElectrodePitchInfo pitch = new ElectrodePitchInfo();
            pitch.PitchX = double.Parse(this.textBox_pitchX.Text);
            pitch.PitchXNum = int.Parse(this.textBox_pitchXNum.Text);
            pitch.PitchY = double.Parse(this.textBox_pitchY.Text);
            pitch.PitchYNum = int.Parse(this.textBox_pitchYNum.Text);
            this.preveiw.UpdatePattern(pitch);
            Point3d value = this.condition.ExpAndMatr.Matr.GetHeadSetValue(GetElePitchInfo(), checkBox1.Checked);
            double[] pre = this.condition.ExpAndMatr.Matr.GetPreparation(pitch, checkBox1.Checked);


            this.textBox_eleX.Text = value.X.ToString();
            this.textBox_eleY.Text = value.Y.ToString();
            this.textBox_eleZ.Text = value.Z.ToString();

            this.textBox_preparationX.Text = pre[0].ToString();
            this.textBox_preparationY.Text = pre[1].ToString();
            this.textBox_preparationZ.Text = pre[2].ToString();
            Session.GetSession().Parts.Work.ModelingViews.WorkView.Regenerate();
        }
        /// <summary>
        /// 初始化对话框
        /// </summary>
        private void InitializeForm()
        {
            comboBox_material.Items.AddRange(GetContr("Material").ToArray());
            comboBox_material.SelectedIndex = 0;
            comboBox_Condition.Items.AddRange(GetContr("Condition").ToArray());
            comboBox_Condition.SelectedIndex = 0;
            comboBox_eleType.Items.AddRange(GetContr("EleType").ToArray());
            comboBox_remarks.Items.AddRange(GetContr("Remarks").ToArray());
            comboBox_technology.Items.AddRange(GetContr("Technology").ToArray());
            comboBox_technology.SelectedIndex = 1;
            comboBox_cam.Items.AddRange(GetContr("Templates").ToArray());

            comboBox_crudeInter.Enabled = false;
            comboBox_crudeNum.Enabled = false;
            comboBox_duringInter.Enabled = false;
            comboBox_duringNum.Enabled = false;
            comboBox_fineInter.Text = "0.05";
            comboBox_fineNum.Text = "1";
            checkBox_fine.Checked = true;

            textBox_pitchYNum.Text = "1";
            textBox_pitchXNum.Text = "1";
            textBox_pitchX.Text = "0";
            textBox_pitchY.Text = "0";
            comboBox_Ch.Text = "18";
            textBox_Ext.Text = "1.3";
            this.textBox_name.Text = GetDefEleName(this.parent.MoldInfo);
            SetPichContrShow();
            CreatePreview();
        }
        /// <summary>
        /// 获取数据控件类型
        /// </summary>
        /// <param name="controlType"></param>
        /// <returns></returns>
        private List<string> GetContr(string controlType)
        {
            List<string> control = new List<string>();
            var temp = ControlDeserialize.Controls.GroupBy(a => a.ControlType);
            foreach (var i in temp)
            {
                if (i.Key == controlType)
                {
                    foreach (var k in i)
                    {
                        control.Add(k.EnumName);
                    }
                }
            }
            return control;
        }
        /// <summary>
        /// 获取电极名称
        /// </summary>
        /// <returns></returns>
        private ElectrodeNameInfo GetEleNameInfo()
        {
            ElectrodeNameInfo name = new ElectrodeNameInfo();
            name.EleName = this.textBox_name.Text;
            name.EleNumber = name.GetEleNumber(this.textBox_name.Text);
            name.BorrowName = "";
            return name;
        }
        /// <summary>
        /// 获取Pitch
        /// </summary>
        /// <returns></returns>
        private ElectrodePitchInfo GetElePitchInfo()
        {
            ElectrodePitchInfo pitch = new ElectrodePitchInfo();
            pitch.PitchX = Math.Abs(double.Parse(this.textBox_pitchX.Text));
            pitch.PitchXNum = int.Parse(this.textBox_pitchXNum.Text);
            pitch.PitchY = Math.Abs(double.Parse(this.textBox_pitchY.Text));
            pitch.PitchYNum = int.Parse(this.textBox_pitchYNum.Text);
            return pitch;
        }
        /// <summary>
        /// 获取备料
        /// </summary>
        /// <returns></returns>
        private ElectrodePreparationInfo GetElePre()
        {
            ElectrodePreparationInfo prep = new ElectrodePreparationInfo();
            prep.Preparation[0] = int.Parse(this.textBox_preparationX.Text);
            prep.Preparation[1] = int.Parse(this.textBox_preparationY.Text);
            prep.Preparation[2] = int.Parse(this.textBox_preparationZ.Text);
            prep.IsPreparation = pre.IsPreCriterion(prep.Preparation);
            prep.Material = this.comboBox_material.Text;
            return prep;
        }
        /// <summary>
        /// 获取设定值
        /// </summary>
        /// <returns></returns>
        private ElectrodeSetValueInfo GetEleSetValue()
        {
            ElectrodeSetValueInfo setValue = new ElectrodeSetValueInfo();
            setValue.EleSetValue[0] = double.Parse(this.textBox_eleX.Text);
            setValue.EleSetValue[1] = double.Parse(this.textBox_eleY.Text);
            setValue.EleSetValue[2] = double.Parse(this.textBox_eleZ.Text);
            setValue.ContactArea = condition.ToolhInfo[0].GetAllContactArea();
            setValue.ProjectedArea = condition.ToolhInfo[0].GetProjectedArea(work.Info.Matr);
            setValue.Positioning = condition.ToolhInfo[0].ToolhName;
            return setValue;
        }
        /// <summary>
        /// 获取电极间隙
        /// </summary>
        /// <returns></returns>
        private ElectrodeGapValueInfo GetEleGapValue()
        {
            ElectrodeGapValueInfo gapValue = new ElectrodeGapValueInfo();
            if (checkBox_crude.Checked)
            {
                string cru = this.comboBox_crudeInter.Text;
                string num = this.comboBox_crudeNum.Text;
                if (cru != "")
                {
                    gapValue.CrudeInter = double.Parse(cru);

                }
                if (num != "")
                {
                    gapValue.CrudeNum = int.Parse(num);
                }
            }
            if (checkBox_during.Checked)
            {
                string cru = this.comboBox_duringInter.Text;
                string num = this.comboBox_duringNum.Text;
                if (cru != "")
                {
                    gapValue.DuringInter = double.Parse(cru);
                }
                if (num != "")
                {
                    gapValue.DuringNum = int.Parse(num);
                }
            }
            if (checkBox_fine.Checked)
            {
                string cru = this.comboBox_fineInter.Text;
                string num = this.comboBox_fineNum.Text;
                if (cru != "")
                {
                    gapValue.FineInter = double.Parse(cru);
                }
                if (num != "")
                {
                    gapValue.FineNum = int.Parse(num);
                }
            }
            gapValue.ERNum = er;
            return gapValue;
        }
        /// <summary>
        /// 获取电极基准
        /// </summary>
        /// <returns></returns>
        private ElectrodeDatumInfo GetEleDatumInfo()
        {
            ElectrodeDatumInfo datum = new ElectrodeDatumInfo();
            datum.ExtrudeHeight = double.Parse(this.textBox_Ext.Text);
            datum.DatumHeigth = 2;
            datum.DatumWidth = 1;
            datum.EleHeight = this.condition.ExpAndMatr.Matr.GetZHeight(datum.ExtrudeHeight);
            datum.EleProcessDir = this.condition.ExpAndMatr.Matr.EleProcessDir;
            return datum;
        }
        /// <summary>
        /// 获取电极CAM
        /// </summary>
        /// <returns></returns>
        private ElectrodeCAMInfo GetEleCamInfo()
        {
            ElectrodeCAMInfo cam = new ElectrodeCAMInfo();
            cam.CamTemplate = this.comboBox_cam.Text;
            cam.EleHeadDis = condition.ExpAndMatr.Matr.GetHeadDis();
            cam.EleMinDim = condition.AskMinDim();
            return cam;
        }
        /// <summary>
        /// 获取电极备注
        /// </summary>
        /// <returns></returns>
        private ElectrodeRemarksInfo GetEleRemarksInfo()
        {
            ElectrodeRemarksInfo re = new ElectrodeRemarksInfo();
            re.Technology = this.comboBox_technology.Text;
            re.Ch = "CH" + this.comboBox_Ch.Text;
            re.Condition = this.comboBox_Condition.Text;
            string temp = "";
            if (this.checkBox_rotate.Checked)
                temp += "旋转电极";
            if (this.checkBox_clearEngle.Checked)
                temp += "清角电极";
            if (this.checkBox_wedm.Checked)
                temp += "线割电极";
            if (this.checkBox_clearGlitch.Checked)
                temp += "去毛刺电极";
            if (this.checkBox_gate.Checked)
                temp += "浇口电极";
            re.ElePresentation = temp;

            re.EleType = this.comboBox_eleType.Text;
            re.Remarks = this.comboBox_remarks.Text;
            return re;
        }
        /// <summary>
        /// 反求单齿设置值
        /// </summary>
        /// <param name="setValue"></param>
        /// <param name="pitch"></param>
        /// <param name="pre"></param>
        /// <param name="zDatum"></param>
        /// <returns></returns>
        private Point3d GetSingleHeadSetValue(ElectrodeSetValueInfo setValue, ElectrodePitchInfo pitch, ElectrodePreparationInfo pre, bool zDatum)
        {
            Point3d temp = new Point3d(setValue.EleSetValue[0], setValue.EleSetValue[1], setValue.EleSetValue[2]);
            double x1 = temp.X - (pitch.PitchXNum - 1) * pitch.PitchX / 2;
            double y1 = temp.Y - (pitch.PitchYNum - 1) * pitch.PitchY / 2;
            if (zDatum)
            {
                if (pre.Preparation[0] > pre.Preparation[1])
                {
                    x1 = temp.X - (pitch.PitchXNum - 2) * pitch.PitchX / 2;
                }
                else
                {
                    y1 = temp.Y - (pitch.PitchYNum - 2) * pitch.PitchY / 2;
                }
            }
            return new Point3d(x1, y1, temp.Z);
        }
        /// <summary>
        /// 默认电极名
        /// </summary>
        /// <returns></returns>
        private string GetDefEleName(MoldInfo mold)
        {
            WorkCollection wc = new WorkCollection(mold);

            if (wc.Electrodes.Count == 0)
            {
                return mold.MoldNumber + "-" + mold.WorkpieceNumber + "E1";
            }
            else
            {
                ElectrodeNameInfo nameInfo = (wc.Electrodes[wc.Electrodes.Count - 1].Info).AllInfo.Name;
                string name = nameInfo.EleName.Substring(0, nameInfo.EleName.LastIndexOf("E"));
                return name + "E" + (nameInfo.EleNumber + 1).ToString();
            }

        }
        /// <summary>
        /// 获取ER个数
        /// </summary>
        /// <returns></returns>
        private void GetERNumber(ElectrodePitchInfo pitch)
        {
            ElectrodeERForm erForm = new ElectrodeERForm(er, pitch);
            if (checkBox_crude.Checked)
            {
                string cru = this.comboBox_crudeInter.Text;
                string num = this.comboBox_crudeNum.Text;
                if (cru != "" && num == "")
                {
                    erForm.Name = "ER个数";
                    erForm.ShowDialog(this);
                }
            }
            if (checkBox_during.Checked)
            {
                string cru = this.comboBox_duringInter.Text;
                string num = this.comboBox_duringNum.Text;
                if (cru != "" && num == "")
                {
                    erForm.Name = "DR个数";
                    erForm.ShowDialog(this);
                }

            }

        }
    }
}
