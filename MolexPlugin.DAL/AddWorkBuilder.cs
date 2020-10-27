using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;
using MolexPlugin.Model;
using System.IO;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 添加wrok操作(再ASM装配下)
    /// </summary>
    public class AddWorkBuilder
    {

        private NXOpen.Assemblies.Component workCt;
        private WorkModel work;
        private ASMModel asm;
        public AddWorkBuilder(ASMModel asm, NXOpen.Assemblies.Component workCt)
        {
            this.workCt = workCt;
            this.asm = asm;
            work = new WorkModel(workCt.Prototype as Part);
        }
        /// <summary>
        /// 获取工件Part
        /// </summary>
        /// <returns></returns>
        private List<NXOpen.Assemblies.Component> GetWorkpieces()
        {
            List<NXOpen.Assemblies.Component> workpiece = new List<NXOpen.Assemblies.Component>();
            foreach (NXOpen.Assemblies.Component ct in this.workCt.GetChildren())
            {
                if (ParentAssmblieInfo.IsEDM(ct))
                {
                    foreach (NXOpen.Assemblies.Component cp in ct.GetChildren())
                    {
                        if (!workpiece.Exists(a => a.Equals(cp)))
                        {
                            workpiece.Add(cp);
                        }
                    }
                }
            }
            return workpiece;
        }
        /// <summary>
        /// 获取电极Component
        /// </summary>
        /// <returns></returns>
        public List<NXOpen.Assemblies.Component> GetElectrodeComponent()
        {
            List<NXOpen.Assemblies.Component> eleCt = new List<NXOpen.Assemblies.Component>();
            foreach (NXOpen.Assemblies.Component ct in this.workCt.GetChildren())
            {
                if (ParentAssmblieInfo.IsElectrode(ct))
                {
                    eleCt.Add(ct);
                }
            }
            return eleCt;
        }
        /// <summary>
        /// 获取EDM
        /// </summary>
        /// <returns></returns>
        public EDMModel GetEDMModel()
        {
            EDMModel model = null;
            foreach (NXOpen.Assemblies.Component ct in this.workCt.GetChildren())
            {
                if (ParentAssmblieInfo.IsEDM(ct))
                {
                    model = new EDMModel(ct.Prototype as Part);
                }
            }
            return model;
        }
        /// <summary>
        /// 获取工件名
        /// </summary>
        /// <returns></returns>
        public List<string> GetWorkpieceNames()
        {
            List<string> name = new List<string>();
            foreach (NXOpen.Assemblies.Component pt in this.GetWorkpieces())
            {
                if (name.Exists(a => a.Equals(pt.Name, StringComparison.CurrentCultureIgnoreCase)))
                    name.Add(pt.Name);
            }
            return name;
        }
        /// <summary>
        /// 拷贝同件号
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<string> CopWork(UserModel user)
        {
            List<string> err = new List<string>();
            WorkInfo workInfo = work.Info.Clone() as WorkInfo;
            WorkCollection workColl = new WorkCollection(workInfo.MoldInfo);
            int workNum = workColl.Work.Count + 1;
            workInfo.WorkNumber = workNum;
            workInfo.UserModel = user;
            EDMModel edm = GetEDMModel();
            if (edm != null)
            {
                WorkCreateAssmbile create = new WorkCreateAssmbile(workInfo, edm, asm);
                err.AddRange(create.CreatePart(work.WorkpieceDirectoryPath));
                err.AddRange(create.LoadAssmbile());
                if (err.Count == 0)
                {
                    create.Work.SaveCsys(asm.PartTag);
                }
                foreach (NXOpen.Assemblies.Component elect in GetElectrodeComponent())
                {
                    err.AddRange(LoadEle(elect, create.Work.PartTag));
                }
            }
            else
            {
                err.Add("无法获取EDM");

            }

            return err;
        }

        /// <summary>
        /// 拷贝不用件号的work
        /// </summary>
        /// <param name="user"></param>
        /// <param name="workpieceName"></param>
        /// <returns></returns>
        public List<string> CopOtherWork(UserModel user, string workpieceName)
        {
            List<string> err = new List<string>();
            NXOpen.Assemblies.Component ct = GetWorkpieceModelForName(workpieceName);
            if (ct == null)
            {
                err.Add("无法找到工件！");
            }
            else
            {
                WorkpieceModel model = new WorkpieceModel(ct.Prototype as Part);
                WorkInfo workInfo = work.Info.Clone() as WorkInfo;
                WorkCollection workColl = new WorkCollection(model.Info.MoldInfo);
                int workNum = workColl.Work.Count + 1;
                workInfo.WorkNumber = workNum;
                workInfo.UserModel = user;
                workInfo.MoldInfo = model.Info.MoldInfo;
                EDMInfo edmInfo = new EDMInfo(model.Info.MoldInfo, user);
                EDMModel edmModel = new EDMModel(edmInfo);
                GetEDMName(ref edmModel);
                WorkCreateAssmbile create = new WorkCreateAssmbile(workInfo, edmModel, asm);
                err.AddRange(create.CreatePart(work.WorkpieceDirectoryPath));
                err.AddRange(create.LoadAssmbile());
                if (err.Count == 0)
                {
                    create.Work.SaveCsys(asm.PartTag);
                }
                foreach (NXOpen.Assemblies.Component elect in GetElectrodeComponent())
                {
                    err.AddRange(LoadEle(elect, create.Work.PartTag));
                }
                err.AddRange(LoadWorkpiece(edmModel.PartTag, ct));
            }
            return err;
        }
        /// <summary>
        /// 创建Work
        /// </summary>
        /// <param name="user"></param>
        /// <param name="workpieceCt"></param>
        /// <returns></returns>
        public List<string> CreateWork(UserModel user, NXOpen.Assemblies.Component workpieceCt, Matrix4 mat)
        {
            List<string> err = new List<string>();
            WorkpieceModel workpieceModel = new WorkpieceModel(workpieceCt.Prototype as Part);
            WorkInfo workInfo = work.Info.Clone() as WorkInfo;
            WorkCollection workColl = new WorkCollection(workpieceModel.Info.MoldInfo);
            int workNum = workColl.Work.Count + 1;
            workInfo.WorkNumber = workNum;
            workInfo.UserModel = user;
            workInfo.Matr = mat;
            workInfo.MatrInfo = new Matrix4Info(mat);
            EDMModel edmModel;
            bool isOther = false;
            if (workpieceModel.Info.MoldInfo.Equals(work.Info.MoldInfo))
            {
                edmModel = GetEDMModel();
            }
            else
            {
                workInfo.MoldInfo = workpieceModel.Info.MoldInfo;
                EDMInfo info = new EDMInfo(workpieceModel.Info.MoldInfo, user);
                edmModel = new EDMModel(info);
                isOther = true;
            }
            WorkCreateAssmbile create = new WorkCreateAssmbile(workInfo, edmModel, asm);
            err.AddRange(create.CreatePart(work.WorkpieceDirectoryPath));
            err.AddRange(create.LoadAssmbile());
            if (err.Count == 0)
            {
                create.Work.SaveCsys(asm.PartTag);
            }
            if (isOther)
            {
                err.AddRange(LoadWorkpiece(edmModel.PartTag, workpieceCt));
            }
            return err;
        }
        /// <summary>
        /// 拷贝电极到新Work下
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public List<string> CopyElectrodeToWork(Part pt)
        {
            List<string> err = new List<string>();
            foreach (NXOpen.Assemblies.Component eleCt in GetElectrodeComponent())
            {
                err.AddRange(LoadEle(eleCt, pt));
            }
            return err;
        }

        /// <summary>
        /// 复制电极
        /// </summary>
        /// <param name="eleCt"></param>
        /// <param name="workPt"></param>
        /// <returns></returns>
        public List<string> LoadEle(NXOpen.Assemblies.Component eleCt, Part workPt)
        {
            Matrix3x3 mat;
            Point3d setPt;
            List<string> err = new List<string>();
            eleCt.GetPosition(out setPt, out mat);
            ElectrodeSetValueInfo setValue = ElectrodeSetValueInfo.GetAttribute(eleCt);
            NXOpen.PartLoadStatus partLoadStatus1 = null;
            string partPath = (eleCt.Prototype as Part).FullPath;
            try
            {
                NXOpen.Assemblies.Component copyCt = workPt.ComponentAssembly.AddComponent(partPath, "None", eleCt.Name, setPt, mat, -1, out partLoadStatus1, true);
                NXObject obj = AssmbliesUtils.GetOccOfInstance(copyCt.Tag);
                bool attOk = setValue.SetAttribute(obj);
                AttributeUtils.AttributeOperation("EleComponentCopy", 1, obj);
                if (!attOk)
                    err.Add("写入属性错误！");
            }
            catch (NXException ex)
            {
                err.Add(eleCt.Name + "复制电极错误！           " + ex.Message);
            }
            return err;
        }
        /// <summary>
        /// 通过名字获得workpiece
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private NXOpen.Assemblies.Component GetWorkpieceModelForName(string name)
        {
            foreach (NXOpen.Assemblies.Component pt in GetWorkpieces())
            {
                if (name.Equals(pt.Name, StringComparison.CurrentCultureIgnoreCase))
                    return pt;
            }
            return null;
        }
        /// <summary>
        /// 获得EDM名字
        /// </summary>
        /// <param name="edmModel"></param>
        private void GetEDMName(ref EDMModel edmModel)
        {
            string edmPath = work.WorkpieceDirectoryPath + edmModel.AssembleName + ".prt";
            string edmName = edmModel.AssembleName;
            int count = 0;
            while (File.Exists(edmPath))
            {
                count++;
                edmModel.SetAssembleName(edmName + count.ToString());
                edmPath = work.WorkpieceDirectoryPath + edmModel.AssembleName + ".prt";
            }
        }
        /// <summary>
        /// 装配工件
        /// </summary>
        /// <param name="edm"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        private List<string> LoadWorkpiece(Part edm, NXOpen.Assemblies.Component ct)
        {
            List<string> err = new List<string>();
            try
            {
                Matrix3x3 mat;
                Point3d setPt;
                NXOpen.PartLoadStatus partLoadStatus1 = null;
                Part pt = ct.Prototype as Part;
                ct.GetPosition(out setPt, out mat);
                NXOpen.Assemblies.Component copyCt = edm.ComponentAssembly.AddComponent(pt.FullPath, "None", ct.Name, setPt, mat, -1, out partLoadStatus1, true);
            }
            catch (NXException ex)
            {
                err.Add("无法装配" + ct.Name + "工件         " + ex.Message);
            }
            return err;
        }
    }
}
