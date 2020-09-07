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
    public class EdmCreateAssmbile : AbstractCreateAssmbile
    {
        private AbstractAssmbileModel edmModel;
        public EdmCreateAssmbile(EDMInfo edmInfo, WorkModel work, WorkpieceModel workpiece) : base(edmInfo.MoldInfo, edmInfo.UserModel)
        {
            AbstractAssmbileModel edm = new EDMModel(edmInfo);
            edm.SetParentModel(work);
            workpiece.SetParentModel(edm);
            models.Add(work);
            models.Add(edm);
            models.Add(workpiece);
            this.edmModel = edm;
        }
        /// <summary>
        /// 加载到ASM
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        public bool LoadAsm(Part workPart)
        {
            Matrix4 matr = new Matrix4();
            matr.Identity();
            try
            {
                Component ct = Basic.AssmbliesUtils.PartLoad(workPart, edmModel.WorkpiecePath, edmModel.AssembleName, matr, new Point3d(0, 0, 0));
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
