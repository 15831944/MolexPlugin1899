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
    /// ASM信息
    /// </summary>
    [Serializable]
    public class ASMInfo : ParentAssmblieInfo
    {

        public ASMInfo(MoldInfo mold, UserModel user) : base(mold, user)
        {
            this.Type = PartType.ASM;
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
                return new ASMInfo(MoldInfo.GetAttribute(obj), UserModel.GetAttribute(obj));
            }
            catch (NXException ex)
            {
                throw ex;
            }
        }
    }
}
