using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;
using System.Data;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace MolexPlugin.Model
{
  
    /// <summary>
    /// 矩阵信息
    /// </summary>
    [Serializable]
    public class Matrix4Info : ISetAttribute, ICloneable
    {
        /// <summary>
        /// 坐标中心点
        /// </summary>
        public Point3d CenterPt { get; private set; }
        /// <summary>
        /// 矩阵
        /// </summary>
        public Matrix4 Matr { get; private set; }

        public Matrix4Info(Matrix4 mat)
        {
            this.Matr = mat;
            this.CenterPt = mat.GetCenter();
        }
        /// <summary>
        /// 设置模具信息属性
        /// </summary>
        /// <param name="part"></param>
        public bool SetAttribute(NXObject obj)
        {
            try
            {
                AttributeUtils.AttributeOperation("Matrx4", Matrx4ToString(this.Matr), obj);
                AttributeUtils.AttributeOperation("CsysCenter", new double[] { Math.Round(this.CenterPt.X, 4), Math.Round(this.CenterPt.Y, 4), Math.Round(this.CenterPt.Z, 4) }, obj);
                return true;
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("写入属性错误！" + ex.Message);
                return false;
            }

        }
        /// <summary>
        /// 读取模具属性
        /// </summary>
        /// <param name="part"></param>
        public static Matrix4Info GetAttribute(NXObject obj)
        {
            Matrix4 mat = new Matrix4();
            mat.Identity();
            try
            {             
                string[] temp = new string[4];
                for (int i = 0; i < 4; i++)
                {
                    temp[i] = AttributeUtils.GetAttrForString(obj, "Matrx4", i);
                }
                mat = StringToMatrx4(temp);
                return new Matrix4Info(mat);
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("未获取到属性" + ex.Message);
                return new Matrix4Info(mat);
            }
        }
        public bool SetAttribute(params NXObject[] objs)
        {
            try
            {
                AttributeUtils.AttributeOperation("Matrx4", Matrx4ToString(this.Matr), objs);
                AttributeUtils.AttributeOperation("CsysCenter", new double[] { Math.Round(this.CenterPt.X, 4), Math.Round(this.CenterPt.Y, 4), Math.Round(this.CenterPt.Z, 4) }, objs);
                return true;
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("写入属性错误！" + ex.Message);
                return false;
            }
        }

        public object Clone()
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, this);
            ms.Seek(0, SeekOrigin.Begin);
            return bf.Deserialize(ms);
        }

        /// <summary>
        /// 字符转矩阵
        /// </summary>
        /// <param name="matrString"></param>
        /// <returns></returns>
        protected static Matrix4 StringToMatrx4(string[] matrString)
        {
            double[,] temp = new double[4, 4];
            for (int i = 0; i < 4; i++)
            {
                string[] ch = { "," };
                string[] str = matrString[i].Split(ch, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < 4; j++)
                {
                    temp[i, j] = Convert.ToDouble(str[j]);
                }
            }
            return new Matrix4(temp);
        }
        /// <summary>
        /// 矩阵转字符
        /// </summary>
        /// <param name="matr"></param>
        /// <returns></returns>
        protected static string[] Matrx4ToString(Matrix4 matr)
        {
            string[] temp = new string[4];

            for (int i = 0; i < 4; i++)
            {
                temp[i] = Math.Round(matr.matrix[i, 0], 4).ToString() + "," + Math.Round(matr.matrix[i, 1], 4).ToString() + "," +
                   Math.Round(matr.matrix[i, 2], 4).ToString() + "," + Math.Round(matr.matrix[i, 3], 4).ToString();
            }
            return temp;
        }
    }
}
