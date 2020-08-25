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
    /// Work装配档
    /// </summary>
    public class WorkModel : AbstractAssmbileModel, IComparable<WorkModel>
    {

        public WorkModel(Part part) : base(part)
        {

        }

        public WorkModel(WorkInfo info) : base(info)
        {
        }

        public int CompareTo(WorkModel other)
        {
            return (this.AssmblieInfo as WorkInfo).WorkNumber.CompareTo((other.AssmblieInfo as WorkInfo).WorkNumber);
        }

        public override string GetAssembleName()
        {
            return this.AssmblieInfo.MoldInfo.MoldNumber + "-" + this.AssmblieInfo.MoldInfo.WorkpieceNumber + "-" + "WORK" + (this.AssmblieInfo as WorkInfo).WorkNumber.ToString();
        }

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

        protected override void GetModelForAttribute(NXObject obj)
        {
            this.AssmblieInfo = WorkPieceInfo.GetAttribute(obj);
        }
        // 修改矩阵
        /// </summary>
        /// <param name="matr"></param>
        public bool AlterMatr(Matrix4 matr)
        {
            WorkInfo info = (this.AssmblieInfo as WorkInfo);
            info.Matr = matr;
            info.MatrInfo = new Matrix4Info(matr);
            return info.SetAttribute(this.PartTag);
        }
    }
}
