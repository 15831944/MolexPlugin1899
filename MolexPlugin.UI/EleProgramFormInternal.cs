using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NXOpen;
using Basic;
using MolexPlugin.Model;
using MolexPlugin.DAL;
using System.Data;

namespace MolexPlugin
{
    public partial class EleProgramForm
    {
        private void SetEleInfo()
        {
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
            foreach (Part pt in elePart)
            {
                if (ParentAssmblieInfo.IsElectrode(pt))
                {
                    ElectrodeModel em = new ElectrodeModel(pt);
                    try
                    {
                        em.Info.AllInfo.CreateDataRow(ref table);
                    }
                    catch (Exception ex)
                    {
                        ClassItem.WriteLogFile(em.AssembleName + "          创建行错误！" + ex.Message);
                    }
                }
                else
                {
                    DataRow row = table.NewRow();
                    row["EleName"] = pt.Name;
                }

            }
            dataGridViewEle.DataSource = table;

        }
    }
}
