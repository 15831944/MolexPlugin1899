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
    /// 电极PH
    /// </summary>
    [Serializable]
    public class ElectrodePitchInfo : ISetAttribute, ICloneable
    {
        /// <summary>
        /// X向PH
        /// </summary>
        public double PitchX { get; set; } = 0;
        /// <summary>
        /// X向个数
        /// </summary>
        public int PitchXNum { get; set; } = 0;
        /// <summary>
        /// Y向PH
        /// </summary>
        public double PitchY { get; set; } = 0;
        /// <summary>
        /// Y向个数
        /// </summary>
        public int PitchYNum { get; set; } = 0;
        /// <summary>
        /// 设置属性
        /// </summary>
        public bool SetAttribute(Part obj)
        {
            try
            {
                AttributeUtils.AttributeOperation("PitchX", this.PitchX, obj);
                AttributeUtils.AttributeOperation("PitchXNum", this.PitchXNum, obj);
                AttributeUtils.AttributeOperation("PitchY", this.PitchY, obj);
                AttributeUtils.AttributeOperation("PitchYNum", this.PitchYNum, obj);
                return true;
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("写入属性错误！" + ex.Message);
                return false;
            }


        }
        /// <summary>
        /// 读取属性
        /// </summary>
        public static ElectrodePitchInfo GetAttribute(NXObject obj)
        {
            ElectrodePitchInfo info = new ElectrodePitchInfo();
            try
            {
                info.PitchX = AttributeUtils.GetAttrForDouble(obj, "PitchX");
                info.PitchXNum = AttributeUtils.GetAttrForInt(obj, "PitchXNum");
                info.PitchY = AttributeUtils.GetAttrForDouble(obj, "PitchY");
                info.PitchYNum = AttributeUtils.GetAttrForInt(obj, "PitchYNum");

                return info;
            }
            catch (NXException ex)
            {
                throw ex;
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
        /// 写入属性
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        public bool SetAttribute(params NXObject[] objs)
        {
            try
            {
                AttributeUtils.AttributeOperation("PitchX", this.PitchX, objs);
                AttributeUtils.AttributeOperation("PitchXNum", this.PitchXNum, objs);
                AttributeUtils.AttributeOperation("PitchY", this.PitchY, objs);
                AttributeUtils.AttributeOperation("PitchYNum", this.PitchYNum, objs);
                return true;
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("写入属性错误！" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 通过属性获取
        /// </summary>
        /// <param name="bodys">一种齿（阵列）</param>
        /// <returns></returns>
        public ElectrodeToolhInfo[,] GetToolhInfosForAttribute(List<Body> bodys, Matrix4 matr, CartesianCoordinateSystem csys)
        {
            ElectrodeToolhInfo[,] info = new ElectrodeToolhInfo[this.PitchXNum, this.PitchYNum];
            var toolhNumList = bodys.GroupBy(a => AttributeUtils.GetAttrForInt(a, "ToolhNumber"));
            List<BodyPitchClassify> bps = new List<BodyPitchClassify>();
            foreach (var toolhNum in toolhNumList)
            {
                BodyPitchClassify bp = new BodyPitchClassify(toolhNum.ToList(), matr, csys, this.PitchXNum, this.PitchYNum);
                bp.SetAttribute();
                bps.Add(bp);
            }
            for (int i = 0; i < this.PitchXNum; i++)
            {
                for (int k = 0; k < this.PitchYNum; k++)
                {
                    List<Body> temp = new List<Body>();
                    foreach (BodyPitchClassify by in bps)
                    {
                        temp.Add(by.ClassifyBodys[i, k]);
                    }
                    info[i, k] = ElectrodeToolhInfo.GetToolhInfoForAttribute(temp.ToArray());
                }
            }
            return info;
        }

        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="table"></param>   
        public static void CreateDataTable(ref DataTable table)
        {

            foreach (PropertyInfo propertyInfo in typeof(ElectrodePitchInfo).GetProperties())  //以属性添加列
            {
                try
                {
                    table.Columns.Add(new DataColumn(propertyInfo.Name, propertyInfo.PropertyType));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
        /// <summary>
        ///创建行
        /// </summary>
        /// <param name="row"></param>
        public void CreateDataRow(ref DataRow row)
        {
            ElectrodePitchInfo info = this.Clone() as ElectrodePitchInfo;
            foreach (PropertyInfo propertyInfo in typeof(ElectrodePitchInfo).GetProperties())
            {
                try
                {
                    row[propertyInfo.Name] = propertyInfo.GetValue(info, null);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        /// <summary>
        /// 通过行获取数据
        /// </summary>
        /// <param name="row"></param>
        public static ElectrodePitchInfo GetInfoForDataRow(DataRow row)
        {
            ElectrodePitchInfo info = new ElectrodePitchInfo();
            for (int i = 0; i < row.Table.Columns.Count; i++)
            {
                try
                {
                    PropertyInfo propertyInfo = info.GetType().GetProperty(row.Table.Columns[i].ColumnName);
                    if (propertyInfo != null && row[i] != DBNull.Value)
                        propertyInfo.SetValue(info, row[i], null);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return info;
        }
        /// <summary>
        /// 比较是否修改电极
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsEquals(ElectrodePitchInfo other)
        {
            return this.PitchX == other.PitchX && this.PitchXNum == other.PitchXNum &&
                this.PitchY == other.PitchY && this.PitchYNum == other.PitchYNum;


        }
    }
}
