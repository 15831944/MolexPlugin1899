using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;
using Basic;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 创建连接体特征
    /// </summary>
    public class ElectrodeMoveBuilder : IElectrodeBuilder
    {
        private IElectrodeBuilder builder = null;
        private bool isok = false;
        private List<Body> headBodys;
        private ElectrodeDatumInfo datum;
        private ElectrodeGapValueInfo gapValue;
        private ElectrodePitchInfo pitch;

        public List<Body> AllBodys { get; private set; } = new List<Body>();
        public IElectrodeBuilder ParentBuilder { get { return builder; } }

        public bool IsCreateOk { get { return isok; } }


        public ElectrodeMoveBuilder(List<Body> headBodys, ElectrodeDatumInfo datum, ElectrodeGapValueInfo gapValue, ElectrodePitchInfo pitch)
        {
            this.headBodys = headBodys;
            this.datum = datum;
            this.gapValue = gapValue;
            this.pitch = pitch;
        }
        private bool CreatWave()
        {
            Matrix4 mat = new Matrix4();
            mat.Identity();
            try
            {
                //  Body[] waveBodys = AssmbliesUtils.WaveAssociativeBodys(this.headBodys.ToArray()).GetBodies();
                PullFaceForWave(headBodys);
                NXOpen.Features.PatternGeometry patt = PatternUtils.CreatePattern(" xNCopies", "xPitchDistance", "yNCopies", " yPitchDistance"
              , mat, headBodys.ToArray()); //创建阵列(就是绝对坐标的矩阵)
                AllBodys.AddRange(patt.GetAssociatedBodies());
                AllBodys.AddRange(headBodys);
                MoveObject.CreateMoveObjToXYZ("moveX", "moveY", "moveZ", null, AllBodys.ToArray());
                SetHeadColour();
                return true;
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("创建Part档错误！" + ex.Message);
                return false;
            }

        }
        public bool CreateBuilder()
        {
            if (ParentBuilder != null)
            {
                if (!ParentBuilder.IsCreateOk)
                    ParentBuilder.CreateBuilder();
                if (!isok && ParentBuilder.IsCreateOk)
                    isok = CreatWave();
            }
            if (!isok)
                isok = CreatWave();
            return isok;
        }

        public void SetParentBuilder(IElectrodeBuilder builder)
        {
            this.builder = builder;
        }


        /// <summary>
        /// 获取最低面
        /// </summary>
        /// <returns></returns>
        private void PullFaceForWave(List<Body> bodys)
        {

            foreach (Body body in bodys)
            {
                FaceData maxFace = null;
                double zMin = 9999;

                foreach (Face face in body.GetFaces())
                {
                    FaceData data = FaceUtils.AskFaceData(face);
                    Point3d center = UMathUtils.GetMiddle(data.BoxMaxCorner, data.BoxMinCorner);
                    if (zMin > center.Z)
                    {
                        zMin = center.Z;
                        maxFace = data;
                    }
                }
                if (maxFace != null)
                {
                    double z = maxFace.BoxMaxCorner.Z + this.datum.EleHeight;
                    if (z > 0)
                        try
                        {
                            SynchronousUtils.CreatePullFace(new Vector3d(0, 0, -1), z, maxFace.Face);
                        }
                        catch
                        {

                        }
                }
            }

        }
        /// <summary>
        /// 给齿上颜色
        /// </summary>
        private void SetHeadColour()
        {
            Matrix4 matr = new Matrix4();
            matr.Identity();
            Matrix4 inv = matr.GetInversMatrix();
            CartesianCoordinateSystem csys = BoundingBoxUtils.CreateCoordinateSystem(matr, inv);
            List<Body> bodys = Session.GetSession().Parts.Work.Bodies.ToArray().ToList();
            var toolhList = bodys.GroupBy(a => AttributeUtils.GetAttrForString(a, "ToolhName"));
            List<ElectrodeToolhInfo[,]> toolhInfos = new List<ElectrodeToolhInfo[,]>();
            try
            {
                foreach (var toolh in toolhList)
                {
                    ElectrodeToolhInfo[,] toolhInfo = pitch.GetToolhInfosForAttribute(toolh.ToList(), matr, csys);
                    toolhInfos.Add(toolhInfo);
                }
                if (toolhInfos.Count != 0)
                    gapValue.SetERToolh(toolhInfos);
            }
            catch (Exception ex)
            {
                ClassItem.WriteLogFile("设置颜色错误！" + ex.Message);
            }
        }


    }
}
