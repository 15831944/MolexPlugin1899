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

namespace MolexPlugin.DAL
{

    /// <summary>
    /// 替换其他组件
    /// </summary>
    public class ReplaceOther
    {
        private Part pt;
        private string directoryPath;
        private ParentAssmblieInfo newInfo;
        public ReplaceOther(Part pt, ParentAssmblieInfo info)
        {
            this.pt = pt;
            string oldPath = pt.FullPath;
            this.directoryPath = Path.GetDirectoryName(oldPath) + "\\";
            this.newInfo = info;
        }
        public List<string> Alter(string newName)
        {
            string newPath = this.directoryPath + newName + ".prt";
            Part newPart;
            List<string> err = ReplacePart.Replace(pt, newPath, newName, out newPart);
            if (newPart != null)
            {
                ParentAssmblieInfo info = ParentAssmblieInfo.GetAttribute(newPart);
                info.MoldInfo = newInfo.MoldInfo;
                info.UserModel = newInfo.UserModel;
                info.SetAttribute(newPart);
            }
            return err;
        }

    }
}
