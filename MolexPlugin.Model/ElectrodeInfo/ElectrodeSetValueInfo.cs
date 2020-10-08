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
    /// 电极跑位信息
    /// </summary>
    [Serializable]
    public class ElectrodeSetValueInfo : ISetAttribute, ICloneable
    {

        /// <summary>
        /// 电极设定参数
        /// </summary>
        public double[] EleSetValue { get; set; } = new double[3];

        /// <summary>
        /// 接触面积
        /// </summary>
        public double ContactArea { get; set; }
        /// <summary>
        /// 投影面积
        /// </summary>
        public double ProjectedArea { get; set; }
        /// <summary>
        /// 跑位
        /// </summary>
        public string Positioning { get; set; }

        public string PositioningRemark { get; set; } = "";
        /// <summary>
        /// 设置属性
        /// </summary>
        public bool SetAttribute(Part obj)
        {

            try
            {
                AttributeUtils.AttributeOperation("EleSetValue", this.EleSetValue, obj);
                AttributeUtils.AttributeOperation("ContactArea", this.ContactArea, obj);
                AttributeUtils.AttributeOperation("ProjectedArea", this.ProjectedArea, obj);
                AttributeUtils.AttributeOperation("Positioning", this.Positioning, obj);
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
        public static ElectrodeSetValueInfo GetAttribute(NXObject obj)
        {
            ElectrodeSetValueInfo info = new ElectrodeSetValueInfo();
            try
            {
                info.ContactArea = AttributeUtils.GetAttrForDouble(obj, "ContactArea");
                info.ProjectedArea = AttributeUtils.GetAttrForDouble(obj, "ProjectedArea");
                info.Positioning = AttributeUtils.GetAttrForString(obj, "Positioning");
                info.PositioningRemark = AttributeUtils.GetAttrForString(obj, "PositioningRemark");
                for (int i = 0; i < 3; i++)
                {
                    info.EleSetValue[i] = AttributeUtils.GetAttrForDouble(obj, "EleSetValue", i);
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
                AttributeUtils.AttributeOperation("EleSetValue", this.EleSetValue, objs);
                AttributeUtils.AttributeOperation("ContactArea", this.ContactArea, objs);
                AttributeUtils.AttributeOperation("ProjectedArea", this.ProjectedArea, objs);
                AttributeUtils.AttributeOperation("Positioning", this.Positioning, objs);
                AttributeUtils.AttributeOperation("PositioningRemark", this.PositioningRemark, objs);
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
            foreach (PropertyInfo propertyInfo in typeof(ElectrodeSetValueInfo).GetProperties())  //以属性添加列
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
                table.Columns.Add("EleSetValueX", Type.GetType("System.Double"));
                table.Columns.Add("EleSetValueY", Type.GetType("System.Double"));
                table.Columns.Add("EleSetValueZ", Type.GetType("System.Double"));
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
            ElectrodeSetValueInfo info = this.Clone() as ElectrodeSetValueInfo;
            foreach (PropertyInfo propertyInfo in typeof(ElectrodeSetValueInfo).GetProperties())
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
                row["EleSetValueX"] = info.EleSetValue[0];
                row["EleSetValueY"] = info.EleSetValue[1];
                row["EleSetValueZ"] = info.EleSetValue[2];
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
        public static ElectrodeSetValueInfo GetInfoForDataRow(DataRow row)
        {
            ElectrodeSetValueInfo info = new ElectrodeSetValueInfo();
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
                info.EleSetValue[0] = Convert.ToDouble(row["EleSetValueX"]);
                info.EleSetValue[1] = Convert.ToDouble(row["EleSetValueY"]);
                info.EleSetValue[2] = Convert.ToDouble(row["EleSetValueZ"]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return info;
        }
    }
}

