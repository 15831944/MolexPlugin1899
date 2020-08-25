using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.BlockStyler;
using System.IO;
using Basic;

namespace MolexPlugin.Model
{
    /// <summary>
    /// 装配档抽像父类
    /// </summary>
    public abstract class AbstractAssmbileModel : IEquatable<AbstractAssmbileModel>, IDisplayObject
    {
        private string name = "";
        /// <summary>
        /// 装配信息
        /// </summary>
        public ParentAssmblieInfo AssmblieInfo { get; protected set; }
        /// <summary>
        /// 工件
        /// </summary>
        public Part PartTag { get; protected set; } = null;
        /// <summary>
        /// 工件名
        /// </summary>
        public string AssembleName
        {
            get
            {
                if (name.Equals("") && PartTag == null)
                {
                    this.name = GetAssembleName();
                }
                else if (PartTag != null)
                {
                    this.name = GetAssembleName(PartTag);
                }
                return name;
            }
        }
        /// <summary>
        /// 工件地址
        /// </summary>
        public string WorkpiecePath { get; protected set; }
        /// <summary>
        /// 文件夹位置
        /// </summary>
        public string WorkpieceDirectoryPath { get; protected set; }
        /// <summary>
        /// 树列表
        /// </summary>
        public Node Node { get; set; }

        public AbstractAssmbileModel(Part part)
        {
            this.PartTag = part;
            GetPath(part);
            GetModelForAttribute(part);
        }

        public AbstractAssmbileModel(ParentAssmblieInfo info)
        {
            this.AssmblieInfo = info;
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <returns></returns>
        public bool SetAttribute(NXObject obj)
        {
            return this.AssmblieInfo.SetAttribute(obj);
        }
        /// <summary>
        /// 以属性获取实体
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        protected abstract void GetModelForAttribute(NXObject obj);

        /// <summary>
        /// 获取名字
        /// </summary>
        public abstract string GetAssembleName();

        public string GetAssembleName(NXObject obj)
        {
            return obj.Name;
        }
        /// <summary>
        /// 创建part档
        /// </summary>
        /// <param name="directoryPath">文件夹地址加\\</param>
        /// <returns></returns>
        public bool CreatePart(string directoryPath)
        {
            this.WorkpieceDirectoryPath = directoryPath;
            this.WorkpiecePath = directoryPath + this.AssembleName + ".prt";
            if (File.Exists(this.WorkpiecePath))
            {
                File.Delete(this.WorkpiecePath);
            }
            try
            {
                Part part = PartUtils.NewFile(this.WorkpiecePath) as Part;
                this.PartTag = part;
                SetAttribute(part);
                return true;
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("创建" + this.AssembleName + "失败" + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 装配
        /// </summary>
        /// <param name="parentPart"></param>
        /// <returns></returns>
        public abstract NXOpen.Assemblies.Component Load(Part parentPart);
        /// <summary>
        /// 获取装配档下Occs值
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public NXOpen.Assemblies.Component GetPartComp(Part parent)
        {
            Tag[] elePartOccsTag;
            NXOpen.UF.UFSession theUFSession = NXOpen.UF.UFSession.GetUFSession();
            try
            {
                theUFSession.Assem.AskOccsOfPart(parent.Tag, this.PartTag.Tag, out elePartOccsTag);
                return NXOpen.Utilities.NXObjectManager.Get(elePartOccsTag[0]) as NXOpen.Assemblies.Component;
            }
            catch
            {
                return null;
            }

        }
        public void Highlight(bool highlight)
        {
            Part workPart = Session.GetSession().Parts.Work;
            NXOpen.Assemblies.Component root = workPart.ComponentAssembly.RootComponent;
            if ((workPart.Tag != this.PartTag.Tag) && highlight && root != null)
            {
                foreach (NXOpen.Assemblies.Component ct in root.GetChildren())
                {
                    ct.Blank();
                }
                NXOpen.Assemblies.Component eleComp = Basic.AssmbliesUtils.GetPartComp(workPart, this.PartTag);
                eleComp.Unblank();
            }
        }

        public bool Equals(AbstractAssmbileModel other)
        {
            return this.AssembleName.Equals(other.AssembleName);
        }

        /// <summary>
        /// 获取地址
        /// </summary>
        /// <param name="part"></param>
        protected void GetPath(Part part)
        {
            this.WorkpiecePath = part.FullPath;
            this.WorkpieceDirectoryPath = Path.GetDirectoryName(WorkpiecePath) + "\\";
        }
    }
}
