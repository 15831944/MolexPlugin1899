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
    public class ElectrodeModel : AbstractAssmbileModel, IComparable<WorkModel>
    {

        public ElectrodeModel(Part part) : base(part)
        {

        }

        public ElectrodeModel(ElectrodeInfo info) : base(info)
        {
        }

        public int CompareTo(WorkModel other)
        {
            return (this.AssmblieInfo as ElectrodeInfo).AllInfo.Attribute.EleNumber.CompareTo((other.AssmblieInfo as ElectrodeInfo).AllInfo.Attribute.EleNumber);
        }

        public override string GetAssembleName()
        {
            return ((this.AssmblieInfo) as ElectrodeInfo).AllInfo.Attribute.EleName;
        }

        public override Component Load(Part parentPart)
        {

            try
            {
                ElectrodeInfo info = (this.AssmblieInfo) as ElectrodeInfo;
                Point3d setVaue = new Point3d(info.AllInfo.Attribute.EleSetValue[0], info.AllInfo.Attribute.EleSetValue[1], info.AllInfo.Attribute.EleSetValue[2]);
                Component ct = Basic.AssmbliesUtils.PartLoad(parentPart, this.WorkpiecePath, this.AssembleName, info.Matr, setVaue);
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
        public Component Load(Part parentPart, Matrix4 mat, Point3d setVaue)
        {
            ElectrodeInfo info = (this.AssmblieInfo) as ElectrodeInfo;
            info.MatrInfo = new Matrix4Info(mat);
            info.Matr = mat;
            info.AllInfo.Attribute.EleSetValue = new double[] { setVaue.X, setVaue.Y, setVaue.Z };
            info.SetAttribute(this.PartTag);
            try
            {
                Component ct = Basic.AssmbliesUtils.PartLoad(parentPart, this.WorkpiecePath, this.AssembleName, mat, setVaue);
                return ct;

            }
            catch (NXException ex)
            {
                throw ex;
            }
        }
        protected override void GetModelForAttribute(NXObject obj)
        {
            this.AssmblieInfo = ElectrodeInfo.GetAttribute(obj);
        }

    }
}
