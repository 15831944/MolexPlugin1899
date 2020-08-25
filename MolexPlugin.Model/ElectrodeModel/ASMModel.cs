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
    /// ASM装配档
    /// </summary>
    public class ASMModel : AbstractAssmbileModel
    {

        public ASMModel(Part part) : base(part)
        {

        }

        public ASMModel(ASMInfo info) : base(info)
        {
        }

        public override string GetAssembleName()
        {
            return this.AssmblieInfo.MoldInfo.MoldNumber + "-" + this.AssmblieInfo.MoldInfo.WorkpieceNumber + "-" + "ASM";
        }

        public override Component Load(Part parentPart)
        {
            return null;
        }

        protected override void GetModelForAttribute(NXObject obj)
        {
            this.AssmblieInfo = ASMInfo.GetAttribute(obj);
        }
    }
}
