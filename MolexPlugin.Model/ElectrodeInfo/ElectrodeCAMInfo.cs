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
    public class ElectrodeCAMInfo : ISetAttribute,ICloneable
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
        /// 设置属性
        /// </summary>
        public bool SetAttribute(Part obj)
        {
           
            try
            {
                AttributeUtils.AttributeOperation("EleHeadDis", this.EleHeadDis, obj);
                AttributeUtils.AttributeOperation("EleMinDim", this.EleMinDim, obj);
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
                for (int i = 0; i < 2; i++)
                {
                    info.EleHeadDis[i] = AttributeUtils.GetAttrForDouble(obj, "EleHeadDis", i);
                }
                return info;
            }
            catch(NXException ex)
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

