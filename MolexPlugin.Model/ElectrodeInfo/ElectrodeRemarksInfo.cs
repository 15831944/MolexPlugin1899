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
    /// 电极不动属性信息
    /// </summary>
    [Serializable]
    public class ElectrodeRemarksInfo : ISetAttribute, ICloneable
    {

        /// <summary>
        /// 电极类型
        /// </summary>
        public string EleType { get; set; }
        /// <summary>
        /// 放电条件
        /// </summary>
        public string Condition { get; set; }

        /// <summary>
        /// CH值
        /// </summary>
        public string Ch { get; set; }

        /// <summary>
        /// 电极描述
        /// </summary>
        public string ElePresentation { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 工艺
        /// </summary>
        public string Technology { get; set; }


        /// <summary>
        /// 设置属性
        /// </summary>
        public bool SetAttribute(NXObject obj)
        {
            try
            {
                AttributeUtils.AttributeOperation("EleType", this.EleType, obj);
                AttributeUtils.AttributeOperation("Condition", this.Condition, obj);
                AttributeUtils.AttributeOperation("CH", this.Ch, obj);
                AttributeUtils.AttributeOperation("Remarks", this.Remarks, obj);
                AttributeUtils.AttributeOperation("Technology", this.Technology, obj);
                AttributeUtils.AttributeOperation("ElePresentation", this.ElePresentation, obj);
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
        public static ElectrodeRemarksInfo GetAttribute(NXObject obj)
        {
            ElectrodeRemarksInfo info = new ElectrodeRemarksInfo();
            try
            {
                info.EleType = AttributeUtils.GetAttrForString(obj, "EleType");
                info.Condition = AttributeUtils.GetAttrForString(obj, "Condition");
                info.Ch = AttributeUtils.GetAttrForString(obj, "CH");
                info.Remarks = AttributeUtils.GetAttrForString(obj, "Remarks");
                info.Technology = AttributeUtils.GetAttrForString(obj, "Technology");
                info.ElePresentation = AttributeUtils.GetAttrForString(obj, "ElePresentation");
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
                AttributeUtils.AttributeOperation("EleType", this.EleType, objs);
                AttributeUtils.AttributeOperation("Condition", this.Condition, objs);
                AttributeUtils.AttributeOperation("CH", this.Ch, objs);
                AttributeUtils.AttributeOperation("Remarks", this.Remarks, objs);
                AttributeUtils.AttributeOperation("Technology", this.Technology, objs);
                AttributeUtils.AttributeOperation("ElePresentation", this.ElePresentation, objs);


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

            foreach (PropertyInfo propertyInfo in typeof(ElectrodeRemarksInfo).GetProperties())  //以属性添加列
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
            ElectrodeRemarksInfo info = this.Clone() as ElectrodeRemarksInfo;
            foreach (PropertyInfo propertyInfo in typeof(ElectrodeRemarksInfo).GetProperties())
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
        public static ElectrodeRemarksInfo GetInfoForDataRow(DataRow row)
        {
            ElectrodeRemarksInfo info = new ElectrodeRemarksInfo();
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
    }
}

