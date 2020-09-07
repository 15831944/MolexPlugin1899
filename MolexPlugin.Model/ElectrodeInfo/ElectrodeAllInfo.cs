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
    /// 电极全部信息
    /// </summary>
    [Serializable]
    public class ElectrodeAllInfo:ISetAttribute,ICloneable
    {
        /// <summary>
        /// 备注信息
        /// </summary>
        public ElectrodeRemarksInfo Remarks { get; set; }
        /// <summary>
        /// 电极程序信息
        /// </summary>
        public ElectrodeCAMInfo CAM { get; set; }
        /// <summary>
        /// 电极间隙信息
        /// </summary>
        public ElectrodeGapValueInfo GapValue { get; set; }
        /// <summary>
        /// PH信息
        /// </summary>
        public ElectrodePitchInfo Pitch { get; set; }
        /// <summary>
        /// 备料信息
        /// </summary>
        public ElectrodePreparationInfo Preparetion { get; set; }
        /// <summary>
        /// 电极名字信息
        /// </summary>
        public ElectrodeNameInfo Name { get; set; }
        /// <summary>
        /// 设定信息
        /// </summary>
        public ElectrodeSetValueInfo SetValue { get; set; }
        /// <summary>
        /// 基准台信息
        /// </summary>
        public ElectrodeDatumInfo Datum { get; set; }
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
        public bool SetAttribute(Part obj)
        {
            return this.Remarks.SetAttribute(obj) && this.CAM.SetAttribute(obj) && this.GapValue.SetAttribute(obj)
                  && this.Pitch.SetAttribute(obj) && this.Preparetion.SetAttribute(obj) && this.Name.SetAttribute(obj)
                  && this.SetValue.SetAttribute(obj) && this.Datum.SetAttribute(obj);
        }
        /// <summary>
        /// 读取属性
        /// </summary>
        public static ElectrodeAllInfo GetAttribute(NXObject obj)
        {         
            try
            {
                ElectrodeRemarksInfo rem = ElectrodeRemarksInfo.GetAttribute(obj);
                ElectrodeCAMInfo cam = ElectrodeCAMInfo.GetAttribute(obj);
                ElectrodeGapValueInfo gap = ElectrodeGapValueInfo.GetAttribute(obj);
                ElectrodePitchInfo pitch = ElectrodePitchInfo.GetAttribute(obj);
                ElectrodePreparationInfo pre = ElectrodePreparationInfo.GetAttribute(obj);
                ElectrodeSetValueInfo sv = ElectrodeSetValueInfo.GetAttribute(obj);
                ElectrodeNameInfo name = ElectrodeNameInfo.GetAttribute(obj);
                ElectrodeDatumInfo da = ElectrodeDatumInfo.GetAttribute(obj);
                return new ElectrodeAllInfo()
                {
                    Remarks = rem,
                    CAM = cam,
                    GapValue = gap,
                    Pitch = pitch,
                    Preparetion = pre,
                    SetValue = sv,
                    Name = name,
                    Datum=da
                };
            }
            catch (NXException ex)
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
            return this.Remarks.SetAttribute(objs) && this.CAM.SetAttribute(objs) && this.GapValue.SetAttribute(objs)
                   && this.Pitch.SetAttribute(objs) && this.Preparetion.SetAttribute(objs) && this.Name.SetAttribute(objs)
                   && this.SetValue.SetAttribute(objs) && this.Datum.SetAttribute(objs);
        }
    }
}
