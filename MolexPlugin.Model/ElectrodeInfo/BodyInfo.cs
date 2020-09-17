using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;


namespace MolexPlugin.Model
{
    /// <summary>
    /// 电极单体信息
    /// </summary>
    public class BodyInfo
    {
        double cArea = 0;
        /// <summary>
        /// 实体
        /// </summary>
        public Body Body { get; private set; }
        int[] pitchNum = null;
        string name = "";

        /// <summary>
        /// 放电面
        /// </summary>
        public List<Face> DischargeFace { get; private set; } = new List<Face>();
        /// <summary>
        /// 接触面积
        /// </summary>
        public double ContactArea
        {
            get
            {
                if (cArea == 0)
                {
                    cArea = GetContactArea();
                }
                return cArea;
            }
        }
        /// <summary>
        /// 体在电极齿的号
        /// </summary>
        public int Number
        {
            get
            {
                try
                {
                    return AttributeUtils.GetAttrForInt(this.Body, "ToolhNumber");
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                try
                {
                    AttributeUtils.AttributeOperation("ToolhNumber", value, this.Body);
                }
                catch
                {

                }
            }
        }

        public bool ER
        {
            get
            {
                if (name == "")
                    name = AttributeUtils.GetAttrForString(this.Body, "ToolhGapValue");
                if (name.Equals("ER", StringComparison.CurrentCultureIgnoreCase))
                    return true;
                else
                    return false;
            }
            set
            {
                if (value)
                {
                    AttributeUtils.AttributeOperation("ToolhGapValue", "ER", this.Body);
                    AttributeUtils.AttributeOperation("ToolhGapValue", "ER", this.Body.GetFaces());
                }

            }
        }

        public bool EF
        {
            get
            {
                if (name == "")
                    name = AttributeUtils.GetAttrForString(this.Body, "ToolhGapValue");
                if (name.Equals("EF", StringComparison.CurrentCultureIgnoreCase))
                    return true;
                else
                    return false;
            }
            set
            {
                if (value)
                {
                    AttributeUtils.AttributeOperation("ToolhGapValue", "EF", this.Body);
                    AttributeUtils.AttributeOperation("ToolhGapValue", "EF", this.Body.GetFaces());
                }

            }
        }
        /// <summary>
        /// 在PH 上的位置
        /// </summary>
        public int[] PitchNum
        {
            get
            {
                if (pitchNum == null)
                    for (int i = 0; i < 2; i++)
                    {
                        pitchNum[i] = AttributeUtils.GetAttrForInt(this.Body, "PitchNum", i);
                    }
                return pitchNum;
            }
        }
        public BodyInfo(Body body, List<Face> face)
        {
            this.Body = body;
            this.DischargeFace = face;
        }
        private BodyInfo(Body body)
        {
            this.Body = body;
            this.DischargeFace = GetDischargeFaceAttr();
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        public bool SetAttribute(CartesianCoordinateSystem csys, Matrix4 matr)
        {

            try
            {
                AttributeUtils.AttributeOperation("ContactArea", this.ContactArea, this.Body);
                SetDischargeFaceAttr();
                return true;
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("写入属性错误！" + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="body"></param>
        /// <param name="mat"></param>
        /// <param name="csys"></param>
        /// <returns></returns>
        public static BodyInfo GetAttribute(Body body)
        {
            BodyInfo info = new BodyInfo(body);
            return info;
        }
        /// <summary>
        /// 放电面加入属性
        /// </summary>
        /// <returns></returns>
        private bool SetDischargeFaceAttr()
        {
            bool isok = true;
            foreach (Face fe in DischargeFace)
            {
                try
                {
                    AttributeUtils.AttributeOperation("DischargeFace", "DF", fe);
                }
                catch
                {
                    isok = false;
                }
            }
            return isok;
        }
        /// <summary>
        /// 放电面加入属性
        /// </summary>
        /// <returns></returns>
        private List<Face> GetDischargeFaceAttr()
        {
            List<Face> dis = new List<Face>();
            foreach (Face fe in this.Body.GetFaces())
            {
                try
                {
                    string tem = AttributeUtils.GetAttrForString(fe, "DischargeFace");
                    if (tem.Equals("DF", StringComparison.CurrentCultureIgnoreCase))
                        dis.Add(fe);
                }
                catch
                {

                }
            }
            return dis;
        }
        /// <summary>
        /// 获取接触面积
        /// </summary>
        /// <returns></returns>
        private double GetContactArea()
        {
            if (this.DischargeFace.Count == 0)
                return 0;
            return FaceUtils.GetFaceArea(this.DischargeFace.ToArray());
        }
        /// <summary>
        /// 获取投影面积
        /// </summary>
        /// <returns></returns>
        public double GetProjectedArea(CartesianCoordinateSystem csys, Matrix4 matr)
        {
            if (this.DischargeFace.Count == 0)
                return 0;
            Point3d center = new Point3d();
            Point3d dis = new Point3d();
            BoundingBoxUtils.GetBoundingBoxInLocal(this.DischargeFace.ToArray(), csys, matr, ref center, ref dis);
            return 2 * (dis.X * dis.Y);
        }

        public static bool IsContactArea(Body by)
        {
            double contact = AttributeUtils.GetAttrForDouble(by, "ContactArea");
            if (contact != 0)
                return true;
            else
                return false;
        }
    }
}

