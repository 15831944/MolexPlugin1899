using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 面斜率和半径工厂
    /// </summary>
    public class FaceSlopeAndDiaFactory
    {
        public static AbstractFaceSlopeAndDia CreateFaceSlopeAndDia(Face face)
        {
            FaceData data = FaceUtils.AskFaceData(face);
            AbstractFaceSlopeAndDia faceSd = null;
            switch (face.SolidFaceType)
            {
                case Face.FaceType.Planar:
                    faceSd = new PlaneFaceSlopeAndDia(data);
                    break;
                case Face.FaceType.Cylindrical:
                    faceSd = new CylinderFaceSlopeAndDia(data);
                    break;
                case Face.FaceType.Conical:
                    faceSd = new CircularConeFaceSlopeAndDia(data);
                    break;
                default:
                    faceSd = new SweptFaceFaceSlopeAndDia(data);
                    break;
            }
            return faceSd;

        }
    }
}
