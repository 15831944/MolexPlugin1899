using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.Utilities;
using NXOpen.Assemblies;
using NXOpen.UF;
using Basic;
using MolexPlugin.Model;
using System.Text.RegularExpressions;
using NXOpen.Drawings;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 替换电极组件
    /// </summary>
    public class ReplaceElectrode
    {
        private Part elePt;
        private string directoryPath;
        private ElectrodeNameInfo oldNameInfo;
        private ElectrodeNameInfo newNameInfo;
        public ReplaceElectrode(Part pt, ElectrodeNameInfo newNameInfo)
        {
            this.elePt = pt;
            this.oldNameInfo = ElectrodeNameInfo.GetAttribute(elePt);
            string oldPath = elePt.FullPath;
            this.directoryPath = Path.GetDirectoryName(oldPath) + "\\";
            this.newNameInfo = newNameInfo;
        }
        /// <summary>
        /// 修改电极名
        /// </summary>
        /// <returns></returns>
        public List<string> AlterEle()
        {
            Part newElePart;
            string newEleName = elePt.Name.Replace(oldNameInfo.EleName, newNameInfo.EleName);
            string newPath = directoryPath + newEleName + ".prt";
            List<string> err = ReplacePart.Replace(elePt, newPath, newEleName, out newElePart);
            if (newElePart != null)
                newNameInfo.SetAttribute(newElePart);
            return err;
        }
        public List<string> AlterEle(ParentAssmblieInfo parenInfo)
        {
            Part newElePart;
            string newEleName = elePt.Name.Replace(oldNameInfo.EleName, newNameInfo.EleName);
            string newPath = directoryPath + newEleName + ".prt";
            ParentAssmblieInfo info = ParentAssmblieInfo.GetAttribute(elePt);
            List<string> err = ReplacePart.Replace(elePt, newPath, newEleName, out newElePart);
            if (newElePart != null)
            {                             
                info.MoldInfo = parenInfo.MoldInfo;
                info.UserModel = parenInfo.UserModel;
                newNameInfo.SetAttribute(newElePart);
                info.SetAttribute(newElePart);
            }
            return err;
        }
        /// <summary>
        /// 修改电极图纸
        /// </summary>
        /// <returns></returns>
        public List<string> AlterEleDra()
        {
            List<string> err = new List<string>();
            UFSession theUFSession = UFSession.GetUFSession();
            string oldDra = directoryPath + oldNameInfo.EleName + "_dwg.prt";
            string newDra = directoryPath + newNameInfo.EleName + "_dwg.prt";
            if (File.Exists(newDra))
            {
                return err;
            }
            else
            {
                foreach (Part pt in Session.GetSession().Parts)
                {
                    if (pt.Name.Equals(oldNameInfo.EleName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        pt.Close(NXOpen.BasePart.CloseWholeTree.False, NXOpen.BasePart.CloseModified.UseResponses, null);
                    }
                }
                File.Move(oldDra, newDra);
                Tag partTag;
                UFPart.LoadStatus error_status;
                theUFSession.Part.Open(newDra, out partTag, out error_status);
                err.Add(newNameInfo.EleName + "           图纸替换成功！          ");
                Part newPart = NXObjectManager.Get(partTag) as Part;
                if (newPart != null)
                    newNameInfo.SetAttribute(newPart);
                return err;
            }

        }
        /// <summary>
        /// 替换电极图纸
        /// </summary>
        /// <param name="oldParenInfo"></param>
        /// <param name="newParenInfo"></param>
        /// <returns></returns>
        public List<string> AlterEleDra(ParentAssmblieInfo oldParenInfo, ParentAssmblieInfo newParenInfo)
        {
            List<string> err = new List<string>();
            UFSession theUFSession = UFSession.GetUFSession();
            Part workPart = Session.GetSession().Parts.Work;
            string oldDra = directoryPath + oldNameInfo.EleName + "_dwg.prt";
            string newDra = directoryPath + newNameInfo.EleName + "_dwg.prt";
            if (File.Exists(newDra))
            {
                return err;
            }
            else
            {
                foreach (Part pt in Session.GetSession().Parts)
                {
                    if (pt.Name.Equals(oldNameInfo.EleName + "_dwg", StringComparison.CurrentCultureIgnoreCase))
                    {
                        pt.Close(NXOpen.BasePart.CloseWholeTree.False, NXOpen.BasePart.CloseModified.UseResponses, null);
                    }
                }
                File.Move(oldDra, newDra);
                Tag partTag;
                UFPart.LoadStatus error_status;
                theUFSession.Part.Open(newDra, out partTag, out error_status);
                Part newPart = NXObjectManager.Get(partTag) as Part;

                PartUtils.SetPartDisplay(newPart);
                NXOpen.Assemblies.Component comp = newPart.ComponentAssembly.RootComponent.GetChildren()[0];
                string workName = comp.Name.Replace(oldParenInfo.MoldInfo.MoldNumber + "-" + oldParenInfo.MoldInfo.WorkpieceNumber, newParenInfo.MoldInfo.MoldNumber + "-" + newParenInfo.MoldInfo.WorkpieceNumber);
                try
                {
                    Basic.AssmbliesUtils.ReplaceComp(comp, this.directoryPath + workName + ".prt", workName);
                }
                catch
                {

                }
                if (newPart != null)
                {
                    newNameInfo.SetAttribute(newPart);
                    ParentAssmblieInfo info = ParentAssmblieInfo.GetAttribute(newPart);
                    info.MoldInfo = newParenInfo.MoldInfo;
                    info.UserModel = newParenInfo.UserModel;
                    info.SetAttribute(newPart);
                    foreach (DrawingSheet st in newPart.DrawingSheets)
                    {
                        Basic.DrawingUtils.UpdateViews(st);
                    }
                }
                PartUtils.SetPartDisplay(workPart);
                err.Add(newNameInfo.EleName + "           图纸替换成功！          ");
                return err;
            }

        }
    }
}
