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
    /// 替换组件
    /// </summary>
    public class ReplacePart
    {
        /// <summary>
        /// 替换组件
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="info"></param>
        /// <param name="newPath"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public static List<string> Replace(Part pt, string newPath, string newName, out Part newPart)
        {
            List<string> err = new List<string>();
            newPart = null;
            Session theSession = Session.GetSession();
            UFSession theUFSession = UFSession.GetUFSession();
            Part workPart = theSession.Parts.Work;
            List<Component> cts = AssmbliesUtils.GetPartComp(workPart, pt);
            string oldPath = pt.FullPath;
            if (File.Exists(newPath))
            {
                err.Add(newName + "            替换失败，替换后有同名工件！          ");
                return err;
            }
            else
            {
                pt.Close(NXOpen.BasePart.CloseWholeTree.False, NXOpen.BasePart.CloseModified.UseResponses, null);
                File.Move(oldPath, newPath);
                if (cts.Count > 0)
                {
                    foreach (Component co in cts)
                    {
                        try
                        {
                            bool rep = AssmbliesUtils.ReplaceComp(co, newPath, newName);
                            if (rep)
                                err.Add(newName + "           组件替换成功！          ");
                            else
                                err.Add(newName + "           组件替换失败！          ");
                        }
                        catch
                        {
                            err.Add(newName + "           组件替换失败！          ");
                        }
                    }
                    newPart = cts[0].Prototype as Part;
                    return err;
                }
                else
                {
                    Tag partTag;
                    UFPart.LoadStatus error_status;
                    theUFSession.Part.Open(newPath, out partTag, out error_status);
                    err.Add(newName + "           组件替换成功！          ");
                    newPart = NXObjectManager.Get(partTag) as Part;
                    return err;
                }
            }
        }
    }
}
