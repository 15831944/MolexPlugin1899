using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;
using NXOpen.BlockStyler;
using NXOpen.Utilities;
using Basic;
using MolexPlugin.DAL;
using MolexPlugin.Model;
using System.IO;

namespace MolexPlugin
{
    public partial class CopyAsm
    {
        /// <summary>
        /// 替换WORK
        /// </summary>
        /// <param name="wks"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        private List<string> AlterWork(List<WorkModel> wks, ParentAssmblieInfo info)
        {
            List<string> err = new List<string>();
            string temp = info.MoldInfo.MoldNumber + "-" + info.MoldInfo.WorkpieceNumber;
            foreach (WorkModel wm in wks)
            {
                string workName = temp + "-WORK" + wm.Info.WorkNumber;
                ReplaceOther rep = new ReplaceOther(wm.PartTag, info);
                err.AddRange(rep.Alter(workName));
            }
            return err;
        }
        /// <summary>
        /// 修改EDM
        /// </summary>
        /// <param name="edms"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        private List<string> AlterEdm(List<EDMModel> edms, ParentAssmblieInfo info)
        {
            List<string> err = new List<string>();
            string temp = info.MoldInfo.MoldNumber + "-" + info.MoldInfo.WorkpieceNumber;
            foreach (EDMModel em in edms)
            {
                string old = em.Info.MoldInfo.MoldNumber + "-" + em.Info.MoldInfo.WorkpieceNumber;
                string edmName = em.AssembleName.Replace(old, temp);
                ReplaceOther rep = new ReplaceOther(em.PartTag, info);
                err.AddRange(rep.Alter(edmName));
            }
            return err;
        }

        private List<string> AlterElectrode(List<ElectrodeModel> eles, ParentAssmblieInfo info, bool isBorrow)
        {
            List<string> err = new List<string>();
            string temp = info.MoldInfo.MoldNumber + "-" + info.MoldInfo.WorkpieceNumber;
            foreach (ElectrodeModel em in eles)
            {
                string old = em.Info.MoldInfo.MoldNumber + "-" + em.Info.MoldInfo.WorkpieceNumber;
                ElectrodeNameInfo newNameInfo = em.Info.AllInfo.Name.Clone() as ElectrodeNameInfo;
                string oldName = em.Info.AllInfo.Name.EleName;
                string newName = oldName.Replace(old, temp);
                newNameInfo.EleName = newName;
                if (isBorrow)
                    newNameInfo.BorrowName = oldName;
                ReplaceElectrode rep = new ReplaceElectrode(em.PartTag, newNameInfo);
                err.AddRange(rep.AlterEle(info));
                err.AddRange(rep.AlterEleDra(em.Info, info));
            }
            return err;
        }

        private List<string> AlterAsm(ASMModel asm, ParentAssmblieInfo info)
        {
            List<string> err = new List<string>();
            bool anyPartsModified1;
            NXOpen.PartSaveStatus partSaveStatus1;
            theSession.Parts.SaveAll(out anyPartsModified1, out partSaveStatus1);
            string temp = info.MoldInfo.MoldNumber + "-" + info.MoldInfo.WorkpieceNumber;
            string name = temp + "-ASM";
            ReplaceOther rep = new ReplaceOther(asm.PartTag, info);
            err.AddRange(rep.Alter(name));
            return err;
        }
    }
}
