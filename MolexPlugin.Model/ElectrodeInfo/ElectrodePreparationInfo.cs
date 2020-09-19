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
    /// 电极备料
    /// </summary>
    [Serializable]
    public class ElectrodePreparationInfo : ISetAttribute, ICloneable
    {
        /// <summary>
        /// 备料值
        /// </summary>
        public int[] Preparation { get; set; } = new int[3];
        /// <summary>
        /// 标准料
        /// </summary>
        public bool IsPreparation { get; set; }
        /// <summary>
        /// 材料
        /// </summary>
        public string Material { get; set; }
        /// <summary>
        /// 设置属性
        /// </summary>
        public bool SetAttribute(Part obj)
        {
            try
            {
                AttributeUtils.AttributeOperation("IsPreparation", this.IsPreparation, obj);
                AttributeUtils.AttributeOperation("Material1", this.Material, obj);
                AttributeUtils.AttributeOperation("Preparation", this.Preparation, obj);
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
        public static ElectrodePreparationInfo GetAttribute(NXObject obj)
        {
            ElectrodePreparationInfo info = new ElectrodePreparationInfo();
            try
            {
                info.IsPreparation = AttributeUtils.GetAttrForBool(obj, "IsPreparation");
                info.Material = AttributeUtils.GetAttrForString(obj, "Material1");
                for (int i = 0; i < 3; i++)
                {
                    info.Preparation[i] = AttributeUtils.GetAttrForInt(obj, "Preparation", i);
                }
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
                AttributeUtils.AttributeOperation("IsPreparation", this.IsPreparation, objs);
                AttributeUtils.AttributeOperation("Material1", this.Material, objs);
                AttributeUtils.AttributeOperation("Preparation", this.Preparation, objs);
                return true;
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("写入属性错误！" + ex.Message);
                return false;
            }

        }

        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="table"></param>   
        public static void CreateDataTable(ref DataTable table)
        {
            foreach (PropertyInfo propertyInfo in typeof(ElectrodePreparationInfo).GetProperties())  //以属性添加列
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
                table.Columns.Add("PreparationX", Type.GetType("System.Int32"));
                table.Columns.Add("PreparationY", Type.GetType("System.Int32"));
                table.Columns.Add("PreparationZ", Type.GetType("System.Int32"));
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
            ElectrodePreparationInfo info = this.Clone() as ElectrodePreparationInfo;
            foreach (PropertyInfo propertyInfo in typeof(ElectrodePreparationInfo).GetProperties())
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
                row["PreparationX"] = info.Preparation[0];
                row["PreparationY"] = info.Preparation[1];
                row["PreparationZ"] = info.Preparation[2];
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
        public static ElectrodePreparationInfo GetInfoForDataRow(DataRow row)
        {
            ElectrodePreparationInfo info = new ElectrodePreparationInfo();
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
                info.Preparation[0] = Convert.ToInt32(row["PreparationX"]);
                info.Preparation[1] = Convert.ToInt32(row["PreparationY"]);
                info.Preparation[2] = Convert.ToInt32(row["PreparationZ"]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return info;
        }

        /// <summary>
        /// 比较是否修改电极
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsEquals(ElectrodePreparationInfo other)
        {
            return this.Preparation[0] == other.Preparation[0] && this.Preparation[1] == other.Preparation[1] && this.Preparation[2] == other.Preparation[2];
        }
    }
}
