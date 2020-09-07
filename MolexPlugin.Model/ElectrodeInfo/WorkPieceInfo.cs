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
    /// WorkPiece信息
    /// </summary>
    [Serializable]
    public class WorkPieceInfo : ParentAssmblieInfo
    {

        public WorkPieceInfo(MoldInfo mold, UserModel user) : base(mold, user)
        {
            this.Type = PartType.Workpiece;
        }
        /// <summary>
        /// 以属性得到实体
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public new static WorkPieceInfo GetAttribute(NXObject obj)
        {
            try
            {
                return new WorkPieceInfo(MoldInfo.GetAttribute(obj), UserModel.GetAttribute(obj));
            }
            catch (NXException ex)
            {
                throw ex;
            }
        }
    }
}
