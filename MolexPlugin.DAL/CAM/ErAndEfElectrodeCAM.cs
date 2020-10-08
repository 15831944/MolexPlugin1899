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
    /// ER和EF电极
    /// </summary>
    public class ErAndEfElectrodeCAM : AbstractElectrodeCAM
    {
        private ElectrodeModel model;
        public ErAndEfElectrodeCAM(ElectrodeModel model, UserModel user) : base(model.PartTag, user)
        {
            this.IsCompute = true;
            this.model = model;
        }

        public override bool CreateNewFile(string filePath)
        {
            if (model.Info.AllInfo.Name.EleName.Equals(this.pt.Name, StringComparison.CurrentCultureIgnoreCase))
                throw new Exception("电极名与属性名不一样。");
            UFSession theUFSession = UFSession.GetUFSession();
            string name = pt.Name;
            string moldName = model.Info.MoldInfo.MoldNumber;
            string ptPath = pt.FullPath;
            string moldPath = filePath + moldName + "//";
            string newPath = moldPath + name + "//";
            string newPtPath = newPath + pt.Name + ".part";
            if (!Directory.Exists(moldPath)) //模号文件夹
            {
                Directory.CreateDirectory(moldPath);
            }
            if (Directory.Exists(newPath)) //电极号文件夹
            {
                Directory.Delete(newPath);
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
            List<Face> erFaces = new List<Face>();
            List<Face> efFaces = new List<Face>();
            GetEFanERFace(allFace, out erFaces, out efFaces);
            ElectrodeGapValueInfo gap = this.model.Info.AllInfo.GapValue;
            this.IsOffsetInter = offser.SetOffsetInter(erFaces, gap.CrudeInter, efFaces, gap.FineInter);
            if (!this.IsOffsetInter)
                this.Inter = gap.FineInter;
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
                GetEFanERFace(allFace, out dic);
                return dic;
            }
            else
                return new Dictionary<double, Face[]>();
        }

        public override Dictionary<double, BoundaryModel[]> GetBaseFaceBoundary()
        {
            Dictionary<double, BoundaryModel[]> dic = new Dictionary<double, BoundaryModel[]>();
            if (this.IsOffsetInter)
            {
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
                dic.Add(0, boundary.ToArray());
            }
            else
            {
                List<BoundaryModel> erBoundary = new List<BoundaryModel>();
                List<BoundaryModel> efBoundary = new List<BoundaryModel>();
                List<FaceData> other = analysis.GetOtherBaseFaces();
                if (other.Count > 0)
                {

                    List<FaceData> er = new List<FaceData>();
                    List<FaceData> ef = new List<FaceData>();
                    GetEFanERFace(other, out er, out ef);
                    foreach (FaceData fd in er)
                    {
                        PlanarBoundary pb = new PlanarBoundary(fd);
                        BoundaryModel model;
                        double blank;
                        pb.GetPeripheralBoundary(out model, out blank);
                        erBoundary.Add(model);
                    }
                    foreach (FaceData fd in ef)
                    {
                        PlanarBoundary pb = new PlanarBoundary(fd);
                        BoundaryModel model;
                        double blank;
                        pb.GetPeripheralBoundary(out model, out blank);
                        efBoundary.Add(model);
                    }
                }
                PlanarBoundary pby = new PlanarBoundary(analysis.BaseFace);
                List<BoundaryModel> boundary = pby.GetHoleBoundary();
                foreach (BoundaryModel bm in boundary)
                {
                    Edge eg = bm.Curves[0] as Edge;
                    foreach (Face fe in eg.GetFaces())
                    {
                        string er = AttributeUtils.GetAttrForString(fe, "ToolhGapValue");
                        if (er.Equals("ER", StringComparison.CurrentCultureIgnoreCase))
                        {
                            erBoundary.Add(bm);
                            break;
                        }
                        else if (er.Equals("EF", StringComparison.CurrentCultureIgnoreCase))
                        {
                            efBoundary.Add(bm);
                            break;
                        }
                    }
                }
                ElectrodeGapValueInfo gap = this.model.Info.AllInfo.GapValue;
                dic.Add(gap.CrudeInter, erBoundary.ToArray());
                dic.Add(gap.FineInter, efBoundary.ToArray());
            }

            return dic;
        }

        public override Line[] GetBaseFaceLine()
        {
            Part workPart = Session.GetSession().Parts.Work;
            Point3d minPt = analysis.BaseFace.BoxMinCorner;
            double length = analysis.BaseFace.BoxMaxCorner.X - analysis.BaseFace.BoxMinCorner.X;
            double width = analysis.BaseFace.BoxMinCorner.Y - analysis.BaseFace.BoxMinCorner.Y;
            double z = analysis.BaseFace.BoxMinCorner.Z;

            Line line1 = workPart.Curves.CreateLine(new Point3d(minPt.X + 3.5, minPt.Y - 1, z), new Point3d(minPt.X - 1, minPt.Y + 3.5, z));
            line1.Layer = 254;
            Line line2 = workPart.Curves.CreateLine(new Point3d(minPt.X - 1, minPt.Y + width - 3.5, z), new Point3d(minPt.X + 3.5, minPt.Y + width + 1, z));
            line2.Layer = 254;
            Line line3 = workPart.Curves.CreateLine(new Point3d(minPt.X + length - 1, minPt.Y + 3.5, z), new Point3d(minPt.X + length - 3.5, minPt.Y - 1, z));
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
                GetEFanERFace(flat, out dic);
                return dic;
            }
            else
                return new Dictionary<double, Face[]>();
        }

        public override Dictionary<double, Face[]> GetPlaneFaces()
        {
            double min;
            List<Face> plane = analysis.GetPlaneFaces();
            Dictionary<double, Face[]> dic = new Dictionary<double, Face[]>();
            analysis.GetFlatFaces(out plane, out min);
            if (plane.Count > 0)
            {
                GetEFanERFace(plane, out dic);
                return dic;
            }
            else
                return new Dictionary<double, Face[]>();
        }

        public override Dictionary<double, Face[]> GetSlopeFaces()
        {
            double min;
            List<Face> slope = new List<Face>();
            Dictionary<double, Face[]> dic = new Dictionary<double, Face[]>();
            analysis.GetSlopeFaces(out slope, out min);
            if (slope.Count > 0)
            {
                GetEFanERFace(slope, out dic);
                return dic;
            }
            else
                return new Dictionary<double, Face[]>();
        }

        public override Dictionary<double, Face[]> GetSteepFaces()
        {
            double min;
            List<Face> steep = new List<Face>();
            Dictionary<double, Face[]> dic = new Dictionary<double, Face[]>();
            analysis.GetSteepFaces(out steep, out min);
            if (steep.Count > 0)
            {
                GetEFanERFace(steep, out dic);
                return dic;
            }
            else
                return new Dictionary<double, Face[]>();
        }

        public override CompterToolName GetTool()
        {
            return new CompterToolName(this.analysis, analysis.GetMinDis(this.model));
        }
        /// <summary>
        /// 获取ER和EF面
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="erFaces"></param>
        /// <param name="efFaces"></param>
        private void GetEFanERFace(List<Face> faces, out List<Face> erFaces, out List<Face> efFaces)
        {
            erFaces = new List<Face>();
            efFaces = new List<Face>();
            Dictionary<double, Face[]> dic = new Dictionary<double, Face[]>();
            if (faces.Count > 0)
            {
                foreach (Face fe in faces)
                {
                    string er = AttributeUtils.GetAttrForString(fe, "ToolhGapValue");
                    if (er.Equals("ER", StringComparison.CurrentCultureIgnoreCase))
                    {
                        erFaces.Add(fe);
                    }
                    else if (er.Equals("EF", StringComparison.CurrentCultureIgnoreCase))
                    {
                        efFaces.Add(fe);
                    }
                }
            }
        }
        private void GetEFanERFace(List<FaceData> faces, out List<FaceData> erFaces, out List<FaceData> efFaces)
        {
            erFaces = new List<FaceData>();
            efFaces = new List<FaceData>();
            Dictionary<double, Face[]> dic = new Dictionary<double, Face[]>();
            if (faces.Count > 0)
            {
                foreach (FaceData fe in faces)
                {
                    string er = AttributeUtils.GetAttrForString(fe.Face, "ToolhGapValue");
                    if (er.Equals("ER", StringComparison.CurrentCultureIgnoreCase))
                    {
                        erFaces.Add(fe);
                    }
                    else if (er.Equals("EF", StringComparison.CurrentCultureIgnoreCase))
                    {
                        efFaces.Add(fe);
                    }
                }
            }


        }
        private void GetEFanERFace(List<Face> faces, out Dictionary<double, Face[]> dic)
        {
            List<Face> erFaces = new List<Face>();
            List<Face> efFaces = new List<Face>();
            dic = new Dictionary<double, Face[]>();
            if (faces.Count > 0)
            {
                foreach (Face fe in faces)
                {
                    string er = AttributeUtils.GetAttrForString(fe, "ToolhGapValue");
                    if (er.Equals("ER", StringComparison.CurrentCultureIgnoreCase))
                    {
                        erFaces.Add(fe);
                    }
                    else if (er.Equals("EF", StringComparison.CurrentCultureIgnoreCase))
                    {
                        efFaces.Add(fe);
                    }
                }
            }
            if (!this.IsOffsetInter)
            {

                dic.Add(this.model.Info.AllInfo.GapValue.FineInter, efFaces.ToArray());
                dic.Add(this.model.Info.AllInfo.GapValue.DuringInter, erFaces.ToArray());
            }
            else
                dic.Add(0, faces.ToArray());

        }
    }
}
