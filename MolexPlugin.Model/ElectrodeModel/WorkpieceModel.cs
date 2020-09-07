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
    public class WorkpieceModel : AbstractAssmbileModel
    {
        public WorkPieceInfo Info { get; private set; } = null;

        public WorkpieceModel(WorkPieceInfo info, Part part = null) 
        {
            this.Info = info;
            this.PartTag = part;
        }
        public WorkpieceModel(Part part) : base(part)
        {
           
        }
        /// <summary>
        /// 创建part档
        /// </summary>
        /// <param name="directoryPath">文件夹地址加\\</param>
        /// <returns></returns>
        public override bool CreatePart(string directoryPath)
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
                    {
                        string oldPath = this.PartTag.FullPath;
                        PartTag.Close(BasePart.CloseWholeTree.False, BasePart.CloseModified.UseResponses, null);
                        File.Move(oldPath, this.WorkpiecePath);
                        this.PartTag = PartUtils.OpenPartFile(this.WorkpiecePath);
                        return SetAttribute(this.PartTag);
                    }
                }
                catch (Exception ex)
                {
                    ClassItem.WriteLogFile("移动工件错误" + ex.Message);
                }
            }
            return false;

        }
        public bool CreatePart(string directoryPath, string partPath)
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
                    foreach (Part pt in Session.GetSession().Parts)
                    {
                        if (pt.FullPath.Equals(partPath, StringComparison.CurrentCultureIgnoreCase))
                        {
                            pt.Close(BasePart.CloseWholeTree.False, BasePart.CloseModified.UseResponses, null);
                            break;
                        }
                    }
                    File.Move(partPath, this.WorkpiecePath);
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
        public override string GetAssembleName()
        {
            return this.Info.MoldInfo.MoldNumber + "-" + this.Info.MoldInfo.WorkpieceNumber + "-" + this.Info.MoldInfo.EditionNumber;
        }
        public override string GetAssembleName(NXObject obj)
        {
            return this.Info.MoldInfo.MoldNumber + "-" + this.Info.MoldInfo.WorkpieceNumber + "-" + this.Info.MoldInfo.EditionNumber;
        }
        /// <summary>
        /// 装配
        /// </summary>
        /// <param name="parentPart"></param>
        /// <returns></returns>
        public override Component Load(Part parentPart)
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
        /// 判断当前部件是否是Workpiece
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public static bool IsWorkpiece(Part part)
        {
            ParentAssmblieInfo info = ParentAssmblieInfo.GetAttribute(part);
            return info.Type == PartType.Workpiece;
        }


        protected override void GetModelForAttribute(NXObject obj)
        {
            this.Info = WorkPieceInfo.GetAttribute(obj);
        }
        public override bool SetAttribute(NXObject obj)
        {
            return this.Info.SetAttribute(obj);
        }
    }
}
