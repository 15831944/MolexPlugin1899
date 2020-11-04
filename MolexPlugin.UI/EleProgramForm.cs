using System;
using System.IO;
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
using MolexPlugin.Model;

namespace MolexPlugin
{
    public partial class EleProgramForm : Form
    {
        private UserModel model;
        private List<string> filePath = new List<string>();
        private ElectrodeCAMFile file = new ElectrodeCAMFile();
        private List<Part> elePart = new List<Part>();
        private List<CreateElectrodeCAMBuilder> camBd = new List<CreateElectrodeCAMBuilder>();
        private OperationTreeListViewInfo operInfo = null;
        private object copy = null;
        private AbstractCreateOperation addOper = null;
        private string fileSave = "";
        public EleProgramForm(UserModel model)
        {
            InitializeComponent();
            InitializeTreeListView();
            this.model = model;
            dataGridViewEle.RowsDefaultCellStyle.BackColor = Color.Bisque;
            dataGridViewEle.AlternatingRowsDefaultCellStyle.BackColor = Color.Beige; //交替行不同颜色
            dataGridViewEle.Columns[1].Frozen = true; //冻结首列
            dataGridViewEle.AutoGenerateColumns = false;
            this.listBoxTemplate.Text = "直电极";
            this.butTemplate.Enabled = false;
            this.groupBut.Enabled = false;
            this.groupAddOper.Enabled = false;
            this.groupParameter.Enabled = false;
            this.label.Text = "";
            this.comboBoxRefTool.Hide();
            this.labelRefTool.Hide();
        }

        private void butOpen_Click(object sender, EventArgs e)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            Session theSession = Session.GetSession();
            ClassItem.StatusMessge("选择电极文件");
            List<string> elePath = new List<string>();
            if (elePart.Count == 0)
            {
                elePath = file.CopyFile();
            }
            else
            {
                string fileName = Path.GetDirectoryName(elePart[0].FullPath) + "\\";
                elePath = file.AddFile(fileName);
            }
            List<string> errd = new List<string>();
            if (elePath.Count > 0)
            {
                foreach (string st in elePath)
                {
                    Tag partTag;
                    UFPart.LoadStatus err;
                    try
                    {
                        theUFSession.Part.Open(st, out partTag, out err);
                        elePart.Add(NXObjectManager.Get(partTag) as Part);
                    }
                    catch (NXException ex)
                    {
                        errd.Add(st + "             " + ex.Message);
                    }
                }
                if (errd.Count != 0)
                {
                    ClassItem.Print(errd.ToArray());
                }
                SetEleInfo();
                DispSeleteEle();
                this.butTemplate.Enabled = true;
            }

        }

