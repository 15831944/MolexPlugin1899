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
    /// 电极CAM信息
    /// </summary>
    [Serializable]
    public class ElectrodeCAMInfo : ISetAttribute, ICloneable
    {

        /// <summary>
        /// 电极单齿外形
        /// </summary>
        public double[] EleHeadDis { get; set; } = new double[2] { 0, 0 };
        /// <summary>
        /// 电极单齿最小距离
        /// </summary>
        public double EleMinDim { get; set; } = 9999;

        /// <summary>
        /// 加工模板
        /// </summary>
        public string CamTemplate { get; set; }
        /// <summary>
        /// 设置属性
        /// </summary>
        public bool SetAttribute(Part obj)
        {

            try
            {
                AttributeUtils.AttributeOperation("EleHeadDis", this.EleHeadDis, obj);
                AttributeUtils.AttributeOperation("EleMinDim", this.EleMinDim, obj);
                AttributeUtils.AttributeOperation("CamTemplate", this.CamTemplate, obj);
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
        public static ElectrodeCAMInfo GetAttribute(NXObject obj)
        {
            ElectrodeCAMInfo info = new ElectrodeCAMInfo();
            try
            {
                info.EleMinDim = AttributeUtils.GetAttrForDouble(obj, "EleMinDim");
                info.CamTemplate = AttributeUtils.GetAttrForString(obj, "CamTemplate");
                for (int i = 0; i < 2; i++)
                {
                    info.EleHeadDis[i] = AttributeUtils.GetAttrForDouble(obj, "EleHeadDis", i);
                }
                return info;
            }
            catch (NXException ex)
            {
                throw ex;
            }

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
                AttributeUtils.AttributeOperation("EleHeadDis", this.EleHeadDis, objs);
                AttributeUtils.AttributeOperation("EleMinDim", this.EleMinDim, objs);
                AttributeUtils.AttributeOperation("CamTemplate", this.CamTemplate, objs);
                return true;
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("写入属性错误！" + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, this);
            ms.Seek(0, SeekOrigin.Begin);
            return bf.Deserialize(ms);
        }

        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="table"></param>   
        public static void CreateDataTable(ref DataTable table)
        {
            foreach (PropertyInfo propertyInfo in typeof(ElectrodeCAMInfo).GetProperties())  //以属性添加列
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
            try
            {
                table.Columns.Add("EleHeadDis-X", Type.GetType("System.Double"));
                table.Columns.Add("EleHeadDis-Y", Type.GetType("System.Double"));

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        /// <summary>
        ///创建行
        /// </summary>
        /// <param name="row"></param>
        public void CreateDataRow(ref DataRow row)
        {
            ElectrodeCAMInfo info = this.Clone() as ElectrodeCAMInfo;
            foreach (PropertyInfo propertyInfo in typeof(ElectrodeCAMInfo).GetProperties())
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
            try
            {
                row["EleHeadDis-X"] = info.EleHeadDis[0];
                row["EleHeadDis-Y"] = info.EleHeadDis[1];

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 通过行获取数据
        /// </summary>
        /// <param name="row"></param>
        public static ElectrodeCAMInfo GetInfoForDataRow(DataRow row)
        {
            ElectrodeCAMInfo info = new ElectrodeCAMInfo();
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
            try
            {
                info.EleHeadDis[0] = Convert.ToInt32(row["EleHeadDis-X"]);
                info.EleHeadDis[1] = Convert.ToInt32(row["EleHeadDis-Y"]);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return info;
        }
    }
}

