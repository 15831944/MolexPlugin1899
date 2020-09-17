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
    public partial class AddWork
    {
        /// <summary>
        /// 创建Work
        /// </summary>
        /// <param name="workpiece"></param>
        /// <param name="mat"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private List<string> CreateNewWork(NXOpen.Assemblies.Component ct, WorkpieceModel workpiece, Matrix4 mat, UserModel user)
        {
            List<string> err = new List<string>();
            MoldInfo moldInfo = workpiece.Info.MoldInfo;
            int num = asmColl.GetWorkCollection(moldInfo).Work.Count;
            WorkInfo workInfo = new WorkInfo(workpiece.Info.MoldInfo, user, num + 1, mat);
            EDMModel edm = GetEdm(ct);
            if (edm == null)
            {
                err.Add("获取EDM错误");
                return err;
            }
            WorkCreateAssmbile create;
            if (edm.Info.MoldInfo.Equals(workInfo.MoldInfo))
            {
                create = new WorkCreateAssmbile(workInfo, edm, asmModel);
            }
            else
            {
                EDMModel temp = new EDMModel(new EDMInfo(workInfo.MoldInfo, workInfo.UserModel));
                create = new WorkCreateAssmbile(workInfo, temp, asmModel, workpiece);
            }
            err.AddRange(create.CreatePart(workpiece.WorkpieceDirectoryPath));
            err.AddRange(create.LoadAssmbile());
            if (err.Count == 0)
            {
                create.Work.SaveCsys(workPart);
            }
            return err;
        }

        /// <summary>
        /// 创建Work
        /// </summary>
        /// <param name="workpiece"></param>
        /// <param name="mat"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private bool AlterWork(WorkModel work, Matrix4 mat)
        {
            Matrix4Info info = new Matrix4Info(mat);
            bool isSet = info.SetAttribute(work.PartTag);
            bool isSave = work.SaveCsys(workPart);
            if (isSave && isSet)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 获取EDM
        /// </summary>
        /// <param name="partCt"></param>
        /// <returns></returns>
        private EDMModel GetEdm(NXOpen.Assemblies.Component partCt)
        {
            try
            {
                Part edmPart = (partCt.Parent.Prototype) as Part;
                return new EDMModel(edmPart);
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("获取EDM错误" + ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 获取主工件
        /// </summary>
        /// <param name="work"></param>
        /// <returns></returns>
        private Part GetWorkPieceForWork(WorkModel work)
        {
            return asmColl.GetWorkCollection(work.Info.MoldInfo).GetHostWorkpiece();
        }



    }
}
