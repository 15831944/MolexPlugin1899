using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 操作树列表
    /// </summary>
    public class OperationTreeListViewInfo
    {

        private AbstractElectrodeTemplate template;
        private List<ElectrodeCAMTreeInfo> treeInfo = new List<ElectrodeCAMTreeInfo>();

        public List<ElectrodeCAMTreeInfo> TreeInfo
        {
            get
            {
                GetTreeInfo();
                return treeInfo;
            }
        }
        public OperationTreeListViewInfo(AbstractElectrodeTemplate template)
        {
            this.template = template;
        }
        /// <summary>
        /// 获取树信息
        /// </summary>
        private void GetTreeInfo()
        {
            this.treeInfo.Clear();
            foreach (ProgramOperationName pn in template.Programs)
            {
                ElectrodeCAMTreeInfo info = new ElectrodeCAMTreeInfo(pn);
                treeInfo.Add(info);
            }
        }
        /// <summary>
        /// 删除操作
        /// </summary>
        /// <param name="oper"></param>
        /// <returns></returns>
        public bool Delete(ElectrodeCAMTreeInfo info)
        {
            if (info.Program is ProgramOperationName)
            {
                ProgramOperationName temp = info.Program as ProgramOperationName;
                if (this.template.Programs.Exists(a => a.Equals(temp)))
                {
                    this.template.DeleteProgram(temp);
                    GetTreeInfo();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (info.Program is AbstractCreateOperation)
            {
                AbstractCreateOperation temp = info.Program as AbstractCreateOperation;
                if (info.Parent != null && info.Parent.Program is ProgramOperationName)
                {
                    ProgramOperationName tp = info.Parent.Program as ProgramOperationName;
                    if (tp.Oper.Exists(a => a.Equals(temp)))
                    {
                        tp.DeleteOperationNameModel(temp);
                        GetTreeInfo();
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 向上移动
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool MoveUp(ElectrodeCAMTreeInfo info)
        {
            if (info.Program is ProgramOperationName)
            {
                ProgramOperationName temp = info.Program as ProgramOperationName;
                int count = this.template.Programs.FindIndex(a => a.Equals(temp));
                if (this.template.Programs.Exists(a => a.Equals(temp)))
                {
                    if (count != 0 && count != -1)
                    {
                        this.template.DeleteProgram(count);
                        this.template.AddProgram(temp, count - 1);
                        GetTreeInfo();
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            if (info.Program is AbstractCreateOperation)
            {
                AbstractCreateOperation temp = info.Program as AbstractCreateOperation;
                if (info.Parent != null && info.Parent.Program is ProgramOperationName)
                {
                    ProgramOperationName tp = info.Parent.Program as ProgramOperationName;
                    int count = tp.Oper.FindIndex(a => a.Equals(temp));
                    if (tp.Oper.Exists(a => a.Equals(temp)))
                    {
                        if (count != 0 && count != -1)
                        {
                            tp.DeleteOperationNameModel(temp);
                            tp.AddOperationNameModel(temp, count - 1);
                            GetTreeInfo();
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 向下移动
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool MoveDown(ElectrodeCAMTreeInfo info)
        {
            if (info.Program is ProgramOperationName)
            {
                ProgramOperationName temp = info.Program as ProgramOperationName;
                int count = this.template.Programs.FindIndex(a => a.Equals(temp));
                if (this.template.Programs.Exists(a => a.Equals(temp)))
                {
                    if (count < this.template.Programs.Count && count != -1)
                    {
                        this.template.DeleteProgram(count);
                        this.template.AddProgram(temp, count + 1);
                        GetTreeInfo();
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            if (info.Program is AbstractCreateOperation)
            {
                AbstractCreateOperation temp = info.Program as AbstractCreateOperation;
                if (info.Parent != null && info.Parent.Program is ProgramOperationName)
                {
                    ProgramOperationName tp = info.Parent.Program as ProgramOperationName;
                    int count = tp.Oper.FindIndex(a => a.Equals(temp));
                    if (tp.Oper.Exists(a => a.Equals(temp)))
                    {
                        if (count < tp.Oper.Count && count != -1)
                        {
                            tp.DeleteOperationNameModel(temp);
                            tp.AddOperationNameModel(temp, count + 1);
                            GetTreeInfo();
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            GetTreeInfo();
        }
        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public object Copy(ElectrodeCAMTreeInfo info)
        {
            if (info.Program is ProgramOperationName)
            {
                ProgramOperationName temp = info.Program as ProgramOperationName;
                return new ProgramOperationName(temp.Program);
            }
            if (info.Program is AbstractCreateOperation)
            {
                AbstractCreateOperation temp = info.Program as AbstractCreateOperation;
                return temp.CopyOperation(10);
            }
            return null;
        }
        /// <summary>
        /// 粘贴
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool Stick(ElectrodeCAMTreeInfo info, object obt)
        {
            if (info.Program is ProgramOperationName)
            {
                ProgramOperationName temp = info.Program as ProgramOperationName;
                if (obt is ProgramOperationName)
                {
                    int count = this.template.Programs.FindIndex(a => a.Equals(temp));
                    if (count != -1)
                    {
                        this.template.AddProgram(obt as ProgramOperationName, count + 1);
                        return true;
                    }
                }
                else if (obt is AbstractCreateOperation)
                {
                    AbstractCreateOperation tp = obt as AbstractCreateOperation;
                    tp.SetToolName(temp.ToolName);
                    return temp.AddOperationNameModel(tp);

                }
            }
            if (info.Program is AbstractCreateOperation && obt is AbstractCreateOperation)
            {
                AbstractCreateOperation temp = info.Program as AbstractCreateOperation;
                if (info.Parent != null && info.Parent.Program is ProgramOperationName)
                {
                    ProgramOperationName tp = info.Parent.Program as ProgramOperationName;
                    AbstractCreateOperation ao = obt as AbstractCreateOperation;
                    int count = tp.Oper.FindIndex(a => a.Equals(temp));
                    if (count != -1)
                    {
                        ao.SetToolName(tp.ToolName);
                        return tp.AddOperationNameModel(ao, count + 1);
                    }

                }
            }
            return false;
        }
        /// <summary>
        /// 添加刀路
        /// </summary>
        /// <param name="oper"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool AddOperation(AbstractCreateOperation oper, ElectrodeCAMTreeInfo info)
        {
            if (info.Program is ProgramOperationName)
            {
                ProgramOperationName temp = info.Program as ProgramOperationName;
                return temp.AddOperationNameModel(oper);
            }
            if (info.Program is AbstractCreateOperation)
            {
                AbstractCreateOperation temp = info.Program as AbstractCreateOperation;
                if (info.Parent != null && info.Parent.Program is ProgramOperationName)
                {
                    ProgramOperationName tp = info.Parent.Program as ProgramOperationName;
                    int count = tp.Oper.FindIndex(a => a.Equals(temp));
                    if (count != -1)
                    {
                        return tp.AddOperationNameModel(oper, count + 1);
                    }
                }
            }
            return false;
        }
    }
}
