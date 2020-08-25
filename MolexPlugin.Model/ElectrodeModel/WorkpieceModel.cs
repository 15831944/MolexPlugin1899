using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.BlockStyler;
using System.IO;
using NXOpen.Assemblies;
using Basic;

namespace MolexPlugin.Model
{
    /// <summary>
    /// Workpiece装配档
    /// </summary>
    public class WorkpieceModel
    {
        public WorkPieceInfo Info { get; private set; } = null;

        public Part PartTag { get; private set; }
        /// <summary>
        /// 工件地址
        /// </summary>
        public string WorkpiecePath { get; private set; }
        /// <summary>
        /// 文件夹位置
        /// </summary>
        public string WorkpieceDirectoryPath { get; private set; }
        /// <summary>
        /// 装配名字
        /// </summary>
        public string AssembleName { get { return GetAssembleName(); } }
        public WorkpieceModel(WorkPieceInfo info, Part part = null)
        {
            this.Info = info;
            this.PartTag = part;
        }
        public WorkpieceModel(Part part)
        {
            this.PartTag = part;
            GetPath(part);
            GetModelForAttribute(part);
        }
        /// <summary>
        /// 创建part档
        /// </summary>
        /// <param name="directoryPath">文件夹地址加\\</param>
        /// <returns></returns>
        public bool CreatePart(string directoryPath)
        {
            this.WorkpieceDirectoryPath = directoryPath;
            this.WorkpiecePath = directoryPath + AssembleName + ".prt";
            if (Info != null)
            {
                try
                {
                    if (File.Exists(this.WorkpiecePath))
                    {
                        File.Delete(this.WorkpiecePath);
                    }
                    if (PartTag != null)
                        PartTag.Close(BasePart.CloseWholeTree.False, BasePart.CloseModified.UseResponses, null);
                    File.Move(this.PartTag.FullPath, this.WorkpiecePath);
                    this.PartTag = PartUtils.OpenPartFile(this.WorkpiecePath);
                    return SetAttribute(this.PartTag);
                }
                catch (Exception ex)
                {
                    ClassItem.WriteLogFile("移动工件错误" + ex.Message);
                }
            }
            return false;

        }

        /// <summary>
        /// 获取装配档名称
        /// </summary>
        /// <returns></returns>
        public string GetAssembleName()
        {

            return this.Info.MoldInfo.MoldNumber + "-" + this.Info.MoldInfo.WorkpieceNumber + "-" + this.Info.MoldInfo.EditionNumber;
        }
        /// <summary>
        /// 装配
        /// </summary>
        /// <param name="parentPart"></param>
        /// <returns></returns>
        public Component Load(Part parentPart)
        {
            Matrix4 matr = new Matrix4();
            matr.Identity();
            try
            {
                Component ct = Basic.AssmbliesUtils.PartLoad(parentPart, this.WorkpiecePath, this.AssembleName, matr, new Point3d(0, 0, 0));
                return ct;

            }
            catch (NXException ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="part"></param>
        protected void GetModelForAttribute(Part part)
        {
            this.Info = (WorkPieceInfo.GetAttribute(part)) as WorkPieceInfo;
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
        /// <summary>
        /// 是否禁用
        /// </summary>
        /// <param name="parentPart">需要禁用的父本文件</param>
        /// <returns></returns>
        public bool IsSuppressed(Part parentPart)
        {
            Component ct = null;
            try
            {
                ct = AssmbliesUtils.GetPartComp(parentPart, this.PartTag);
                return ct.IsSuppressed;
            }
            catch (NXException ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <returns></returns>
        public bool SetAttribute(Part part)
        {
            return this.Info.SetAttribute(part);
        }
    }
}
