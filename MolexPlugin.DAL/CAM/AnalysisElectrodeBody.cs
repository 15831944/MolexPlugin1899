using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.Utilities;
using Basic;
using MolexPlugin.Model;


namespace MolexPlugin.DAL
{
    /// <summary>
    /// 分析电极体
    /// </summary>
    public class AnalysisElectrodeBody
    {
        private Body eleBody;
        private AnalysisBodySlopeAndMinDia analysisBody = null;
        private FaceData baseFace = null;

        /// <summary>
        /// 基准面数据
        /// </summary>
        public FaceData BaseFace
        {
            get
            {
                if (baseFace == null)
                {
                    FaceData fd = GetDatumFace();
                    if (fd == null)
                        throw new Exception(" 电极基准面错误！");
                    else
                    {
                        baseFace = fd;
                    }
                }
                return baseFace;


            }
        }
        /// <summary>
        /// 基准面底面数据
        /// </summary>
        public FaceData BaseSubfaceFace
        {
            get
            {
                if (analysisBody == null)
                {
                    AnalysisElectrode();
                }
                return this.analysisBody.AnaFaces[0].Data;
            }
        }
        public AnalysisElectrodeBody(Body eleBody)
        {
            this.eleBody = eleBody;
        }

        /// <summary>
        /// 分析电极
        /// </summary>
        private void AnalysisElectrode()
        {
            this.analysisBody = new AnalysisBodySlopeAndMinDia(new Vector3d(0, 0, 1), eleBody);
            if (this.analysisBody.AnaFaces.Count >= 13)
                throw new Exception("分析体错误！");
            this.analysisBody.AnaFaces.RemoveRange(0, 4);
            this.analysisBody.AnaFaces.RemoveRange(1, 5);
        }
        /// <summary>
        /// 获得基准面
        /// </summary>
        /// <returns></returns>
        private FaceData GetDatumFace()
        {
            string er = AttributeUtils.GetAttrForString(analysisBody.AnaFaces[1].Face, "DatumFace");
            if (er.Equals("Datum", StringComparison.CurrentCultureIgnoreCase))
                return analysisBody.AnaFaces[1].Data;
            FaceLoopUtils.LoopList[] loops = FaceLoopUtils.AskFaceLoops(analysisBody.AnaFaces[0].Face.Tag);
            foreach (FaceLoopUtils.LoopList lt in FaceLoopUtils.AskFaceLoops(analysisBody.AnaFaces[0].Face.Tag)) //找到竖直面
            {
                if (lt.Type == 2)
                {
                    Edge edg = NXObjectManager.Get(lt.EdgeList[0]) as Edge;
                    foreach (Face fe in edg.GetFaces())
                    {
                        if (!fe.Equals(analysisBody.AnaFaces[0].Face))
                        {
                            foreach (Edge eg in fe.GetEdges())
                            {
                                foreach (Face fc in eg.GetFaces())
                                {
                                    FaceData fd = FaceUtils.AskFaceData(fc);
                                    double angle = UMathUtils.Angle(fd.Dir, new Vector3d(0, 0, 1));
                                    if (UMathUtils.IsEqual(angle, 0))
                                        return fd;
                                }
                            }
                        }
                    }
                }

            }
            return null;
        }

        /// <summary>
        /// 获取平面高度
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        private double GetFaceIsHighst(FaceData faceData)
        {
            double zMax = Math.Round(faceData.BoxMaxCorner.Z, 3);
            double faceZ = zMax;
            foreach (Edge eg in faceData.Face.GetEdges())
            {
                foreach (Face fa in eg.GetFaces())
                {
                    if (fa.Tag != faceData.Face.Tag)
                    {
                        FaceData data = FaceUtils.AskFaceData(fa);
                        double z = Math.Round(data.BoxMaxCorner.Z, 3);
                        if (z > zMax)
                            zMax = z;
                    }
                }
            }
            return zMax - faceZ;
        }

        /// <summary>
        /// 获取基准面最小半径
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public double GetBaseMinDia(Face baseFace)
        {
            double min = 9999;
            if (analysisBody == null)
            {
                AnalysisElectrode();
            }
            foreach (Edge eg in baseFace.GetEdges())
            {
                if (eg.SolidEdgeType == Edge.EdgeType.Circular)
                {
                    foreach (Face fe in eg.GetFaces())
                    {
                        if (!fe.Equals(baseFace))
                        {
                            AnalysisFaceSlopeAndDia af = this.analysisBody.AnaFaces.Find(a => a.Face.Equals(fe));
                            if (af.Data.IntNorm == -1 && af.MinDia != 0)
                            {
                                if (min > af.MinDia)
                                    min = af.MinDia;
                            }
                        }
                    }
                }
            }
            return min;

        }

        /// <summary>
        /// 获取平坦面（0到45度）
        /// </summary>
        /// <returns></returns>
        public void GetFlatFaces(out List<Face> slopeFaces, out double minDia)
        {
            slopeFaces = new List<Face>();
            minDia = 9999;
            if (analysisBody == null)
            {
                AnalysisElectrode();
            }
            foreach (AnalysisFaceSlopeAndDia ar in analysisBody.AnaFaces)
            {
                if (ar.MaxSlope < Math.Round(Math.PI / 2, 3) && ar.MinSlope > 0)
                {
                    // ar.face.Color = 100;
                    slopeFaces.Add(ar.Face);
                    if (ar.MinDia != 0 && ar.Data.IntNorm == -1 && minDia > ar.MinDia)
                        minDia = ar.MinDia;
                }
                if ((ar.Face.SolidFaceType == Face.FaceType.Cylindrical ||
                    ar.Face.SolidFaceType == Face.FaceType.Conical) &&
                    !UMathUtils.IsEqual(ar.MaxSlope, ar.MinSlope))
                {
                    slopeFaces.Add(ar.Face);
                    if (ar.MinDia != 0 && ar.Data.IntNorm == -1 && minDia > ar.MinDia)
                        minDia = ar.MinDia;
                }
            }

        }

