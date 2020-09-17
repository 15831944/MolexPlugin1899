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
    /// 电极图纸信息
    /// </summary>
    [Serializable]
    public class ElectrodeDrawingInfo : ParentAssmblieInfo
    {
        public ElectrodeAllInfo AllInfo { get; set; }

        public string StrPre { get; set; }
        public ElectrodeDrawingInfo(MoldInfo mold, UserModel user, ElectrodeAllInfo allInfo) : base(mold, user)
        {
            this.Type = PartType.Drawing;
            this.AllInfo = allInfo;
            string temp = AllInfo.Preparetion.Preparation[0].ToString() + "*" + AllInfo.Preparetion.Preparation[1].ToString() + "*" + AllInfo.Preparetion.Preparation[2].ToString();
            this.StrPre = temp;
        }
        /// <summary>
        /// 以属性得到实体
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public new static ElectrodeDrawingInfo GetAttribute(NXObject obj)
        {
            try
            {
                
                return new ElectrodeDrawingInfo(MoldInfo.GetAttribute(obj), UserModel.GetAttribute(obj), ElectrodeAllInfo.GetAttribute(obj));

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
                string temp = AllInfo.Preparetion.Preparation[0].ToString() + "*" + AllInfo.Preparetion.Preparation[1].ToString() + "*" + AllInfo.Preparetion.Preparation[2].ToString();
                AttributeUtils.AttributeOperation("StrPre", temp, objs);
                return base.SetAttribute(objs) && this.AllInfo.SetAttribute(objs);
            }
            catch
            {
                return false;
            }

        }



    }
}
