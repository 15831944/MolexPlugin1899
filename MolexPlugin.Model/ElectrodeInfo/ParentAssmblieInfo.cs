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
    public  class ParentAssmblieInfo : ICloneable, ISetAttribute
    {
        /// <summary>
        /// 模具类型
        /// </summary>
        public MoldInfo MoldInfo { get; protected set; }
        /// <summary>
        /// 创建者
        /// </summary>
        public UserModel UserModel { get; protected set; }
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
            AttributeUtils.AttributeOperation("PartType", Enum.GetName(this.Type.GetType(), this.Type));
            return MoldInfo.SetAttribute(objs) && UserModel.SetAttribute(objs);
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
                return new ParentAssmblieInfo(MoldInfo.GetAttribute(obj), UserModel.GetAttribute(obj));
            }
            catch (NXException ex)
            {
                throw ex;
            }
        }
    }
    public enum PartType
    {
        Workpiece = 1,
        EDM = 2,
        Work = 3,
        ASM = 4,
        Electrode = 5
    }
}
