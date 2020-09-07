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
    /// Electrode装配档
    /// </summary>
    public class ElectrodeModel : AbstractAssmbileModel, IComparable<ElectrodeModel>
    {
        public ElectrodeInfo Info { get; protected set; }
        public ElectrodeModel(Part part) : base(part)
        {

        }

        public ElectrodeModel(ElectrodeInfo info)
        {
            this.Info = info;
        }

        public int CompareTo(ElectrodeModel other)
        {
            return this.Info.AllInfo.Name.EleNumber.CompareTo(other.Info.AllInfo.Name.EleNumber);
        }

        public override string GetAssembleName()
        {
            return this.Info.AllInfo.Name.EleName;
        }

        public override Component Load(Part parentPart)
        {

            try
            {
                Component ct = Basic.AssmbliesUtils.PartLoad(parentPart, this.WorkpiecePath, this.AssembleName, this.Info.Matr, this.Info.Matr.GetCenter());
                return ct;

            }
            catch (NXException ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 以矩阵和设定坐标装配
        /// </summary>
        /// <param name="parentPart"></param>
        /// <param name="mat"></param>
        /// <param name="setVaue"></param>
        /// <returns></returns>
        public Component Load(Part parentPart, Matrix4 mat)
        {
            this.Info.MatrInfo = new Matrix4Info(mat);
            this.Info.Matr = mat;
            this.Info.SetAttribute(this.PartTag);
            try
            {
                Component ct = Basic.AssmbliesUtils.PartLoad(parentPart, this.WorkpiecePath, this.AssembleName, mat, mat.GetCenter());
                return ct;

            }
            catch (NXException ex)
            {
                throw ex;
            }
        }
        protected override void GetModelForAttribute(NXObject obj)
        {
            this.Info = ElectrodeInfo.GetAttribute(obj);
        }
        /// <summary>
        /// 判断当前部件是否是Electrode
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public static bool IsElectrode(Part part)
        {
            ParentAssmblieInfo info = ParentAssmblieInfo.GetAttribute(part);
            return info.Type == PartType.Electrode;
        }

        /// <summary>
        /// 创建装配
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public NXOpen.Assemblies.Component CreateCompPart(string directoryPath)
        {
            this.WorkpieceDirectoryPath = directoryPath;
            this.WorkpiecePath = directoryPath + this.AssembleName + ".prt";

            CsysUtils.SetWcsOfCenteAndMatr(this.Info.Matr.GetCenter(), this.Info.Matr.GetMatrix3());
            try
            {
                NXObject obj = AssmbliesUtils.CreateNew(this.AssembleName, WorkpiecePath);
                NXOpen.Assemblies.Component comp = obj as NXOpen.Assemblies.Component;
                this.PartTag = obj.Prototype as Part;
                if (this.PartTag != null)
                {
                    SetAttribute(this.PartTag);
                }
                CsysUtils.SetWcsToAbs();
                return comp;
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("创建装配档错误！" + ex.Message);
                throw ex;
            }

        }
        public override bool SetAttribute(NXObject obj)
        {
            return this.Info.SetAttribute(obj);
        }

    }
}
