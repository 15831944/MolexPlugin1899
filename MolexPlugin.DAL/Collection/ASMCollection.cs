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
    /// Asm集合
    /// </summary>
    public class ASMCollection
    {

        protected ASMModel model;
        private List<MoldInfo> molds = null;
        /// <summary>
        /// 件号组立
        /// </summary>     
        protected static AssmbileCollection coll = new AssmbileCollection();
        public ASMCollection(ASMModel asm)
        {
            this.model = asm;
        }
        /// <summary>
        /// 获取模号所有信息
        /// </summary>
        public List<MoldInfo> MoldInfo
        {
            get
            {
                if (molds == null)
                    molds = GetMoldInfo();
                return molds;
            }
        }
        private List<MoldInfo> GetMoldInfo()
        {
            List<MoldInfo> info = new List<MoldInfo>();
            foreach (WorkModel wm in this.GetWorks())
            {
                Model.MoldInfo temp = wm.Info.MoldInfo;
                if (!info.Exists(a => a.Equals(temp)))
                    info.Add(temp);
            }
            return info;
        }

        /// <summary>
        /// 获取装配下面所有电极
        /// </summary>
        /// <returns></returns>
        public List<ElectrodeModel> GetElectrodes()
        {
            List<ElectrodeModel> eles = new List<ElectrodeModel>();
            foreach (ElectrodeModel ele in coll.Electrode)
            {
                try
                {
                    List<NXOpen.Assemblies.Component> ct = AssmbliesUtils.GetPartComp(this.model.PartTag, ele.PartTag);
                    if (ct.Count > 0)
                        eles.Add(ele);
                }
                catch
                {

                }
            }
            return eles;
        }
        /// <summary>
        /// 获取ASM下所有works
        /// </summary>
        /// <returns></returns>
        public List<WorkModel> GetWorks()
        {
            List<WorkModel> works = new List<WorkModel>();
            foreach (WorkModel wk in coll.Work)
            {
                try
                {
                    List<NXOpen.Assemblies.Component> ct = AssmbliesUtils.GetPartComp(this.model.PartTag, wk.PartTag);
                    if (ct.Count > 0)
                        works.Add(wk);
                }
                catch
                {

                }
            }
            return works;
        }
        /// <summary>
        /// 获取Part的父本ASM
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public static ASMModel GetAsmModel(Part part)
        {
            if (ASMModel.IsAsm(part))
                return new ASMModel(part);
            foreach (ASMModel am in coll.Asm)
            {
                try
                {
                    List<NXOpen.Assemblies.Component> ct = AssmbliesUtils.GetPartComp(am.PartTag, part);
                    if (ct.Count > 0)
                        return am;
                }
                catch
                {

                }

            }
            return null;
        }
        /// <summary>
        /// 获取WorkCollection
        /// </summary>
        /// <param name="mold"></param>
        /// <returns></returns>
        public WorkCollection GetWorkCollection(MoldInfo mold)
        {
            return new WorkCollection(mold);
        }

    }
}
