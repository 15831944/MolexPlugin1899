using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MolexPlugin.Model;
using NXOpen;
using NXOpen.UF;
using System.IO;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 非标电极
    /// </summary>
    public class NonStandardElectrodeCAM : AbstractElectrodeCAM
    {
        public NonStandardElectrodeCAM(Part pt, UserModel user) : base(pt, user)
        {
            this.IsCompute = false;
            this.IsOffsetInter = false;
        }

        public override bool CreateNewFile(string filePath)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            string name = pt.Name;
            string ptPath = pt.FullPath;
            string newPath = filePath + name + "//";
            string newPtPath = newPath + pt.Name + ".part";
            if (Directory.Exists(newPath))
            {
                Directory.Delete(newPath);
            }
            Directory.CreateDirectory(newPath);
            this.pt.Close(BasePart.CloseWholeTree.False, BasePart.CloseModified.CloseModified, null);
            try
            {
                File.Move(ptPath, newPtPath);
                Tag partTag;
                UFPart.LoadStatus err;
                theUFSession.Part.Open(newPtPath, out partTag, out err);
                return true;
            }
            catch
            {
                Tag partTag;
                UFPart.LoadStatus err;
                theUFSession.Part.Open(ptPath, out partTag, out err);
                return false;
            }
        }

        public override bool CreateOffsetInter()
        {
            offser.SetAttribute(false);
            return false;
        }

        public override Dictionary<double, Face[]> GetAllFaces()
        {
            return new Dictionary<double, Face[]>();
        }

        public override Dictionary<double, BoundaryModel[]> GetBaseFaceBoundary()
        {
            return new Dictionary<double, BoundaryModel[]>();
        }

        public override Line[] GetBaseFaceLine()
        {
            return null;
        }


        public override Dictionary<double, Face[]> GetFlatFaces()
        {
            return new Dictionary<double, Face[]>();
        }

        public override Dictionary<double, Face[]> GetPlaneFaces()
        {
            return new Dictionary<double, Face[]>();
        }

        public override Dictionary<double, Face[]> GetSlopeFaces()
        {
            return new Dictionary<double, Face[]>();
        }

        public override Dictionary<double, Face[]> GetSteepFaces()
        {
            return new Dictionary<double, Face[]>();
        }

        public override CompterToolName GetTool()
        {
            return new CompterToolName(this.analysis, 3.0);
        }
    }
}
