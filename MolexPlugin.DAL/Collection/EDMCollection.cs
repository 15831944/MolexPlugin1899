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
    ///work下面EDM
    /// </summary>
    public class EDMCollection 
    {
        private WorkModel workModel;
        private List<EDMModel> edmModel = new List<EDMModel>();
        private List<Part> workPieces = new List<Part>();
        protected static AssmbileCollection coll = new AssmbileCollection();
        public List<EDMModel> EdmModel
        {
            get
            {
                if (edmModel.Count == 0)
                    edmModel = GetEdm();
                return edmModel;

            }
        }
        public EDMCollection(WorkModel model) 
        {
            this.workModel = model;
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
                if (em.Info.MoldInfo.Equals(workModel.Info.MoldInfo))
                {
                    try
                    {
                        List<NXOpen.Assemblies.Component> ct = AssmbliesUtils.GetPartComp(this.workModel.PartTag, em.PartTag);
                        if (ct.Count > 0)
                            emms.Add(em);
                    }
                    catch
                    {

                    }
                }
            }
            return emms;
        }
        /// <summary>
        /// 获取EDM下面工件
        /// </summary>
        /// <returns></returns>
        public List<Part> GetWorkPieces()
        {
            List<Part> parts = new List<Part>();

            foreach (Part pt in coll.Other)
            {
                try
                {
                    List<NXOpen.Assemblies.Component> ct = AssmbliesUtils.GetPartComp(this.workModel.PartTag, pt);
                    if (ct.Count > 0)
                        parts.Add(pt);
                }
                catch
                {

                }
            }
            parts.Sort(delegate (Part a, Part b)
            {
                return a.Name.CompareTo(b.Name);
            }
            );
            return parts;
        }


    }
}
