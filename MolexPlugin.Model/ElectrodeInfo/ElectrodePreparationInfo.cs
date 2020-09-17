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
    }
}
