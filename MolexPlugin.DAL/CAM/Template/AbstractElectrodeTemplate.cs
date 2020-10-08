using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 电极刀路模板抽象类
    /// </summary>
    public abstract class AbstractElectrodeTemplate
    {
        /// <summary>
        /// 程序组
        /// </summary>
        public List<ProgramOperationName> Programs { get; protected set; } = new List<ProgramOperationName>();

        protected CompterToolName tool;

        public ElectrodeTemplate Type { get; protected set; }
        public AbstractElectrodeTemplate(CompterToolName tool)
        {
            this.tool = tool;
        }
        /// <summary>
        /// 添加刀路名
        /// </summary>
        /// <param name="model"></param>
        public void AddProgram(ProgramOperationName program, int count)
        {
            Programs.IndexOf(program, count);
            UpdateOperationNameModel();
        }
        /// <summary>
        /// 删除刀路
        /// </summary>
        /// <param name="model"></param>
        public void DeleteProgram(ProgramOperationName program)
        {
            Programs.Remove(program);
            UpdateOperationNameModel();
        }

        public void DeleteProgram(int count)
        {
            Programs.RemoveAt(count);
            UpdateOperationNameModel();
        }

        public void UpdateOperationNameModel()
        {
            for (int i = 0; i < Programs.Count; i++)
            {
                Programs[i].SetProgram(i + 1);
            }
        }
        /// <summary>
        /// 创建程序组
        /// </summary>
        public abstract void CreateProgramName();
    }

    public enum ElectrodeTemplate
    {
        /// <summary>
        /// 简单电极
        /// </summary>
        SimplenessVerticalEleTemplate = 1,
        /// <summary>
        /// 直加等宽
        /// </summary>
        PlanarAndSufaceEleTemplate,
        /// <summary>
        /// 直加等高
        /// </summary>
        PlanarAndZleveEleTemplate,
        /// <summary>
        /// 直加等高等宽
        /// </summary>
        PlanarAndZleveAndSufaceEleTemplate,
        /// <summary>
        /// 等高加等宽
        /// </summary>
        ZleveAndSufaceEleTemplate,
        /// <summary>
        /// 等高
        /// </summary>
        ZleveEleTemplate,
        /// <summary>
        /// 用户定义电极
        /// </summary>
        User,
    }
}
