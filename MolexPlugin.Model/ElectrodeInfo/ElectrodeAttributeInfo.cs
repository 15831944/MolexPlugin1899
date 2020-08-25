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
    public class ElectrodeAttributeInfo : ISetAttribute,ICloneable
    {
        /// <summary>
        /// 电极名
        /// </summary>
        public string EleName { get; set; }      
        /// <summary>
        /// 电极类型
        /// </summary>
        public string EleType { get; set; }
        /// <summary>
        /// 放电条件
        /// </summary>
        public string Condition { get; set; }
        /// <summary>
        /// 拉升值
        /// </summary>
        public double Extrudewith { get; set; }
        /// <summary>
        /// CH值
        /// </summary>
        public string Ch { get; set; }
        /// <summary>
        /// 借用电极
        /// </summary>
        public string BorrowName { get; set; }
        /// <summary>
        /// 电极设定参数
        /// </summary>
        public double[] EleSetValue { get; set; } = new double[3]; 
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
        /// 加工模板
        /// </summary>
        public string CamTemplate { get; set; }
        /// <summary>
        /// 放电面积
        /// </summary>
        public double Area { get; set; }
        /// <summary>
        /// 电极编号
        /// </summary>
        public int EleNumber { get; set; }
        /// <summary>
        /// 跑位
        /// </summary>
        public string Positioning { get; set; }
        /// <summary>
        /// /Z向基准
        /// </summary>
        public bool ZDatum { get; set; }
        /// <summary>
        /// 设置属性
        /// </summary>
        public bool SetAttribute(NXObject obj)
        {
            try
            {

                AttributeUtils.AttributeOperation("EleName", this.EleName, obj);
                AttributeUtils.AttributeOperation("BorrowName", this.BorrowName, obj);
                AttributeUtils.AttributeOperation("EleType", this.EleType, obj);
                AttributeUtils.AttributeOperation("Condition", this.Condition, obj);
                AttributeUtils.AttributeOperation("Extrudewith", this.Extrudewith, obj);
                AttributeUtils.AttributeOperation("CH", this.Ch, obj);
                AttributeUtils.AttributeOperation("Remarks", this.Remarks, obj);
                AttributeUtils.AttributeOperation("Technology", this.Technology, obj);
                AttributeUtils.AttributeOperation("CamTemplate", this.CamTemplate, obj);
                AttributeUtils.AttributeOperation("EleSetValue", this.EleSetValue, obj);
                AttributeUtils.AttributeOperation("ElePresentation", this.ElePresentation, obj);
                AttributeUtils.AttributeOperation("Area", this.Area, obj);
                AttributeUtils.AttributeOperation("EleNumber", this.EleNumber, obj);
                AttributeUtils.AttributeOperation("Positioning", this.Positioning, obj);
                return true;
            }
            catch(NXException ex)
            {
                ClassItem.WriteLogFile("写入属性错误！" + ex.Message);
                return false;
            }
       
        }
        /// <summary>
        /// 读取属性
        /// </summary>
        public static ElectrodeAttributeInfo GetAttribute(NXObject obj)
        {
            ElectrodeAttributeInfo info = new ElectrodeAttributeInfo();
            try
            {
                info.EleName = AttributeUtils.GetAttrForString(obj, "EleName");
                info.BorrowName = AttributeUtils.GetAttrForString(obj, "BorrowName");
                info.EleType = AttributeUtils.GetAttrForString(obj, "EleType");
                info.Condition = AttributeUtils.GetAttrForString(obj, "Condition");
                info.Extrudewith = AttributeUtils.GetAttrForDouble(obj, "Extrudewith");
                info.Ch = AttributeUtils.GetAttrForString(obj, "CH");
                info.Remarks = AttributeUtils.GetAttrForString(obj, "Remarks");
                info.Technology = AttributeUtils.GetAttrForString(obj, "Technology");
                info.CamTemplate = AttributeUtils.GetAttrForString(obj, "CamTemplate");
                for (int i = 0; i < 3; i++)
                {
                    info.EleSetValue[i] = AttributeUtils.GetAttrForDouble(obj, "EleSetValue", i);
                }
                info.Positioning = AttributeUtils.GetAttrForString(obj, "Positioning");
                info.ElePresentation = AttributeUtils.GetAttrForString(obj, "ElePresentation");
                info.Area = AttributeUtils.GetAttrForDouble(obj, "Area");
                info.EleNumber = AttributeUtils.GetAttrForInt(obj, "EleNumber");
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
                AttributeUtils.AttributeOperation("EleName", this.EleName, objs);
                AttributeUtils.AttributeOperation("BorrowName", this.BorrowName, objs);               
                AttributeUtils.AttributeOperation("EleType", this.EleType, objs);
                AttributeUtils.AttributeOperation("Condition", this.Condition, objs);
                AttributeUtils.AttributeOperation("Extrudewith", this.Extrudewith, objs);
                AttributeUtils.AttributeOperation("CH", this.Ch, objs);           
                AttributeUtils.AttributeOperation("Remarks", this.Remarks, objs);
                AttributeUtils.AttributeOperation("Technology", this.Technology, objs);
                AttributeUtils.AttributeOperation("CamTemplate", this.CamTemplate, objs);
                AttributeUtils.AttributeOperation("EleSetValue", this.EleSetValue, objs);            
                AttributeUtils.AttributeOperation("ElePresentation", this.ElePresentation, objs);
                AttributeUtils.AttributeOperation("Area", this.Area, objs);
                AttributeUtils.AttributeOperation("EleNumber", this.EleNumber, objs);
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

