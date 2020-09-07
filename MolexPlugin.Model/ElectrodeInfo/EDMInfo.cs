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
     
        public EDMInfo(MoldInfo mold, UserModel user) : base(mold, user)
        {
            this.Type = PartType.EDM;
          
        }
        /// <summary>
        /// 以属性得到实体
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public new static EDMInfo GetAttribute(NXObject obj)
        {

         
            try
            {
              
                return new EDMInfo(MoldInfo.GetAttribute(obj), UserModel.GetAttribute(obj));

            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("未获取到EdmNumber 属性" + ex.Message);
                return null;
            }
        }
        public override bool SetAttribute(params NXObject[] objs)
        {
            try
            {               
                return base.SetAttribute(objs);
            }
            catch
            {
                return false;
            }

        }
    }
}
