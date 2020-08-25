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
    /// 电极间隙
    /// </summary>
    [Serializable]
    public class ElectrodeGapValueInfo : ISetAttribute, ICloneable
    {
        /// <summary>
        /// 粗放
        /// </summary>
        public double CrudeInter { get; set; } = 0;
        /// <summary>
        /// 粗放个数
        /// </summary>
        public int CrudeNum { get; set; } = 0;
        /// <summary>
        /// 中放
        /// </summary>
        public double DuringInter { get; set; } = 0;
        /// <summary>
        /// 中放个数
        /// </summary>
        public int DuringNum { get; set; } = 0;
        /// <summary>
        /// 精放
        /// </summary>
        public double FineInter { get; set; } = 0;
        /// <summary>
        /// 精放个数
        /// </summary>
        public int FineNum { get; set; } = 0;

        /// <summary>
        /// 设置属性
        /// </summary>
        public bool SetAttribute(Part obj)
        {
            try
            {
                AttributeUtils.AttributeOperation("CrudeInter", this.CrudeInter, obj);
                AttributeUtils.AttributeOperation("CrudeNum", this.CrudeNum, obj);
                AttributeUtils.AttributeOperation("DuringInter", this.DuringInter, obj);
                AttributeUtils.AttributeOperation("DuringNum", this.DuringNum, obj);
                AttributeUtils.AttributeOperation("FineInter", this.FineInter, obj);
                AttributeUtils.AttributeOperation("FineNum", this.FineNum, obj);
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
        public static ElectrodeGapValueInfo GetAttribute(NXObject obj)
        {
            ElectrodeGapValueInfo info = new ElectrodeGapValueInfo();
            try
            {
                info.CrudeInter = AttributeUtils.GetAttrForDouble(obj, "CrudeInter");
                info.CrudeNum = AttributeUtils.GetAttrForInt(obj, "CrudeNum");
                info.DuringInter = AttributeUtils.GetAttrForDouble(obj, "DuringInter");
                info.DuringNum = AttributeUtils.GetAttrForInt(obj, "DuringNum");
                info.FineInter = AttributeUtils.GetAttrForDouble(obj, "FineInter");
                info.FineNum = AttributeUtils.GetAttrForInt(obj, "FineNum");
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
                AttributeUtils.AttributeOperation("CrudeInter", this.CrudeInter, objs);
                AttributeUtils.AttributeOperation("CrudeNum", this.CrudeNum, objs);
                AttributeUtils.AttributeOperation("DuringInter", this.DuringInter, objs);
                AttributeUtils.AttributeOperation("DuringNum", this.DuringNum, objs);
                AttributeUtils.AttributeOperation("FineInter", this.FineInter, objs);
                AttributeUtils.AttributeOperation("FineNum", this.FineNum, objs);
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
