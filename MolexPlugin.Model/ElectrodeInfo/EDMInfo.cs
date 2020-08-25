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
    /// EDM信息
    /// </summary>
    [Serializable]
    public class EDMInfo : ParentAssmblieInfo
    {
        public int EdmNumber { get; private set; }
        public EDMInfo(MoldInfo mold, UserModel user, int edmNum) : base(mold, user)
        {
            this.Type = PartType.EDM;
            this.EdmNumber = edmNum;
        }
        /// <summary>
        /// 以属性得到实体
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ParentAssmblieInfo GetAttribute(NXObject obj)
        {

            int num = 0;
            try
            {
                num = AttributeUtils.GetAttrForInt(obj, "EdmNumber");
                return new EDMInfo(MoldInfo.GetAttribute(obj), UserModel.GetAttribute(obj), num);

            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("未获取到EdmNumber 属性" + ex.Message);
                return new EDMInfo(MoldInfo.GetAttribute(obj), UserModel.GetAttribute(obj), num);
            }
        }
        public override bool SetAttribute(params NXObject[] objs)
        {
            try
            {
                AttributeUtils.AttributeOperation("EdmNumber", this.EdmNumber, objs);
                return base.SetAttribute(objs);
            }
            catch
            {
                return false;
            }

        }
    }
}
