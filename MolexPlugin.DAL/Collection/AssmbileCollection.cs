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
    /// 整个打开档分类的信息
    /// </summary>
    public class AssmbileCollection
    {
        public List<ASMModel> Asm { get; private set; } = new List<ASMModel>();
        public List<WorkModel> Work { get; private set; } = new List<WorkModel>();
        public List<EDMModel> Edm { get; private set; } = new List<EDMModel>();
        public List<ElectrodeModel> Electrode { get; private set; } = new List<ElectrodeModel>();
        public List<Part> Other { get; private set; } = new List<Part>();

        public AssmbileCollection()
        {
            GetAllModel();
        }
        private void GetAllModel()
        {
            foreach (Part pt in Session.GetSession().Parts)
            {
                if (ParentAssmblieInfo.IsParent(pt))
                {
                    ParentAssmblieInfo parent = ParentAssmblieInfo.GetAttribute(pt);
                    switch (parent.Type)
                    {
                        case PartType.ASM:
                            Asm.Add(new ASMModel(pt));
                            break;
                        case PartType.EDM:
                            Edm.Add(new EDMModel(pt));
                            break;
                        case PartType.Electrode:
                            ElectrodeModel eleModel = new ElectrodeModel(pt);
                            if (eleModel.Info.AllInfo.SetValue.Positioning == "")
                                Electrode.Add(eleModel);
                            break;
                        case PartType.Work:
                            Work.Add(new WorkModel(pt));
                            break;
                        default:
                            Other.Add(pt);
                            break;
                    }
                }
            }
        }
    }
}
