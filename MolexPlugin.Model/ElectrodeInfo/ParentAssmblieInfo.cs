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
    /// 装配档信息父类
    /// </summary>
    [Serializable]
    public class ParentAssmblieInfo : ICloneable, ISetAttribute
    {
        /// <summary>
        /// 模具类型
        /// </summary>
        public MoldInfo MoldInfo { get;  set; }
        /// <summary>
        /// 创建者
        /// </summary>
        public UserModel UserModel { get;  set; }
        /// <summary>
        /// 文档类型
        /// </summary>
        public PartType Type { get; protected set; }
        public ParentAssmblieInfo(MoldInfo mold, UserModel user)
        {
            this.MoldInfo = mold;
            this.UserModel = user;
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
        /// 设置属性
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        public virtual bool SetAttribute(params NXObject[] objs)
        {
            try
            {
                AttributeUtils.AttributeOperation("PartType", Enum.GetName(this.Type.GetType(), this.Type), objs);
                return MoldInfo.SetAttribute(objs) && UserModel.SetAttribute(objs);
            }
            catch
            {
                return false;
            }

        }
        /// <summary>
        /// 以属性得到实体
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ParentAssmblieInfo GetAttribute(NXObject obj)
        {
            try
            {
                string partType = AttributeUtils.GetAttrForString(obj, "PartType");
                ParentAssmblieInfo info = new ParentAssmblieInfo(MoldInfo.GetAttribute(obj), UserModel.GetAttribute(obj));
                info.Type = (PartType)Enum.Parse(typeof(PartType), partType);
                return info;
            }
            catch (NXException ex)
            {
                throw ex;
            }
        }

        public static bool IsParent(NXObject obj)
        {
            try
            {
                string partType = AttributeUtils.GetAttrForString(obj, "PartType");
                if (partType.Equals(""))
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 判断是否电极
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsElectrode(NXObject obj)
        {
            try
            {
                string partType = AttributeUtils.GetAttrForString(obj, "PartType");
                if (partType.Equals("Electrode"))
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 判断是否Workpiece
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsWorkpiece(NXObject obj)
        {
            try
            {
                string partType = AttributeUtils.GetAttrForString(obj, "PartType");
                if (partType.Equals("Workpiece"))
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 判断是否EDM
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsEDM(NXObject obj)
        {
            try
            {
                string partType = AttributeUtils.GetAttrForString(obj, "PartType");
                if (partType.Equals("EDM"))
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 判断是否Work
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsWork(NXObject obj)
        {
            try
            {
                string partType = AttributeUtils.GetAttrForString(obj, "PartType");
                if (partType.Equals("Work"))
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 判断是否ASM
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsAsm(NXObject obj)
        {
            try
            {
                string partType = AttributeUtils.GetAttrForString(obj, "PartType");
                if (partType.Equals("ASM"))
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 判断是否Drawing
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsDrawing(NXObject obj)
        {
            try
            {
                string partType = AttributeUtils.GetAttrForString(obj, "PartType");
                if (partType.Equals("Drawing"))
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
    }
    public enum PartType
    {
        Workpiece = 1,
        EDM = 2,
        Work = 3,
        ASM = 4,
        Electrode = 5,
        Drawing = 6
    }
}
