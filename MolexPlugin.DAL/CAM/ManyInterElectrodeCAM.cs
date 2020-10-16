using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MolexPlugin.Model;
using NXOpen;
using NXOpen.UF;
using System.IO;
using Basic;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 多种间隙多只电极
    /// </summary>
    public class ManyInterElectrodeCAM : AbstractElectrodeCAM
    {
        private double tempInter;
        private string interName;
        private ElectrodeModel model;
        public ManyInterElectrodeCAM(ElectrodeModel model, UserModel user) : base(model.PartTag, user)
        {
            this.IsCompute = true;
            this.model = model;
            ElectrodeGapValueInfo gap = model.Info.AllInfo.GapValue;
            if (gap.FineInter != 0)
            {
                tempInter = gap.FineInter;
                interName = "F";
                return;
            }

            if (gap.DuringInter != 0)
            {
                tempInter = gap.DuringInter;
                interName = "D";
                return;
            }
            if (gap.CrudeInter != 0)
            {
                tempInter = gap.CrudeInter;
                interName = "R";
                return;
            }
        }

        public override bool CreateNewFile(string filePath)
        {
            if (!model.Info.AllInfo.Name.EleName.Equals(this.pt.Name, StringComparison.CurrentCultureIgnoreCase))
                throw new Exception("电极名与属性名不一样。");
            UFSession theUFSession = UFSession.GetUFSession();
            string name = pt.Name;
            string moldName = model.Info.MoldInfo.MoldNumber;
            string ptPath = pt.FullPath;
            string moldPath = filePath + moldName + "\\";
            string elePath = moldPath + name + "\\";
            string newPath = elePath + interName + "\\";
            string newPtPath = newPath + pt.Name + ".prt";
            if (!Directory.Exists(moldPath)) //模号文件夹
            {
                Directory.CreateDirectory(moldPath);
            }
            if (Directory.Exists(elePath)) //电极号文件夹
            {
                Directory.Delete(elePath, true);
            }
            Directory.CreateDirectory(elePath);
            if (Directory.Exists(newPath)) //电极号文件夹
            {
                Directory.Delete(newPath, true);
            }
            Directory.CreateDirectory(newPath);
            this.pt.Save(BasePart.SaveComponents.False, BasePart.CloseAfterSave.False);
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
            if (allFace.Count == 0)
                allFace = analysis.GetAllFaces();
            this.IsOffsetInter = offser.SetOffsetInter(allFace, tempInter);
            if (!this.IsOffsetInter)
                this.Inter = this.tempInter;
            else
                this.Inter = 0;
            return false;
        }

        public override Dictionary<double, Face[]> GetAllFaces()
        {
            if (allFace.Count == 0)
                allFace = analysis.GetAllFaces();
            Dictionary<double, Face[]> dic = new Dictionary<double, Face[]>();
            if (allFace.Count > 0)
            {
                if (this.IsOffsetInter)
                {
                    dic.Add(0, allFace.ToArray());
                }
                else
                {
                    dic.Add(tempInter, allFace.ToArray());
                }
            }
            return dic;
        }

        public override Dictionary<double, BoundaryModel[]> GetBaseFaceBoundary()
        {
            Dictionary<double, BoundaryModel[]> dic = new Dictionary<double, BoundaryModel[]>();
            List<BoundaryModel> boundary = new List<BoundaryModel>();
            List<FaceData> other = analysis.GetOtherBaseFaces();
            if (other.Count > 0)
            {
                foreach (FaceData fe in other)
                {
                    PlanarBoundary pb = new PlanarBoundary(fe);
                    BoundaryModel model;
                    double blank;
                    pb.GetPeripheralBoundary(out model, out blank);
                    boundary.Add(model);
                }
            }
            PlanarBoundary pby = new PlanarBoundary(analysis.BaseFace);
            boundary.AddRange(pby.GetHoleBoundary());
            if (this.IsOffsetInter)
            {

                dic.Add(0, boundary.ToArray());
            }
            else
            {
                dic.Add(tempInter, boundary.ToArray());
            }

            return dic;
        }

        public override Line[] GetBaseFaceLine()
        {
            Part workPart = Session.GetSession().Parts.Work;
            Point3d minPt = analysis.BaseFace.BoxMinCorner;
            double length = analysis.BaseFace.BoxMaxCorner.X - analysis.BaseFace.BoxMinCorner.X;
            double width = analysis.BaseFace.BoxMaxCorner.Y - analysis.BaseFace.BoxMinCorner.Y;
            double z = analysis.BaseFace.BoxMinCorner.Z;

            Line line1 = workPart.Curves.CreateLine(new Point3d(minPt.X + 3.5, minPt.Y - 1, z), new Point3d(minPt.X - 1, minPt.Y + 3.5, z));
            line1.Layer = 254;
            Line line2 = workPart.Curves.CreateLine(new Point3d(minPt.X - 1, minPt.Y + width - 3.5, z), new Point3d(minPt.X + 3.5, minPt.Y + width + 1, z));
            line2.Layer = 254;
            Line line3 = workPart.Curves.CreateLine(new Point3d(minPt.X + length + 1, minPt.Y + 3.5, z), new Point3d(minPt.X + length - 3.5, minPt.Y - 1, z));
            line3.Layer = 254;
            return new Line[3] { line1, line2, line3 };
        }


        public override Dictionary<double, Face[]> GetFlatFaces()
        {
            double min;
            List<Face> flat = new List<Face>();
            Dictionary<double, Face[]> dic = new Dictionary<double, Face[]>();
            analysis.GetFlatFaces(out flat, out min);
            if (flat.Count > 0)
            {
                if (this.IsOffsetInter)
                {
                    dic.Add(0, flat.ToArray());
                }
                else
                {
                    dic.Add(tempInter, flat.ToArray());
                }
            }
            return dic;
        }

        public override Dictionary<double, Face[]> GetPlaneFaces()
        {

            List<Face> plane = analysis.GetPlaneFaces();
            Dictionary<double, Face[]> dic = new Dictionary<double, Face[]>();
            if (plane.Count > 0)
            {
                if (this.IsOffsetInter)
                {
                    dic.Add(0, plane.ToArray());
                }
                else
                {
                    dic.Add(tempInter, plane.ToArray());
                }
            }
            return dic;
        }

        public override Dictionary<double, Face[]> GetSlopeFaces()
        {
            double min;
            List<Face> slope = new List<Face>();
            Dictionary<double, Face[]> dic = new Dictionary<double, Face[]>();
            analysis.GetSlopeFaces(out slope, out min);
            if (slope.Count > 0)
            {
                if (this.IsOffsetInter)
                {
                    dic.Add(0, slope.ToArray());
                }
                else
                {
                    dic.Add(tempInter, slope.ToArray());
                }
            }
            return dic;
        }

        public override Dictionary<double, Face[]> GetSteepFaces()
        {
            double min;
            List<Face> steep = new List<Face>();
            Dictionary<double, Face[]> dic = new Dictionary<double, Face[]>();
            analysis.GetSteepFaces(out steep, out min);
            if (steep.Count > 0)
            {
                if (this.IsOffsetInter)
                {
                    dic.Add(0, steep.ToArray());
                }
                else
                {
                    dic.Add(tempInter, steep.ToArray());
                }
            }
            return dic;
        }

        public override CompterToolName GetTool()
        {
            return new CompterToolName(this.analysis, analysis.GetMinDis(this.model));
        }

    }
}
