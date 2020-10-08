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

        private List<string> CreateNewWork1(NXOpen.Assemblies.Component ct, WorkpieceModel workpiece, Matrix4 mat, UserModel user)
        {
            List<string> err = new List<string>();
            MoldInfo moldInfo = workpiece.Info.MoldInfo;
            int num = asmColl.GetWorkCollection(moldInfo).Work.Count;
            WorkInfo workInfo = new WorkInfo(workpiece.Info.MoldInfo, user, num + 1, mat);
            string workName = workInfo.MoldInfo.MoldNumber + "-" + workInfo.MoldInfo.WorkpieceNumber + "-" + "WORK" + workInfo.WorkNumber.ToString();
            EDMModel edm = GetEdm(ct);
            if (edm == null)
            {
                err.Add("获取EDM错误");
                return err;
            }
            NXOpen.Assemblies.Component comp = AssmbliesUtils.MoveCompCopyPart(ct.Parent.Parent, new Vector3d(0, 0, 0), mat);
            AssmbliesUtils.MakeUnique(comp, edm.WorkpieceDirectoryPath + workName + ".prt");
            workInfo.SetAttribute(comp.Prototype as Part);
            WorkModel wm = new WorkModel(comp.Prototype as Part);
            if (wm != null)
                wm.SaveCsys(workPart);
            if (!edm.Info.MoldInfo.Equals(workInfo.MoldInfo))
            {
                EDMInfo edmInfo = new EDMInfo(workInfo.MoldInfo, workInfo.UserModel);
                string edmName = edmInfo.MoldInfo.MoldNumber + "-" + edmInfo.MoldInfo.WorkpieceNumber + "-" + "EDM";
                foreach (NXOpen.Assemblies.Component cp in comp.GetChildren())
                {
                    if (ParentAssmblieInfo.IsEDM(cp))
                    {
                        AssmbliesUtils.MakeUnique(cp, edm.WorkpieceDirectoryPath + edmName + ".prt");
                        edmInfo.SetAttribute(cp.Prototype as Part);
                        foreach (NXOpen.Assemblies.Component co in cp.GetChildren())
                        {
                            if (!(co.Prototype as Part).Equals(workpiece.PartTag))
                                AssmbliesUtils.DeleteComponent(co);
                        }
                    }
                }

            }
            return err;
        }
        /// <summary>
        /// 修改Work
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
            bool setValue = work.AlterEleSetValue();
            if (isSave && isSet && setValue)
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
        private WorkModel GetWork(NXOpen.Assemblies.Component partCt)
        {
            try
            {
                Part workPart = (partCt.Parent.Parent.Prototype) as Part;
                return new WorkModel(workPart);
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("获取work错误" + ex.Message);
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
        /// <summary>
        /// 获取工件名字
        /// </summary>
        /// <param name="work"></param>
        /// <returns></returns>
        private List<string> GetWorkpieceName(WorkModel work)
        {
            List<Part> otherWorkpeces = work.GetAllWorkpiece();
            List<string> name = new List<string>();
            foreach (Part pt in otherWorkpeces)
            {
                name.Add(pt.Name);
            }
            name.Sort();
            return name;
        }
        /// <summary>
        /// 复制work
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="work"></param>
        /// <param name="workpieceNumber"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private List<string> CopyWork(NXOpen.Assemblies.Component ct, WorkModel work, string workpieceNumber, UserModel user)
        {
            List<string> err = new List<string>();
            Matrix4 mat = new Matrix4();
            mat.Identity();
            MoldInfo mold = work.Info.MoldInfo.Clone() as MoldInfo;
            NXOpen.Assemblies.Component moveCt = null;
            try
            {
                moveCt = AssmbliesUtils.MoveCompCopyPart(ct, new Vector3d(), mat);
            }
            catch (NXException ex)
            {
                err.Add("无法移动工件！" + ex.Message);
                return err;
            }
            mold.WorkpieceNumber = workpieceNumber;
            string name = work.WorkpieceDirectoryPath + mold.MoldNumber + "-" + mold.WorkpieceNumber + "-";
            if (moveCt != null)
            {
                foreach (NXOpen.Assemblies.Component com in moveCt.GetChildren())
                {
                    if (ParentAssmblieInfo.IsParent(com))
                    {
                        ParentAssmblieInfo info1 = ParentAssmblieInfo.GetAttribute(com);
                        if (info1.Type == PartType.EDM)
                        {
                            EDMInfo edm = new EDMInfo(mold, user);
                            try
                            {
                                NXObject make = AssmbliesUtils.MakeUnique(com, name + "EDM.prt");
                                edm.SetAttribute(com.Prototype as Part);
                            }
                            catch (NXException ex)
                            {
                                err.Add(name + "EDM.prt" + ex.Message + "无法创建唯一");
                            }
                        }

                    }

                }
                try
                {
                    NXObject make1 = AssmbliesUtils.MakeUnique(moveCt, name + "WORK" + work.Info.WorkNumber.ToString() + ".prt");
                    WorkInfo wk = new WorkInfo(mold, user, work.Info.WorkNumber, work.Info.Matr);
                    wk.SetAttribute(moveCt.Prototype as Part);
                }
                catch (NXException ex)
                {
                    err.Add(name + "WORK" + work.Info.WorkNumber.ToString() + ".prt" + ex.Message + "无法创建唯一");
                }
            }

            return err;

        }

        private List<string> CopWork(NXOpen.Assemblies.Component ct, WorkModel work, UserModel user)
        {
            List<string> err = new List<string>();
            Matrix4 mat = new Matrix4();
            mat.Identity();

            MoldInfo mold = work.Info.MoldInfo.Clone() as MoldInfo;
            WorkCollection workColl = new WorkCollection(mold);

            NXOpen.Assemblies.Component moveCt = null;
            try
            {
                moveCt = AssmbliesUtils.MoveCompCopyPart(ct, new Vector3d(), mat);
            }
            catch (NXException ex)
            {
                err.Add("无法移动工件！" + ex.Message);
                return err;
            }
            string name = work.WorkpieceDirectoryPath + mold.MoldNumber + "-" + mold.WorkpieceNumber + "-";
            if (moveCt != null)
            {
                WorkInfo wk = new WorkInfo(mold, user, work.Info.WorkNumber, work.Info.Matr);
                try
                {
                    NXObject make1 = AssmbliesUtils.MakeUnique(moveCt, name + "WORK" + (workColl.Work[workColl.Work.Count - 1].Info.WorkNumber + 1).ToString() + ".prt");
                    wk.WorkNumber = (workColl.Work[workColl.Work.Count - 1].Info.WorkNumber + 1);
                    wk.SetAttribute(moveCt.Prototype as Part);
                }
                catch (NXException ex)
                {
                    err.Add(name + "WORK" + (workColl.Work[workColl.Work.Count - 1].Info.WorkNumber + 1).ToString() + ".prt" + ex.Message + "无法创建唯一");
                }

            }

            return err;
        }
        /// <summary>
        /// 获取件号名
        /// </summary>
        /// <param name="name"></param>
        /// <param name="work"></param>
        /// <returns></returns>
        private string GetWorkpieceNumber(string name, WorkModel work)
        {
            foreach (Part pt in work.GetAllWorkpiece())
            {
                if (pt.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (WorkpieceModel.IsWorkpiece(pt))
                    {
                        WorkpieceModel wk = new WorkpieceModel(pt);
                        return wk.Info.MoldInfo.WorkpieceNumber;
                    }
                    else
                        return pt.Name;
                }
            }
            return null;
        }
        /// <summary>
        /// 获取工件实体在装配下的体
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        private List<Body> GetCompBodys(NXOpen.Assemblies.Component ct, Part pt)
        {
            List<Body> bodys = new List<Body>();
            if ((ct.Prototype as Part).Equals(pt))
            {
                foreach (Body by in pt.Bodies.ToArray())
                {
                    Body bo = AssmbliesUtils.GetNXObjectOfOcc(ct.Tag, by.Tag) as Body;
                    if (bo != null)
                        bodys.Add(bo);
                }
            }
            else
            {
                foreach (NXOpen.Assemblies.Component co in AssmbliesUtils.GetPartComp(workPart, pt))
                {
                    if (!co.IsSuppressed)
                    {
                        foreach (Body by in pt.Bodies.ToArray())
                        {
                            Body bo = AssmbliesUtils.GetNXObjectOfOcc(co.Tag, by.Tag) as Body;
                            if (bo != null)
                                bodys.Add(bo);
                        }
                    }
                }
            }

            return bodys;
        }
        /// <summary>
        /// 获得选择工件work副本的矩阵
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        private Matrix4 GetParentWorkMatr(NXOpen.Assemblies.Component ct)
        {
            NXOpen.Assemblies.Component workComp = ct.Parent.Parent;
            WorkModel work = new WorkModel(workComp.Prototype as Part);
            return work.Info.Matr;
        }

    }
}
