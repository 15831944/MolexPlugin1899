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
    }
}