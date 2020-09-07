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
        public EDMInfo Info { get; protected set; }
        public EDMModel(Part part) : base(part)
        {

        }

        public EDMModel(EDMInfo info)
        {
            this.Info = info;
        }

        public override string GetAssembleName()
        {

            return this.Info.MoldInfo.MoldNumber + "-" + this.Info.MoldInfo.WorkpieceNumber + "-" + "EDM";
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
            this.Info = EDMInfo.GetAttribute(obj);
        }
        /// <summary>
        /// 判断当前部件是否是EDM
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public static bool IsEDM(Part part)
        {
            ParentAssmblieInfo info = ParentAssmblieInfo.GetAttribute(part);
            return info.Type == PartType.EDM;
        }
        public override bool SetAttribute(NXObject obj)
        {
            return this.Info.SetAttribute(obj);
        }
    }
}
