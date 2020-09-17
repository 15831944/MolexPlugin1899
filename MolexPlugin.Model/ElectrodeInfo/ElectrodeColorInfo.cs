using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;

namespace MolexPlugin.Model
{
    public class ElectrodeColorInfo : ISetAttribute
    {
        /// <summary>
        /// 颜色名
        /// </summary>
        public string ColorName { get; set; }
        /// <summary>
        /// 颜色号
        /// </summary>
        public int ColorId { get; set; }
        /// <summary>
        /// 齿名
        /// </summary>
        public string ToolhName { get; set; }
        /// <summary>
        /// 间隙
        /// </summary>
        public string GapValue { get; set; }
        /// <summary>
        /// 颜色类型
        /// </summary>
        public ColorType Type { get; set; }
        public bool SetAttribute(params NXObject[] objs)
        {
            try
            {
                AttributeUtils.AttributeOperation("ToolhName", this.ToolhName, objs);
                AttributeUtils.AttributeOperation("GapValue", this.GapValue, objs);
                return true;
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("写入属性错误！" + ex.Message);
                return false;
            }
        }
    }

    public enum ColorType
    {
        Red = 1,
        Orange = 2,
        Purple = 3,
        Green = 4,
        DeepPlum = 5,
        Magenta = 6,
        Yellow = 7,
        Borwn = 8,
        DeepCoral = 9,
        MediumMoss = 10,
        Blue = 11,
        Cornflower = 12,
    }
}
