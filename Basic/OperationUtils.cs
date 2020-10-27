using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.CAM;


namespace Basic
{
    public class OperationUtils
    {
        /// <summary>
        /// 设置平面铣边界(以绝对点)
        /// </summary>
        /// <param name="toolside">刀具侧</param>
        /// <param name="types"></param>
        /// <param name="pt">封闭还是开放</param>
        /// <param name="boundary">边界</param>
        /// <param name="edges">边</param>
        public static BoundarySetPlanarMill CreateBoundaryPlanarMill(BoundarySet.ToolSideTypes toolside, BoundarySet.BoundaryTypes types,
            Point3d pt, Boundary boundary, params NXObject[] edges)
        {
            if (boundary == null)
                throw new Exception("输入边界为空！");
            if (edges.Length == 0)
                throw new Exception("输入边为空！");
            Matrix4 mat = new Matrix4();
            mat.Identity();
            Part workPart = Session.GetSession().Parts.Work;
            BoundarySetPlanarMill boundarySetPlanarMill;
            boundarySetPlanarMill = boundary.CreateBoundarySetPlanarMill();
            Vector3d normal = new NXOpen.Vector3d(0.0, 0.0, 1.0);
            Plane plane = workPart.Planes.CreatePlane(pt, normal, NXOpen.SmartObject.UpdateOption.AfterModeling);
            boundarySetPlanarMill.ToolSide = toolside;
            boundarySetPlanarMill.AppendCurves(edges, pt, mat.GetMatrix3());
            boundarySetPlanarMill.BoundaryType = types;
            boundarySetPlanarMill.Plane = plane;
            boundarySetPlanarMill.PlaneType = NXOpen.CAM.BoundarySet.PlaneTypes.UserDefined;
            return boundarySetPlanarMill;
        }
        /// <summary>
        /// 设置面铣边界（以绝对点）
        /// </summary>
        /// <param name="toolside">刀具侧</param>
        /// <param name="types"></param>
        /// <param name="pt">封闭还是开放</param>
        /// <param name="boundary">边界</param>
        /// <param name="edges">边</param>
        public static BoundaryMillingSet CreateBoundaryMillingSet(BoundarySet.ToolSideTypes toolside, BoundarySet.BoundaryTypes types,
            Point3d pt, Boundary boundary, params NXObject[] edges)
        {
            if (boundary == null)
                throw new Exception("输入边界为空！");
            if (edges.Length == 0)
                throw new Exception("输入边为空！");
            Matrix4 mat = new Matrix4();
            mat.Identity();
            Part workPart = Session.GetSession().Parts.Work;
            BoundaryMillingSet boundarySet;
            boundarySet = boundary.CreateBoundaryMillingSet();
            boundarySet.ToolSide = toolside;
            boundarySet.PlaneType = NXOpen.CAM.BoundarySet.PlaneTypes.UserDefined;
            boundarySet.AppendCurves(edges, pt, mat.GetMatrix3());
            boundarySet.BoundaryType = types;
            return boundarySet;
        }

    }
}
