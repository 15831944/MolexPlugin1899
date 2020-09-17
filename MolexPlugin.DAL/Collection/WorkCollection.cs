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
    /// 同件号下的Work
    /// </summary>
    public class WorkCollection
    {
        private List<WorkModel> workModel = new List<WorkModel>();
        private List<ElectrodeModel> electrodes = new List<ElectrodeModel>();
        private List<EDMModel> edmModel = new List<EDMModel>();
        protected AssmbileCollection coll;
        private MoldInfo info;
        /// <summary>
        /// 同件号下的Work
        /// </summary>
        public List<WorkModel> Work
        {
            get
            {
                if (workModel.Count == 0)
                    workModel = GetWorkpieceNumWorks();
                return workModel;
            }
        }
        /// <summary>
        /// 同件号下的电极
        /// </summary>
        public List<ElectrodeModel> Electrodes
        {
            get
            {
                if (electrodes.Count == 0)
                    electrodes = GetEqualMoldInfoEletrode();
                return electrodes;
            }
        }
        /// <summary>
        /// 同一件号下的所有EDM
        /// </summary>
        public List<EDMModel> EdmModel
        {
            get
            {
                if (edmModel.Count == 0)
                    edmModel = GetEdm();
                return edmModel;

            }
        }
        public WorkCollection(MoldInfo moldInfo)
        {
            this.info = moldInfo;
            coll = new AssmbileCollection();
        }
        /// <summary>
        /// 获取同一个模号件号的电极
        /// </summary>
        /// <param name="mold"></param>
        /// <returns></returns>
        private List<ElectrodeModel> GetEqualMoldInfoEletrode()
        {
            List<ElectrodeModel> ele = new List<ElectrodeModel>();
            foreach (ElectrodeModel em in coll.Electrode)
            {
                if (em.Info.MoldInfo.Equals(info))
                {
                    ele.Add(em);
                }
            }
            ele.Sort();
            return ele;
        }
        /// <summary>
        /// 获取件号下面所有Work
        /// </summary>
        /// <returns></returns>
        private List<WorkModel> GetWorkpieceNumWorks()
        {
            List<WorkModel> work = new List<WorkModel>();
            foreach (WorkModel wm in coll.Work)
            {
                if (wm.Info.MoldInfo.Equals(info))
                {
                    work.Add(wm);
                }
            }
            work.Sort();
            return work;
        }

        /// <summary>
        /// 获取EDm
        /// </summary>
        /// <returns></returns>
        private List<EDMModel> GetEdm()
        {
            List<EDMModel> emms = new List<EDMModel>();
            foreach (EDMModel em in coll.Edm)
            {
                if (em.Info.MoldInfo.Equals(this.info))
                {
                    emms.Add(em);
                }
            }
            emms.Sort();
            return emms;
        }
        /// <summary>
        /// 获取Work下的电极
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<ElectrodeModel> GetElectrodeForWork(WorkModel model)
        {
            List<ElectrodeModel> eles = new List<ElectrodeModel>();
            foreach (ElectrodeModel ele in coll.Electrode)
            {
                try
                {
                    List<NXOpen.Assemblies.Component> ct = AssmbliesUtils.GetPartComp(model.PartTag, ele.PartTag);
                    if (ct.Count > 0)
                        eles.Add(ele);
                }
                catch
                {

                }
            }
            eles.Sort();
            return eles;
        }
        /// <summary>
        /// 获取EDM
        /// </summary>
        /// <param name="work"></param>
        /// <returns></returns>
        public EDMCollection GetEdmCollection(WorkModel work)
        {
            return new EDMCollection(work);
        }
        /// <summary>
        /// 获取主工件
        /// </summary>
        /// <param name="workPiece"></param>
        /// <returns></returns>
        public Part GetHostWorkpiece()
        {
            string name = info.MoldNumber + info.WorkpieceNumber + info.EditionNumber;
            foreach (Part pt in coll.Other)
            {
                if (name.Equals(pt.Name.Replace("-", ""), StringComparison.CurrentCultureIgnoreCase))
                {
                    return pt;
                }
            }
            return null;
        }
    }
}
