using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 程序组操作
    /// </summary>
    public class ProgramOperationName
    {
        private string program;
        private string tool;

        public List<AbstractCreateOperation> Oper { get; private set; } = new List<AbstractCreateOperation>();

        /// <summary>
        /// 程序组名
        /// </summary>
        public string Program
        {
            get
            {
                return program;
            }
            set
            {
                program = value;
                SetProgram(value);
            }
        }
        /// <summary>
        /// 刀具
        /// </summary>
        public string ToolName
        {
            get
            {
                return tool;
            }
            set
            {
                program = value;
                SetTool(value);
            }
        }

        public ProgramOperationName(int site, params AbstractCreateOperation[] oper)
        {
            this.program = "O" + string.Format("{0:D4}", site);
            this.Oper = oper.ToList();
            this.tool = oper[0].NameModel.ToolName;
        }
        public ProgramOperationName(string program, params AbstractCreateOperation[] oper)
        {
            this.program = program;
            this.Oper = oper.ToList();
            this.tool = oper[0].NameModel.ToolName;
        }
        /// <summary>
        /// 添加刀路名
        /// </summary>
        /// <param name="model"></param>
        public bool AddOperationNameModel(AbstractCreateOperation model, int count)
        {
            if (model.ToolName.Equals(this.tool, StringComparison.CurrentCultureIgnoreCase))
            {
                Oper.IndexOf(model, count);
                UpdateProgramName();
                return true;
            }
            return false;
        }
        /// <summary>
        /// 删除刀路
        /// </summary>
        /// <param name="model"></param>
        public void DeleteOperationNameModel(AbstractCreateOperation model)
        {
            Oper.Remove(model);
            UpdateProgramName();
        }

        public void DeleteOperationNameModel(int count)
        {
            Oper.RemoveAt(count);
            UpdateProgramName();
        }
        /// <summary>
        /// 更新列表名
        /// </summary>
        public void UpdateProgramName()
        {
            for (int i = 0; i < Oper.Count; i++)
            {
                Oper[i].CreateOperationName(i + 1);
            }
        }
        /// <summary>
        /// 设置程序组
        /// </summary>
        /// <param name="program"></param>
        private void SetProgram(string program)
        {
            foreach (AbstractCreateOperation ao in Oper)
            {
                ao.SetProgramName(program);
            }
        }

        /// <summary>
        /// 设置程序组
        /// </summary>
        /// <param name="program"></param>
        public void SetProgram(int site)
        {
            this.program = "O" + string.Format("{0:D4}", site);
            foreach (AbstractCreateOperation ao in Oper)
            {
                ao.SetProgramName(program);
            }
        }
        /// <summary>
        /// 设置刀具
        /// </summary>
        /// <param name="toolName"></param>
        private void SetTool(string toolName)
        {
            foreach (AbstractCreateOperation ao in Oper)
            {
                ao.SetToolName(toolName);
            }
        }
        /// <summary>
        /// 创建刀路
        /// </summary>
        /// <param name="eleCam"></param>
        /// <returns></returns>
        public List<string> CreateOperation(AbstractElectrodeCAM eleCam)
        {
            List<string> err = new List<string>();
            UpdateProgramName();
            foreach (AbstractCreateOperation ao in Oper)
            {
                ao.SetOperationData(eleCam);
            }            
            foreach (AbstractCreateOperation ao in Oper)
            {
                try
                {
                    err.AddRange(ao.CreateOperation());
                }
                catch (Exception ex)
                {
                    err.Add(ex.Message);
                }
            }
            return err;
        }

        public List<string> CreateOperation()
        {
            List<string> err = new List<string>();           
            UpdateProgramName();
            foreach (AbstractCreateOperation ao in Oper)
            {
                try
                {
                    err.AddRange(ao.CreateOperation());
                }
                catch (Exception ex)
                {
                    err.Add(ex.Message);
                }
            }
            return err;
        }
    }
}
