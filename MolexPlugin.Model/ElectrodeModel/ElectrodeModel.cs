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
            CsysUtils.SetWcsToAbs();
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

        /// <summary>
        /// 获取电极中设定点
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public Point GetSetPoint()
        {
            if (this.PartTag == null)
                return null;
            foreach (Point k in this.PartTag.Points.ToArray())
            {
                if (k.Name.ToUpper().Equals(("SetValuePoint").ToUpper()))
                    return k;
            }
            return null;
        }

        /// <summary>
        /// 获取电极X中心线
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public Line GetXLine()
        {
            if (this.PartTag == null)
                return null;
            foreach (Line k in this.PartTag.Lines.ToArray())
            {
                if (k.Name.ToUpper().Equals(("XCenterLine").ToUpper()))
                    return k;
            }
            return null;
        }

        /// <summary>
        /// 获取电极Y中心线
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public Line GetYLine()
        {
            if (this.PartTag == null)
                return null;
            foreach (Line k in this.PartTag.Lines.ToArray())
            {
                if (k.Name.ToUpper().Equals(("YCenterLine").ToUpper()))
                    return k;
            }
            return null;
        }

        /// <summary>
        /// 获取work
        /// </summary>
        /// <param name="model"></param>
        /// <param name="asmPart"></param>
        /// <returns></returns>
        public WorkModel GetWorkModel(Part asm)
        {
            List<NXOpen.Assemblies.Component> eleComs = AssmbliesUtils.GetPartComp(asm, this.PartTag);
            List<WorkModel> works = new List<WorkModel>();
            foreach (NXOpen.Assemblies.Component ct in eleComs)
            {
                NXOpen.Assemblies.Component parent = ct.Parent;
                if (parent != null)
                {
                    Part pt = parent.Prototype as Part;
                    if (WorkModel.IsWork(pt) && !works.Exists(a => a.PartTag.Equals(pt)) && ParentAssmblieInfo.GetAttribute(pt).MoldInfo.Equals(this.Info.MoldInfo))
                        works.Add(new WorkModel(pt));
                }
            }
            works.Sort();
            if (works.Count != 0)
            {
                return works[0];
            }
            else
                return null;
        }
    }
}
