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
            List<string> err = new List<string>();
            if (File.Exists(newPath))
            {
                err.Add(newName + "            替换失败，替换后有同名工件！          ");
                return err;
            }
            Part newPart;
            err.AddRange(ReplacePart.Replace(pt, newPath, newName, out newPart));
            if (newPart != null)
            {
                newInfo.SetAttribute(newPart);
            }
            else
            {
                newInfo.SetAttribute(pt);
            }
            return err;
        }

    }
}
