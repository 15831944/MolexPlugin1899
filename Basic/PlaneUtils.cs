using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace Basic
{
    public class PlaneUtils : ClassItem
    {
        /// <summary>
        /// 以面中心点和向量创建平面
        /// </summary>
        /// <param name="planeFace"></param>
        /// <param name="flip">设置是否翻转方向</param>
        /// <returns></returns>
        public static NXOpen.Plane CreatePlaneOfFace(Face face, bool flip)
        {
            Part workPart = theSession.Parts.Work;
            Point3d originPt;
            Vector3d normal;
            FaceUtils.AskFaceOriginAndNormal(face, out originPt, out normal);
            try
            {
                NXOpen.Plane plane1 = workPart.Planes.CreatePlane(originPt, normal, NXOpen.SmartObject.UpdateOption.WithinModeling);
                plane1.SetFlip(flip);
                return plane1;
            }
            catch (NXException ex)
            {
                LogMgr.WriteLog("Basic.TrimBody:CreateTrimBodyFeature:" + ex.Message);
                throw ex;
            }
        }



    }
}
