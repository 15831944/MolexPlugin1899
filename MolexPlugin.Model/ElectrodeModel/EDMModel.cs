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
    /// EDM装配档
    /// </summary>
    public class EDMModel : AbstractAssmbileModel
    {

        public EDMModel(Part part) : base(part)
        {

        }

        public EDMModel(EDMInfo info) : base(info)
        {
        }

        public override string GetAssembleName()
        {
            string num = "";
            if ((this.AssmblieInfo as EDMInfo).EdmNumber != 0)
            {
                num = (this.AssmblieInfo as EDMInfo).EdmNumber.ToString();
            }
            return this.AssmblieInfo.MoldInfo.MoldNumber + "-" + this.AssmblieInfo.MoldInfo.WorkpieceNumber + "-" + "EDM" + num;
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
            this.AssmblieInfo = EDMInfo.GetAttribute(obj);
        }
    }
}
