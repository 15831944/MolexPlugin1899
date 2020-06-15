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
    /// 圆形面类型
    /// </summary>
    public class AbstractCircleFace:IComparable<AbstractCircleFace>
    {
        public FaceData Data { get; private set; }
        /// <summary>
        /// 方向
        /// </summary>
        public Vector3d Direction { get; protected set; }

        public List<ArcEdgeData> ArcEdge { get; private set; } = new List<ArcEdgeData>();

        
        public AbstractCircleFace(FaceData data)
        {
            this.Data = data;        
            string err = "";
            foreach (Edge ed in data.Face.GetEdges())
            {
                if (ed.SolidEdgeType == Edge.EdgeType.Circular)
                {
                    ArcEdgeData ard = EdgeUtils.GetArcData(ed, ref err);
                    ArcEdge.Add(ard);
                }
                   
            }
        }

        public int CompareTo(AbstractCircleFace other)
        {
            Matrix4 mat = new Matrix4();
            mat.TransformToZAxis(new Point3d(0, 0, 0), this.Direction);
        }
    }
}
