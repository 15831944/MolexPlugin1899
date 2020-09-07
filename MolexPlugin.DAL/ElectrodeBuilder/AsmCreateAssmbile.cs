using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 创建电极组立档
    /// </summary>
    public class AsmCreateAssmbile : AbstractCreateAssmbile
    {

        private AbstractAssmbileModel asmModel;
        public AsmCreateAssmbile(MoldInfo mold, UserModel user, Part workpiecePart) : base(mold, user)
        {
            Matrix4 mat = new Matrix4();
            mat.Identity();
            AbstractAssmbileModel asm = new ASMModel(new ASMInfo(base.moldInfo, base.userModel));
            AbstractAssmbileModel work = new WorkModel(new WorkInfo(base.moldInfo, base.userModel, 1, mat));
            AbstractAssmbileModel edm = new EDMModel(new EDMInfo(base.moldInfo, base.userModel));
            AbstractAssmbileModel workpiece = new WorkpieceModel(new WorkPieceInfo(base.moldInfo, base.userModel), workpiecePart);
            work.SetParentModel(asm);
            edm.SetParentModel(work);
            workpiece.SetParentModel(edm);
            this.asmModel = asm;
            models.Add(asm);
            models.Add(work);
            models.Add(edm);
            models.Add(workpiece);
        }

        public override List<string> CreatePart(string directoryPath)
        {
            List<string> err = new List<string>();
            foreach (AbstractAssmbileModel am in models)
            {
                if (!am.CreatePart(directoryPath))
                    err.Add(am.AssembleName + "创建失败");
            }
            if (asmModel.PartTag != null)
                PartUtils.SetPartDisplay(asmModel.PartTag);
            return err;
        }


    }
}