        private void but_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 关闭对话框前事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EleProgramForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (Part pt in elePart)
            {
                pt.Close(BasePart.CloseWholeTree.False, BasePart.CloseModified.CloseModified, null);
            }
            file.DeleteFile();
        }
        /// <summary>
        /// 选择电极行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewEle_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DispSeleteEle();
        }
        /// <summary>
        /// 选择模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxTemplate_MouseClick(object sender, MouseEventArgs e)
        {
            string type = this.listBoxTemplate.Text;
            string dllPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string pngName = dllPath.Replace("application\\", "Images\\") + type + ".bmp";
            if (File.Exists(pngName))
            {
                this.png.Image = Image.FromFile(pngName);
            }
            else
            {
                this.png.Image = null;
            }
        }
        /// <summary>
        /// 套用模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butTemplate_Click(object sender, EventArgs e)
        {
            Part pt = FindSelectEle();
            if (pt != null)
            {
                PartUtils.SetPartDisplay(pt);
                PartUtils.SetPartWork(null);
                CreateElectrodeCAMBuilder cc = GetBuilder(pt, GetTemplate());
                if (cc != null)
                {
                    if (cc.Template == null)
                        cc.CreateOperationNameModel(GetTemplate());
                    this.operInfo = new OperationTreeListViewInfo(cc.Template);
                    ShowTreeInfo(this.operInfo.TreeInfo);
                    if (!this.camBd.Exists(a => a.Pt.Equals(pt)))
                        this.camBd.Add(cc);
                }
            }
        }
        /// <summary>
        /// 树右键菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeListViewOper_CellRightClick(object sender, BrightIdeasSoftware.CellRightClickEventArgs e)
        {
            if (this.operInfo == null)
            {
                this.contextMenuStrip.Enabled = false;
            }
            else
            {
                this.contextMenuStrip.Enabled = true;
                if (this.copy == null)
                {
                    this.toolSkick.Enabled = false;
                }
                else
                {
                    this.toolSkick.Enabled = true;
                }
            }

        }

        /// <summary>
        /// 选择行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeListViewOper_CellClick(object sender, BrightIdeasSoftware.CellClickEventArgs e)
        {

            if (this.treeListViewOper.SelectedObject != null)
            {
                this.groupBut.Enabled = true;
                AddGrogram();

            }
            else
            {
                this.comboBoxProgram.Items.Clear();
                this.comboBoxRefTool.Items.Clear();
                this.comboBoxTool.Items.Clear();
                this.groupBut.Enabled = false;
                this.groupParameter.Enabled = false;
                this.groupAddOper.Enabled = false;
                this.label.Text = "";
            }
        }
        #region 菜单事件
        /// <summary>
        /// 菜单向上移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolUp_Click(object sender, EventArgs e)
        {
            if (this.operInfo != null && this.treeListViewOper.SelectedObject is ElectrodeCAMTreeInfo)
            {
                bool isok = this.operInfo.MoveUp(this.treeListViewOper.SelectedObject as ElectrodeCAMTreeInfo);
                if (isok)
                {
                    ShowTreeInfo(this.operInfo.TreeInfo);
                    ClassItem.StatusMessge("移动成功");
                }

            }

        }
        /// <summary>
        /// 向下移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolDown_Click(object sender, EventArgs e)
        {
            if (this.operInfo != null && this.treeListViewOper.SelectedObject is ElectrodeCAMTreeInfo)
            {
                bool isok = this.operInfo.MoveDown(this.treeListViewOper.SelectedObject as ElectrodeCAMTreeInfo);
                if (isok)
                {
                    ShowTreeInfo(this.operInfo.TreeInfo);
                    ClassItem.StatusMessge("移动成功");
                }
            }

        }
        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolCopy_Click(object sender, EventArgs e)
        {
            if (this.operInfo != null && this.treeListViewOper.SelectedObject is ElectrodeCAMTreeInfo)
            {
                copy = this.operInfo.Copy(this.treeListViewOper.SelectedObject as ElectrodeCAMTreeInfo);
                if (copy != null)
                {
                    this.toolSkick.Enabled = true;
                    ClassItem.StatusMessge("复制成功");
                }
            }

        }
        /// <summary>
        /// 粘贴
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolSkick_Click(object sender, EventArgs e)
        {
            if (this.operInfo != null && this.treeListViewOper.SelectedObject is ElectrodeCAMTreeInfo && copy != null)
            {
                bool isok = this.operInfo.Stick(this.treeListViewOper.SelectedObject as ElectrodeCAMTreeInfo, copy);
                if (isok)
                {
                    ShowTreeInfo(this.operInfo.TreeInfo);
                    ClassItem.StatusMessge("粘贴成功");
                }

            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolDelete_Click(object sender, EventArgs e)
        {
            if (this.operInfo != null && this.treeListViewOper.SelectedObject is ElectrodeCAMTreeInfo)
            {
                bool isok = this.operInfo.Delete(this.treeListViewOper.SelectedObject as ElectrodeCAMTreeInfo);
                if (isok)
                {
                    ShowTreeInfo(this.operInfo.TreeInfo);
                    ClassItem.StatusMessge("删除成功");
                }
            }
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolUpdate_Click(object sender, EventArgs e)
        {
            if (this.operInfo != null)
            {
                this.operInfo.Update();
                ShowTreeInfo(this.operInfo.TreeInfo);
                ClassItem.StatusMessge("更新成功");
            }
        }

        private void butDown_Click(object sender, EventArgs e)
        {
            if (this.operInfo != null && this.treeListViewOper.SelectedObject is ElectrodeCAMTreeInfo)
            {
                bool isok = this.operInfo.MoveDown(this.treeListViewOper.SelectedObject as ElectrodeCAMTreeInfo);
                if (isok)
                {
                    ShowTreeInfo(this.operInfo.TreeInfo);
                    ClassItem.StatusMessge("移动成功");
                }
            }
        }

        private void butUp_Click(object sender, EventArgs e)
        {
            if (this.operInfo != null && this.treeListViewOper.SelectedObject is ElectrodeCAMTreeInfo)
            {
                bool isok = this.operInfo.MoveUp(this.treeListViewOper.SelectedObject as ElectrodeCAMTreeInfo);
                if (isok)
                {
                    ShowTreeInfo(this.operInfo.TreeInfo);
                    ClassItem.StatusMessge("移动成功");
                }

            }
        }

        private void butCopy_Click(object sender, EventArgs e)
        {
            if (this.operInfo != null && this.treeListViewOper.SelectedObject is ElectrodeCAMTreeInfo)
            {
                copy = this.operInfo.Copy(this.treeListViewOper.SelectedObject as ElectrodeCAMTreeInfo);
                if (copy != null)
                {
                    ShowTreeInfo(this.operInfo.TreeInfo);
                    this.toolSkick.Enabled = true;
                    ClassItem.StatusMessge("复制成功");
                }
            }

        }

        private void butSkick_Click(object sender, EventArgs e)
        {
            if (this.operInfo != null && this.treeListViewOper.SelectedObject is ElectrodeCAMTreeInfo && copy != null)
            {
                bool isok = this.operInfo.Stick(this.treeListViewOper.SelectedObject as ElectrodeCAMTreeInfo, copy);
                if (isok)
                {
                    ShowTreeInfo(this.operInfo.TreeInfo);
                    ClassItem.StatusMessge("粘贴成功");
                }

            }
        }

        private void butDelete_Click(object sender, EventArgs e)
        {
            if (this.operInfo != null && this.treeListViewOper.SelectedObject is ElectrodeCAMTreeInfo)
            {
                bool isok = this.operInfo.Delete(this.treeListViewOper.SelectedObject as ElectrodeCAMTreeInfo);
                if (isok)
                {
                    ShowTreeInfo(this.operInfo.TreeInfo);
                    ClassItem.StatusMessge("删除成功");
                }
            }
        }

        private void butUpdate_Click(object sender, EventArgs e)
        {
            if (this.operInfo != null)
            {
                this.operInfo.Update();
                ShowTreeInfo(this.operInfo.TreeInfo);
                ClassItem.StatusMessge("更新成功");
            }
        }
        #endregion
        #region 添加刀路
        private void butAdd_Click(object sender, EventArgs e)
        {
            string progName = this.comboBoxProgram.Text;
            string toolName = this.comboBoxTool.Text;
            string refName = this.comboBoxRefTool.Text;
            if (refName != null && !refName.Equals(""))
            {
                if (this.addOper is TwiceRoughCreateOperation)
                {
                    (this.addOper as TwiceRoughCreateOperation).SetReferencetool(refName);
                }
                if (this.addOper is FlowCutCreateOperation)
                {
                    (this.addOper as FlowCutCreateOperation).SetReferencetool(refName);
                }
            }
            if (progName != null && !progName.Equals("") && this.addOper != null)
            {
                ElectrodeCAMTreeInfo info = GetInfoForProgramName(progName);
                if (info != null && info.Program is ProgramOperationName)
                {
                    ProgramOperationName pn = (info.Program as ProgramOperationName);
                    if (!pn.ToolName.Equals(toolName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        pn.ToolName = toolName;
                        if (!pn.Oper.Exists(a => a.Equals(this.addOper)))
                        {
                            this.addOper.SetToolName(toolName);
                            this.addOper.SetProgramName(progName);
                            this.operInfo.AddOperation(this.addOper, info);
                            this.addOper = null;
                        }
                    }
                    else
                    {
                        if (!pn.Oper.Exists(a => a.Equals(this.addOper)))
                        {
                            this.addOper.SetToolName(toolName);
                            this.addOper.SetProgramName(progName);
                            this.operInfo.AddOperation(this.addOper, info);
                            this.addOper = null;
                        }
                    }
                    ShowTreeInfo(this.operInfo.TreeInfo);
                }
            }
        }

        private void butTwicRough_Click(object sender, EventArgs e)
        {

            addOper = new TwiceRoughCreateOperation(10, "EM3");
            addOper.CreateOperationName(10);
            AddGrogram();
        }

        private void butPlanar_Click(object sender, EventArgs e)
        {
            addOper = new PlanarMillingCreateOperation(10, "EM2.98");
            addOper.CreateOperationName(10);
            AddGrogram();
        }

        private void butFace_Click(object sender, EventArgs e)
        {
            addOper = new FaceMillingCreateOperation(10, "EM2.98");
            addOper.CreateOperationName(10);
            AddGrogram();
        }

        private void butZlevel_Click(object sender, EventArgs e)
        {
            addOper = new ZLevelMillingCreateOperation(10, "BN0.98");
            addOper.CreateOperationName(10);
            AddGrogram();
        }

        private void butSurface_Click(object sender, EventArgs e)
        {
            addOper = new SurfaceContourCreateOperation(10, "BN0.98");
            addOper.CreateOperationName(10);
            AddGrogram();
        }

        private void butFolw_Click(object sender, EventArgs e)
        {
            addOper = new FlowCutCreateOperation(10, "BN0.98");
            addOper.CreateOperationName(10);
            AddGrogram();
        }
        #endregion

        private void butSave_Click(object sender, EventArgs e)
        {
            this.fileSave = file.SaveFilePath();
            if (!Directory.Exists(this.fileSave + "\\"))
            {
                Directory.CreateDirectory(this.fileSave + "\\");
            }
        }

        private void butOK_Click(object sender, EventArgs e)
        {
            List<string> err = new List<string>();
            if (this.fileSave.Equals(""))
            {
                this.fileSave = file.GetSaveFilePath();
            }
            if (this.fileSave.Equals(""))
            {
                ClassItem.MessageBox("请选择保存文件位置", NXMessageBox.DialogType.Error);
                return;
            }
            foreach (CreateElectrodeCAMBuilder cb in this.camBd)
            {
                if (cb.Template.Type == ElectrodeTemplate.User)
                {
                    err.AddRange(cb.CreateUserOperation());
                }
                else
                {
                    err.AddRange(cb.CreateOperation());
                }

                cb.SetGenerateToolPath(this.checkBoxIsGenerate.Checked);
                err.AddRange(cb.ExportFile(this.fileSave, true));
                elePart.Remove(cb.Pt);
            }
            if (err.Count > 0)
            {
                ClassItem.Print(err.ToArray());
            }
            this.Close();
        }

        private void ToolAdd_Click(object sender, EventArgs e)
        {
            this.butOpen_Click(sender, e);
        }

        private void Tooldelete2_Click(object sender, EventArgs e)
        {
            Part pt = FindSelectEle();
            if (pt != null)
            {
                string partPath = pt.FullPath;
                if (elePart.Exists(a => a.Equals(pt)))
                    elePart.Remove(pt);
                CreateElectrodeCAMBuilder tem = camBd.Find(a => a.Pt.Equals(pt));
                if (tem != null)
                {
                    camBd.Remove(tem);
                }
                pt.Close(BasePart.CloseWholeTree.False, BasePart.CloseModified.CloseModified, null);
                if (File.Exists(partPath))
                    File.Delete(partPath);
                SetEleInfo();
            }
        }
    }
}