        /// <summary>
        /// 获取陡峭面(45到90)
        /// </summary>
        /// <returns></returns>
        public void GetSteepFaces(out List<Face> slopeFaces, out double minDia)
        {
            slopeFaces = new List<Face>();
            minDia = 9999;
            if (analysisBody == null)
            {
                AnalysisElectrode();
            }
            foreach (AnalysisFaceSlopeAndDia ar in analysisBody.AnaFaces)
            {
                if (ar.MaxSlope < Math.Round(Math.PI / 2, 3) && ar.MinSlope > Math.Round(Math.PI / 4, 1))
                {
                    // ar.face.Color = 100;
                    slopeFaces.Add(ar.Face);
                    if (ar.MinDia != 0 && ar.Data.IntNorm == -1 && minDia > ar.MinDia)
                        minDia = ar.MinDia;
                }

            }

        }

        /// <summary>
        /// 获取斜度面
        /// </summary>
        /// <returns></returns>
        public void GetSlopeFaces(out List<Face> slopeFaces, out double minDia)
        {
            slopeFaces = new List<Face>();
            minDia = 9999;
            if (analysisBody == null)
            {
                AnalysisElectrode();
            }
            foreach (AnalysisFaceSlopeAndDia ar in analysisBody.AnaFaces)
            {
                if (ar.MaxSlope < Math.Round(Math.PI / 2, 3) && ar.MinSlope > 0)
                {
                    // ar.face.Color = 100;
                    slopeFaces.Add(ar.Face);
                    if (ar.MinDia != 0 && ar.Data.IntNorm == -1 && minDia > ar.MinDia)
                        minDia = ar.MinDia;
                }
                if ((ar.Face.SolidFaceType == Face.FaceType.Cylindrical ||
                    ar.Face.SolidFaceType == Face.FaceType.Conical) &&
                    !UMathUtils.IsEqual(ar.MaxSlope, ar.MinSlope))
                {
                    slopeFaces.Add(ar.Face);
                    if (ar.MinDia != 0 && ar.Data.IntNorm == -1 && minDia > ar.MinDia)
                        minDia = ar.MinDia;
                }
            }

        }
        /// <summary>
        /// 获取基准面其他面
        /// </summary>
        /// <returns></returns>
        public List<FaceData> GetOtherBaseFaces()
        {
            List<FaceData> faces = new List<FaceData>();
            foreach (AnalysisFaceSlopeAndDia ar in analysisBody.AnaFaces)
            {

                if (ar.Face.SolidFaceType == Face.FaceType.Planar && UMathUtils.IsEqual(ar.Data.BoxMinCorner.Z, ar.Data.BoxMaxCorner.Z) &&
                    UMathUtils.IsEqual(baseFace.BoxMaxCorner.Z, ar.Data.BoxMaxCorner.Z))
                    faces.Add(ar.Data);
            }
            return faces;
        }
        /// <summary>
        /// 获取平面
        /// </summary>       
        /// <returns></returns>
        public List<Face> GetPlaneFaces()
        {
            List<Face> plane = new List<Face>();
            if (analysisBody == null)
            {
                AnalysisElectrode();
            }
            foreach (AnalysisFaceSlopeAndDia ar in analysisBody.AnaFaces)
            {
                if (UMathUtils.IsEqual(ar.MaxSlope, 0) && UMathUtils.IsEqual(ar.MinSlope, 0) && UMathUtils.IsEqual(GetFaceIsHighst(ar.Data), 0))
                    plane.Add(ar.Face);
            }
            // plane.Remove(this.BaseFace.Face);
            // plane.Remove(this.BaseSubfaceFace.Face);
            return plane;
        }
        /// <summary>
        /// 获取所有面
        /// </summary>
        /// <param name="analyzeFace"></param>
        /// <returns></returns>
        public List<Face> GetAllFaces()
        {
            List<Face> faces = new List<Face>();
            if (analysisBody == null)
            {
                AnalysisElectrode();
            }
            foreach (AnalysisFaceSlopeAndDia ar in analysisBody.AnaFaces)
            {
                faces.Add(ar.Face);
            }
            faces.Remove(this.BaseFace.Face);
            faces.Remove(this.BaseSubfaceFace.Face);
            return faces;
        }

        /// <summary>
        /// 获取最小距离值
        /// </summary>
        /// <returns></returns>
        public double GetMinDis(ElectrodeModel eleModel)
        {
            double min = eleModel.Info.AllInfo.CAM.EleMinDim;
            double minPHX = 9999;
            double minPHY = 9999;

            if (eleModel.Info.AllInfo.Pitch.PitchX != 0 && eleModel.Info.AllInfo.Pitch.PitchXNum != 1)
            {
                minPHX = eleModel.Info.AllInfo.Pitch.PitchX - 2 * eleModel.Info.AllInfo.CAM.EleHeadDis[0];
            }
            if (eleModel.Info.AllInfo.Pitch.PitchY != 0 && eleModel.Info.AllInfo.Pitch.PitchYNum != 1)
            {
                minPHY = eleModel.Info.AllInfo.Pitch.PitchY - 2 * eleModel.Info.AllInfo.CAM.EleHeadDis[1];
            }
            if (min >= minPHX)
                min = minPHX;
            if (min >= minPHY)
                min = minPHY;
            return min;
        }
    }
}

