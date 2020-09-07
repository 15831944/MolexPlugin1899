﻿using System;
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
    ///电极基准台信息
    /// </summary>
    [Serializable]
    public class ElectrodeDatumInfo : ISetAttribute, ICloneable
    {
        /// <summary>
        /// 基准台宽度
        /// </summary>
        public double DatumWidth { get; set; }
        /// <summary>
        /// 基准台高度
        /// </summary>
        public double DatumHeigth { get; set; }
        /// <summary>
        /// 拉升值
        /// </summary>
        public double ExtrudeHeight { get; set; }
        /// <summary>
        /// 电极高度
        /// </summary>
        public double EleHeight { get; set; }
        /// <summary>
        /// 设置属性
        /// </summary>
        public bool SetAttribute(Part obj)
        {
            try
            {
                AttributeUtils.AttributeOperation("DatumWidth", this.DatumWidth, obj);
                AttributeUtils.AttributeOperation("DatumHeigth", this.DatumHeigth, obj);
                AttributeUtils.AttributeOperation("Extrudewith", this.ExtrudeHeight, obj);
                AttributeUtils.AttributeOperation("EleHeight", this.EleHeight, obj);
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
        public static ElectrodeDatumInfo GetAttribute(NXObject obj)
        {
            ElectrodeDatumInfo info = new ElectrodeDatumInfo();
            try
            {
                info.DatumWidth = AttributeUtils.GetAttrForDouble(obj, "DatumWidth");
                info.DatumHeigth = AttributeUtils.GetAttrForDouble(obj, "DatumHeigth");
                info.ExtrudeHeight = AttributeUtils.GetAttrForDouble(obj, "Extrudewith");
                info.EleHeight = AttributeUtils.GetAttrForDouble(obj, "EleHeight");
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
                AttributeUtils.AttributeOperation("DatumWidth", this.DatumWidth, objs);
                AttributeUtils.AttributeOperation("DatumHeigth", this.DatumHeigth, objs);
                AttributeUtils.AttributeOperation("Extrudewith", this.ExtrudeHeight, objs);
                AttributeUtils.AttributeOperation("EleHeight", this.EleHeight, objs);
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
