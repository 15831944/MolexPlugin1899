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
    }
}

