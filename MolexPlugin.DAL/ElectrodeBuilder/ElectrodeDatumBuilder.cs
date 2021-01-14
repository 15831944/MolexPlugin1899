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
    /// 创建草绘特征
    /// </summary>
    public class ElectrodeDatumBuilder : IElectrodeBuilder
    {
        private ElectrodeSketchBuilder sketch;
        private IElectrodeBuilder builder = null;
        private bool isok = false;

        /// <summary>
        /// 基准台实体
        /// </summary>
        public Body DatumBody { get; private set; }

        public IElectrodeBuilder ParentBuilder { get { return builder; } }

        public bool IsCreateOk { get { return isok; } }


        public ElectrodeDatumBuilder(ElectrodeSketchBuilder sketch)
        {
            this.sketch = sketch;
        }
        private bool CreateDatum()
        {
            Vector3d dir = new Vector3d(0, 0, -1);
            try
            {
                Body body1 = ExtrudeUtils.CreateExtrude(dir, "0", "DatumHeigth", null, sketch.LeiLine.ToArray()).GetBodies()[0];
                CreateChamfer(body1.Tag);
                SetDatumAttr(body1);
                Body body2 = ExtrudeUtils.CreateExtrude(dir, "DatumHeigth", "extrudePreparation", null, sketch.WaiLine.ToArray()).GetBodies()[0];
                this.DatumBody = BooleanUtils.CreateBooleanFeature(body1, false, false, NXOpen.Features.Feature.BooleanType.Unite, body2).GetBodies()[0];
                return true;
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("创建基准台错误！" + ex.Message);
                return false;
            }


        }
        public bool CreateBuilder()
        {
            if (ParentBuilder != null && !ParentBuilder.IsCreateOk)
                ParentBuilder.CreateBuilder();
            if (!isok && ParentBuilder.IsCreateOk)
                isok = CreateDatum();
            return isok;
        }

        public void SetParentBuilder(IElectrodeBuilder builder)
        {
            this.builder = builder;
        }

        /// <summary>
        /// 创建倒角
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        private Tag CreateChamfer(Tag body)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            Tag[] edgesTag;
            Tag chamferTag = Tag.Null;
            double[] point1 = new double[3];
            double[] point2 = new double[3];
            int connt = 0;
            theUFSession.Modl.AskBodyEdges(body, out edgesTag);
            for (int i = 0; i < edgesTag.Length; i++)
            {
                theUFSession.Modl.AskEdgeVerts(edgesTag[i], point1, point2, out connt);
                if (point1[0] == point2[0] && point1[1] == point2[1] && point1[0] > 0 && point1[1] > 0)
                {
                    Tag[] obj = new Tag[1];
                    obj[0] = edgesTag[i];
                    theUFSession.Modl.CreateChamfer(1, "1.0", "1.0", "45.0", obj, out chamferTag);
                    break;
                }
            }
            return chamferTag;
        }
        /// <summary>
        /// 基准面设定属性
        /// </summary>
        /// <param name="body"></param>
        private void SetDatumAttr(Body body)
        {
            foreach (Face fe in body.GetFaces())
            {
                FaceData data = FaceUtils.AskFaceData(fe);
                if (UMathUtils.IsEqual(data.BoxMinCorner.Z, data.BoxMaxCorner.Z)
                    && UMathUtils.IsEqual(data.BoxMinCorner.Z, this.sketch.LeiLine[0].StartPoint.Z))
                    AttributeUtils.AttributeOperation("DatumFace", "Datum", fe);
            }
        }
    }
}
