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
        /// <summary>
        /// 设置电极信息
        /// </summary>
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
                else if (!ParentAssmblieInfo.IsParent(pt))
                {
                    DataRow row = table.NewRow();
                    row["EleName"] = pt.Name;
                    table.Rows.Add(row);
                }

            }
            dataGridViewEle.DataSource = table;

        }
        /// <summary>
        /// 查找选择电极Part
        /// </summary>
        /// <returns></returns>
        private Part FindSelectEle()
        {
            if (dataGridViewEle.SelectedCells.Count > 0)
            {
                int intRow = dataGridViewEle.SelectedCells[0].RowIndex;
                DataRow dr = (dataGridViewEle.Rows[intRow].DataBoundItem as DataRowView).Row;
                string name = dr["EleName"].ToString();
                if (name != null && !name.Equals(""))
                {
                    Part pt = elePart.Find(a => a.Name.Equals(name));
                    if (pt != null)
                    {
                        return pt;
                    }
                }
            }
            else
            {
                ClassItem.MessageBox("请选择电极！", NXMessageBox.DialogType.Error);
            }
            return null;
        }
        /// <summary>
        /// 获取电极模板
        /// </summary>
        /// <returns></returns>
        private ElectrodeTemplate GetTemplate()
        {
            string type = this.listBoxTemplate.Text;
            if (type != null && type != "")
            {
                if (type.Equals("直电极"))
                {
                    return ElectrodeTemplate.SimplenessVerticalEleTemplate;
                }
                if (type.Equals("直+等高"))
                {
                    return ElectrodeTemplate.PlanarAndZleveEleTemplate;
                }
                if (type.Equals("直+等宽"))
                {
                    return ElectrodeTemplate.PlanarAndSufaceEleTemplate;
                }
                if (type.Equals("直+等高+等宽"))
                {
                    return ElectrodeTemplate.PlanarAndZleveAndSufaceEleTemplate;
                }
                if (type.Equals("直+等高+等宽+清根"))
                {
                    return ElectrodeTemplate.PlanarAndZleveAndSufaceAndFlowCutEleTemplate;
                }
                if (type.Equals("等宽+等高"))
                {
                    return ElectrodeTemplate.ZleveAndSufaceEleTemplate;
                }
                if (type.Equals("等宽+等高+清根"))
                {
                    return ElectrodeTemplate.ZleveAndSufaceAndFlowCutEleTemplate;
                }
                if (type.Equals("等高电极"))
                {
                    return ElectrodeTemplate.ZleveEleTemplate;
                }
                if (type.Equals("模板"))
                {
                    return ElectrodeTemplate.User;
                }
            }
            return ElectrodeTemplate.User;
        }

        private void SetTemplate(ElectrodeTemplate type)
        {
            switch (type)
            {
                case ElectrodeTemplate.SimplenessVerticalEleTemplate:
                    this.listBoxTemplate.Text = "直电极";
                    break;
                case ElectrodeTemplate.PlanarAndSufaceEleTemplate:
                    this.listBoxTemplate.Text = "直+等宽";
                    break;
                case ElectrodeTemplate.PlanarAndZleveAndSufaceEleTemplate:
                    this.listBoxTemplate.Text = "直+等高+等宽";
                    break;
                case ElectrodeTemplate.PlanarAndZleveAndSufaceAndFlowCutEleTemplate:
                    this.listBoxTemplate.Text = "直+等高+等宽+清根";
                    break;
                case ElectrodeTemplate.ZleveAndSufaceEleTemplate:
                    this.listBoxTemplate.Text = "等宽+等高";
                    break;
                case ElectrodeTemplate.ZleveAndSufaceAndFlowCutEleTemplate:
                    this.listBoxTemplate.Text = "等宽+等高+清根";
                    break;
                case ElectrodeTemplate.ZleveEleTemplate:
                    this.listBoxTemplate.Text = "等高电极";
                    break;
                case ElectrodeTemplate.User:
                    this.listBoxTemplate.Text = "模板";
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 获取特征
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="type"></param>
        private CreateElectrodeCAMBuilder GetBuilder(Part pt, ElectrodeTemplate type)
        {
            CreateElectrodeCAMBuilder cm = camBd.Find(a => a.Pt.Equals(pt));
            if (cm != null)
            {
                try
                {
                    cm.CreateOperationNameModel(type);
                    return cm;
                }
                catch (Exception ex)
                {
                    ClassItem.Print(pt.Name + "无法加载模板     " + ex.Message);
                }
            }
            else
            {
                try
                {
                    CreateElectrodeCAMBuilder cc = new CreateElectrodeCAMBuilder(pt, model);
                    cc.CreateOperationNameModel(type);
                    return cc;
                }
                catch (Exception ex)
                {
                    ClassItem.Print(pt.Name + "无法加载模板     " + ex.Message);
                }
            }

            return null;

        }
        private CreateElectrodeCAMBuilder GetBuilder(Part pt)
        {
            CreateElectrodeCAMBuilder cc = camBd.Find(a => a.Pt.Equals(pt));
            if (cc != null)
            {
                return cc;
            }
            return null;

        }
        /// <summary>
        /// 显示树列表
        /// </summary>
        /// <param name="info"></param>
        private void ShowTreeInfo(List<ElectrodeCAMTreeInfo> info)
        {
            this.treeListViewOper.SetObjects(info);
            this.treeListViewOper.DiscardAllState();
            this.treeListViewOper.ExpandAll();
        }
        /// <summary>
        ///初始化树
        /// </summary>
        private void InitializeTreeListView()
        {
            this.treeListViewOper.CanExpandGetter = delegate (object x) { return ((ElectrodeCAMTreeInfo)x).Children.Count > 0; };
            this.treeListViewOper.ChildrenGetter = delegate (object x) { return ((ElectrodeCAMTreeInfo)x).Children; };
            this.treeListViewOper.ParentGetter = delegate (object x) { return ((ElectrodeCAMTreeInfo)x).ProgramName; };
            this.olvProgram.ImageGetter = delegate (object x) { return ((ElectrodeCAMTreeInfo)x).Png; };
        }
        /// <summary>
        /// 添加程序
        /// </summary>
        private void AddGrogram()
        {
            ElectrodeCAMTreeInfo tree = this.treeListViewOper.SelectedObject as ElectrodeCAMTreeInfo;
            this.comboBoxProgram.Items.Clear();
            this.comboBoxRefTool.Items.Clear();
            this.comboBoxTool.Items.Clear();
            if (tree.Program is ProgramOperationName)
            {
                this.groupAddOper.Enabled = true;
                if (this.addOper != null)
                {
                    this.groupParameter.Enabled = true;
                    this.comboBoxProgram.Items.AddRange(GetProgram().ToArray());
                    this.comboBoxProgram.Text = (tree.ProgramName);
                    // this.comboBoxProgram.Enabled = false;
                    this.comboBoxTool.Items.AddRange(this.addOper.GetAllToolName().ToArray());
                    this.comboBoxTool.Text = (tree.Program as ProgramOperationName).ToolName;
                    List<string> refTool = this.addOper.GetRefToolName();
                    if (refTool.Count > 0)
                    {
                        this.comboBoxRefTool.Show();
                        this.labelRefTool.Show();
                        this.comboBoxRefTool.Items.AddRange(refTool.ToArray());
                    }
                    else
                    {
                        this.comboBoxRefTool.Hide();
                        this.labelRefTool.Hide();
                    }
                }
                else
                {
                    this.groupParameter.Enabled = false;
                }

                this.label.Text = "添加刀路";
            }
            else if (tree.Program is AbstractCreateOperation)
            {
                addOper = tree.Program as AbstractCreateOperation;
                this.groupParameter.Enabled = true;
                this.groupAddOper.Enabled = false;
                this.comboBoxProgram.Items.AddRange(GetProgram().ToArray());
                this.comboBoxProgram.Text = (tree.Parent.Program as ProgramOperationName).Program;
                this.comboBoxTool.Items.AddRange(this.addOper.GetAllToolName().ToArray());
                this.comboBoxTool.Text = tree.ToolName;
                List<string> refTool = this.addOper.GetRefToolName();
                if (refTool.Count > 0)
                {
                    this.comboBoxRefTool.Show();
                    this.labelRefTool.Show();
                    this.comboBoxRefTool.Items.AddRange(refTool.ToArray());
                    if (addOper is TwiceRoughCreateOperation)
                    {
                        this.comboBoxRefTool.Text = (addOper as TwiceRoughCreateOperation).ReferenceTool;
                    }
                    if (addOper is FlowCutCreateOperation)
                    {
                        this.comboBoxRefTool.Text = (addOper as FlowCutCreateOperation).ReferencetoolName;
                    }
                }
                else
                {
                    this.comboBoxRefTool.Hide();
                    this.labelRefTool.Hide();
                }
                this.label.Text = "修改刀路";
            }
        }
        /// <summary>
        /// 获取全面程序组名
        /// </summary>
        /// <returns></returns>
        private List<string> GetProgram()
        {
            List<string> program = new List<string>();

            foreach (var temp in this.treeListViewOper.Objects)
            {
                ElectrodeCAMTreeInfo ele = temp as ElectrodeCAMTreeInfo;
                program.Add(ele.ProgramName);
            }
            return program;
        }
        /// <summary>
        /// 通过名字获取treeInfo
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private ElectrodeCAMTreeInfo GetInfoForProgramName(string name)
        {
            List<ElectrodeCAMTreeInfo> info = this.treeListViewOper.Objects as List<ElectrodeCAMTreeInfo>;
            foreach (var temp in this.treeListViewOper.Objects)
            {
                ElectrodeCAMTreeInfo ele = temp as ElectrodeCAMTreeInfo;
                if (ele.ProgramName.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    return ele;
                }
            }
            return null;
        }
        /// <summary>
        /// 显示选择电极
        /// </summary>
        private void DispSeleteEle()
        {
            Part pt = FindSelectEle();
            if (pt != null)
            {
                PartUtils.SetPartDisplay(pt);
                PartUtils.SetPartWork(null);
                CreateElectrodeCAMBuilder cc = GetBuilder(pt);
                if (cc != null && cc.Template != null)
                {
                    this.operInfo = new OperationTreeListViewInfo(cc.Template);
                    ShowTreeInfo(this.operInfo.TreeInfo);
                }
                else
                {
                    ClassItem.StatusMessge("选择模板类型");                 
                    if (cc == null)
                    {
                        cc = new CreateElectrodeCAMBuilder(pt, model);
                        this.camBd.Add(cc);
                        SetTemplate(cc.GetElectrodeTemplate());
                        ShowTreeInfo(new List<ElectrodeCAMTreeInfo>());
                    }
                    else
                    {
                        SetTemplate(cc.GetElectrodeTemplate());
                        ShowTreeInfo(new List<ElectrodeCAMTreeInfo>());
                    }
                }
                this.butTemplate.Enabled = true;
            }

        }
    }
}
