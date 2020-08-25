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
    /// 电极信息
    /// </summary>
    [Serializable]
    public class ElectrodeInfo : ParentAssmblieInfo
    {
        public ElectrodeAllInfo AllInfo { get;  set; }

        public Matrix4Info MatrInfo { get;  set; }
        /// <summary>
        /// 矩阵
        /// </summary>
        public Matrix4 Matr { get;  set; }
        public ElectrodeInfo(MoldInfo mold, UserModel user, ElectrodeAllInfo allInfo, Matrix4 mat) : base(mold, user)
        {
            this.Type = PartType.Work;
            this.Matr = mat;
            this.MatrInfo = new Matrix4Info(mat);
            this.AllInfo = allInfo;
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
                return new ElectrodeInfo(MoldInfo.GetAttribute(obj), UserModel.GetAttribute(obj), ElectrodeAllInfo.GetAttribute(obj), Matrix4Info.GetAttribute(obj).Matr);

            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("未获取到属性" + ex.Message);
                return null;
            }
        }
        public override bool SetAttribute(params NXObject[] objs)
        {
            try
            {
                return base.SetAttribute(objs) && this.MatrInfo.SetAttribute(objs) && this.AllInfo.SetAttribute(objs);
            }
            catch
            {
                return false;
            }

        }



    }
}
