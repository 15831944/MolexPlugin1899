using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using NXOpen;
using NXOpen.UF;
using NXOpen.BlockStyler;
using NXOpen.Utilities;
using Basic;
using MolexPlugin.DAL;
using MolexPlugin.Model;
using System.IO;
using System.Windows.Forms;
using System.Data;

namespace MolexPlugin
{
    public partial class BomForm
    {
        /// <summary>
        /// 获取所有的电极
        /// </summary>
        /// <returns></returns>
        private List<ElectrodeModel> GetEles()
        {
            collEle = asmColl.GetElectrodes();
            collEle.Sort(delegate (ElectrodeModel a, ElectrodeModel b)
            {
                return a.Info.AllInfo.Name.EleName.CompareTo(b.Info.AllInfo.Name.EleName);
            });
            List<ElectrodeModel> eleModels = new List<ElectrodeModel>();
            foreach (ElectrodeModel em in collEle)
            {
                if ((em.Info.AllInfo.SetValue.Positioning == "" || em.Info.AllInfo.SetValue.Positioning.Equals("A", StringComparison.CurrentCultureIgnoreCase)) &&
                    !eleModels.Exists(a => a.Info.AllInfo.Name.EleName.Equals(em.Info.AllInfo.Name.EleName, StringComparison.CurrentCultureIgnoreCase)))
                    eleModels.Add(em);
            }
            return eleModels;
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
        /// 初始化
        /// </summary>
        private void Initialize()
        {
            this.textBox_MoldNumber.Text = asm.Info.MoldInfo.MoldNumber;
            this.textBox_EditionNumber.Text = asm.Info.MoldInfo.EditionNumber;
            EleType.Items.AddRange(GetContr("EleType").ToArray());
            Material.Items.AddRange(GetContr("Material").ToArray());
            Condition.Items.AddRange(GetContr("Condition").ToArray());
            dataGridView.Columns["EleX"].Visible = false; //隐藏列
            dataGridView.Columns["EleY"].Visible = false;
            dataGridView.Columns["EleZ"].Visible = false;
            dataGridView.Columns["EleName"].ReadOnly = true;  //只读列
            dataGridView.RowsDefaultCellStyle.BackColor = Color.Bisque;
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.Beige; //交替行不同颜色
            dataGridView.Columns[1].Frozen = true; //冻结首列
            dataGridView.AutoGenerateColumns = false;

            DataTable table;        
            try
            {
                table = ElectrodeAllInfo.CreateDataTable();
            }
            catch (Exception ex)
            {
                ClassItem.WriteLogFile("创建表列错误！" + ex.Message);
                return;
            }
            foreach (ElectrodeModel em in eleModels)
            {
                try
                {
                    em.Info.AllInfo.CreateDataRow(ref table);
                }
                catch (Exception ex)
                {
                    ClassItem.WriteLogFile(em.AssembleName + "          创建行错误！" + ex.Message);
                }
            }
            dataGridView.DataSource = table;

        }
        /// <summary>
        /// 修改版本
        /// </summary>
        /// <param name="editionNumber"></param>
        private void UpdateEditionNumber(string editionNumber)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            Session theSession = Session.GetSession();
            MoldInfo mf = asm.Info.MoldInfo;
            mf.EditionNumber = editionNumber;
            mf.SetAttribute(asm.PartTag);
            foreach (ElectrodeModel em in asmColl.GetElectrodes())
            {
                MoldInfo mi = em.Info.MoldInfo;
                mi.EditionNumber = editionNumber;
                mi.SetAttribute(em.PartTag);
                string dwgName = em.Info.AllInfo.Name.EleName + "_dwg";
                string dwg = em.WorkpieceDirectoryPath + em.Info.AllInfo.Name.EleName + "_dwg.prt";
                if (File.Exists(dwg))
                {
                    Part dwgPart = null;
                    try
                    {
                        dwgPart = theSession.Parts.FindObject(dwgName) as Part;
                        if (dwgPart != null)
                        {
                            mi.SetAttribute(dwgPart);
                            continue;
                        }
                    }
                    catch
                    {

                    }
                    Tag part;
                    UFPart.LoadStatus error_status;
                    theUFSession.Part.Open(dwg, out part, out error_status);
                    mi.SetAttribute(NXObjectManager.Get(part) as Part);
                }
            }
            foreach (MoldInfo mi in asmColl.MoldInfo)
            {
                WorkCollection wkColl = asmColl.GetWorkCollection(mi);
                foreach (WorkModel wm in wkColl.Work)
                {
                    MoldInfo wmMold = wm.Info.MoldInfo;
                    wmMold.EditionNumber = editionNumber;
                    wmMold.SetAttribute(wm.PartTag);
                }
                foreach (EDMModel em in wkColl.EdmModel)
                {
                    MoldInfo wmMold = em.Info.MoldInfo;
                    wmMold.EditionNumber = editionNumber;
                    wmMold.SetAttribute(em.PartTag);
                }
            }
            PartUtils.SetPartDisplay(asm.PartTag);

        }

    }
}
