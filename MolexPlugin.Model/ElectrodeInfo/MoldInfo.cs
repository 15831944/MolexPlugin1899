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
    /// 模具信息
    /// </summary>
    [Serializable]
    public class MoldInfo : ISetAttribute, ICloneable, IEquatable<MoldInfo>
    {
        /// <summary>
        /// 模号
        /// </summary>
        public string MoldNumber { get; set; }
        /// <summary>
        /// 件号
        /// </summary>
        public string WorkpieceNumber { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public string EditionNumber { get; set; }
        /// <summary>
        /// 模具类型
        /// </summary>
        public string MoldType { get; set; }
        /// <summary>
        /// 客户名
        /// </summary>
        public string ClientName { get; set; }
        /// <summary>
        /// 加工类型
        /// </summary>
        public string MachineType { get; set; }
        /// <summary>
        /// 设置模具信息属性
        /// </summary>
        /// <param name="part"></param>
        public bool SetAttribute(NXObject obj)
        {
            try
            {
                AttributeUtils.AttributeOperation("MoldNumber", this.MoldNumber, obj);
                AttributeUtils.AttributeOperation("PieceNumber", this.WorkpieceNumber, obj);
                AttributeUtils.AttributeOperation("EditionNumber", this.EditionNumber, obj);
                AttributeUtils.AttributeOperation("MoldType", this.MoldType, obj);
                AttributeUtils.AttributeOperation("ClientName", this.ClientName, obj);
                AttributeUtils.AttributeOperation("MachineType", this.MachineType, obj);
                return true;
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("写入属性错误！" + ex.Message);
                return false;
            }

        }
        /// <summary>
        /// 读取模具属性
        /// </summary>
        /// <param name="part"></param>
        public static MoldInfo GetAttribute(NXObject obj)
        {
            MoldInfo info = new MoldInfo();
            try
            {
                info.MoldNumber = AttributeUtils.GetAttrForString(obj, "MoldNumber");
                info.WorkpieceNumber = AttributeUtils.GetAttrForString(obj, "PieceNumber");
                info.EditionNumber = AttributeUtils.GetAttrForString(obj, "EditionNumber");
                info.MoldType = AttributeUtils.GetAttrForString(obj, "MoldType");
                info.ClientName = AttributeUtils.GetAttrForString(obj, "ClientName");
                info.MachineType = AttributeUtils.GetAttrForString(obj, "MachineType");
                return info;
            }
            catch (NXException ex)
            {
                throw ex;
            }

        }
        public bool SetAttribute(params NXObject[] objs)
        {
            try
            {
                AttributeUtils.AttributeOperation("MoldNumber", this.MoldNumber, objs);
                AttributeUtils.AttributeOperation("PieceNumber", this.WorkpieceNumber, objs);
                AttributeUtils.AttributeOperation("EditionNumber", this.EditionNumber, objs);
                AttributeUtils.AttributeOperation("MoldType", this.MoldType, objs);
                AttributeUtils.AttributeOperation("ClientName", this.ClientName, objs);
                AttributeUtils.AttributeOperation("MachineType", this.MachineType, objs);
                return true;
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("写入属性错误！" + ex.Message);
                return false;
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

        public bool Equals(MoldInfo other)
        {
            return this.MoldNumber.Equals(other.MoldNumber, StringComparison.CurrentCultureIgnoreCase) &&
                 this.WorkpieceNumber.Equals(other.WorkpieceNumber, StringComparison.CurrentCultureIgnoreCase) &&
                 this.EditionNumber.Equals(other.EditionNumber, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
