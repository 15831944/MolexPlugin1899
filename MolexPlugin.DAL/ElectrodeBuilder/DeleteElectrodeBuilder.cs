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
    /// 删除电极
    /// </summary>
    public class DeleteElectrodeBuilder
    {
        private Part elePart;
        public DeleteElectrodeBuilder(Part elePart)
        {
            this.elePart = elePart;
        }
        /// <summary>
        /// 获得电极组件
        /// </summary>
        /// <returns></returns>
        private List<NXOpen.Assemblies.Component> GetEleAllComponent()
        {
            ASMModel asm = ASMCollection.GetAsmModel(this.elePart);
            try
            {
                return AssmbliesUtils.GetPartComp(asm.PartTag, elePart);
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("获取电极组件错误！" + ex.Message);
                return null;
            }
        }

        public bool DeleteBuilder()
        {
            Part workPart = Session.GetSession().Parts.Work;
            List<NXOpen.Assemblies.Component> eleCom = GetEleAllComponent();
            ElectrodeModel eleModel = new ElectrodeModel(elePart);
            int eleNumber = eleModel.Info.AllInfo.Name.EleNumber;
            string path = elePart.FullPath;
            if (eleCom != null)
            {
                Part work = GetEleWorkPart(eleCom);
                if (work != null)
                {
                    PartUtils.SetPartDisplay(work);
                    NXObject[] objs = LayerUtils.GetAllObjectsOnLayer(eleNumber + 100);
                    LayerUtils.MoveDisplayableObject(eleNumber + 10, objs);
                }
                PartUtils.SetPartDisplay(workPart);
                Part elePart = eleCom[0].Prototype as Part;
                AssmbliesUtils.DeleteComponent(eleCom.ToArray());

                elePart.Close(BasePart.CloseWholeTree.False, BasePart.CloseModified.CloseModified, null);
                if (File.Exists(path))
                    File.Delete(path);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取电极的Work档
        /// </summary>
        /// <param name="coms"></param>
        /// <returns></returns>
        private Part GetEleWorkPart(List<NXOpen.Assemblies.Component> coms)
        {
            foreach (NXOpen.Assemblies.Component ct in coms)
            {
                NXOpen.Assemblies.Component parentCt = ct.Parent;
                if (parentCt != null)
                {
                    MoldInfo parentMold = MoldInfo.GetAttribute(parentCt.Prototype as Part);
                    MoldInfo eleMold = MoldInfo.GetAttribute(elePart);
                    if (eleMold.Equals(parentMold))
                        return parentCt.Prototype as Part;
                }
            }
            return null;
        }
    }
}
