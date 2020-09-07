using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;
using MolexPlugin.Model;
using NXOpen.Assemblies;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 创建电极组立档
    /// </summary>
    public class WorkCreateAssmbile : AbstractCreateAssmbile
    {
        public WorkModel Work { get; private set; }
        public WorkCreateAssmbile(WorkInfo workInfo, EDMModel edm, ASMModel asm, WorkpieceModel workpiece = null) : base(workInfo.MoldInfo, workInfo.UserModel)
        {
            WorkModel work = new WorkModel(workInfo);
            work.SetParentModel(asm);
            edm.SetParentModel(work);
            if (workpiece != null)
            {
                workpiece.SetParentModel(edm);
            }
            models.Add(work);
            models.Add(edm);
            if (workpiece != null)
            {              
                models.Add(workpiece);
            }
            this.Work = work;
        }
        /// <summary>
        /// 加载到ASM
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        public bool LoadAsm(Part asm)
        {
            Matrix4 matr = new Matrix4();
            matr.Identity();
            try
            {
                Component ct = Basic.AssmbliesUtils.PartLoad(asm, Work.WorkpiecePath, Work.AssembleName, matr, new Point3d(0, 0, 0));
                return true;

            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("装配到ASM错误!" + ex.Message);
                return false;
            }
        }


    }
}
