using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;
using MolexPlugin.Model;


namespace MolexPlugin.DAL
{
    /// <summary>
    /// 电极颜色
    /// </summary>
    public class ElectrodeColorList
    {
        public List<ElectrodeColorInfo> GetAllColorInfo()
        {
            List<ElectrodeColorInfo> eci = new List<ElectrodeColorInfo>();
            eci.Add(CreateRed());
            eci.Add(CreateOrange());
            eci.Add(CreatePurple());
            eci.Add(CreateGreen());
            eci.Add(CreateDeepPlum());
            eci.Add(CreateMagenta());
            eci.Add(CreateYellow());
            eci.Add(CreateBorwn());
            eci.Add(CreateDeepCoral());
            eci.Add(CreateMediumMoss());
            eci.Add(CreateBlue());
            eci.Add(CreateCornflower());
            eci.OrderBy(a => a.Type);
            return eci;
        }
        /// <summary>
        /// 红色
        /// </summary>
        /// <returns></returns>
        private ElectrodeColorInfo CreateRed()
        {
            return new ElectrodeColorInfo()
            {
                ColorName = "Red",
                ColorId = 186,
                Type = ColorType.Red
            };
        }
        /// <summary>
        /// 橙色
        /// </summary>
        /// <returns></returns>
        private ElectrodeColorInfo CreateOrange()
        {
            return new ElectrodeColorInfo()
            {
                ColorName = "Orange",
                ColorId = 78,
                Type = ColorType.Orange
            };
        }
        /// <summary>
        /// 紫色
        /// </summary>
        /// <returns></returns>
        private ElectrodeColorInfo CreatePurple()
        {
            return new ElectrodeColorInfo()
            {
                ColorName = "Purple",
                ColorId = 164,
                Type = ColorType.Purple
            };
        }
        /// <summary>
        /// 绿色
        /// </summary>
        /// <returns></returns>
        private ElectrodeColorInfo CreateGreen()
        {
            return new ElectrodeColorInfo()
            {
                ColorName = "Green",
                ColorId = 36,
                Type = ColorType.Green
            };
        }
        /// <summary>
        /// 深梅色
        /// </summary>
        /// <returns></returns>
        private ElectrodeColorInfo CreateDeepPlum()
        {
            return new ElectrodeColorInfo()
            {
                ColorName = "DeepPlum",
                ColorId = 203,
                Type = ColorType.DeepPlum
            };
        }
        /// <summary>
        /// 紫红色
        /// </summary>
        /// <returns></returns>
        private ElectrodeColorInfo CreateMagenta()
        {
            return new ElectrodeColorInfo()
            {
                ColorName = "Magenta",
                ColorId = 181,
                Type = ColorType.Magenta
            };
        }

        /// <summary>
        /// 黄色
        /// </summary>
        /// <returns></returns>
        private ElectrodeColorInfo CreateYellow()
        {
            return new ElectrodeColorInfo()
            {
                ColorName = "Yellow",
                ColorId = 6,
                Type = ColorType.Yellow
            };
        }
        /// <summary>
        /// 棕色
        /// </summary>
        /// <returns></returns>
        private ElectrodeColorInfo CreateBorwn()
        {
            return new ElectrodeColorInfo()
            {
                ColorName = "Borwn",
                ColorId = 125,
                Type = ColorType.Borwn
            };
        }

        /// <summary>
        /// 深珊瑚
        /// </summary>
        /// <returns></returns>
        private ElectrodeColorInfo CreateDeepCoral()
        {
            return new ElectrodeColorInfo()
            {
                ColorName = "DeepCoral",
                ColorId = 192,
                Type = ColorType.DeepCoral
            };
        }

        /// <summary>
        /// 中苔
        /// </summary>
        /// <returns></returns>
        private ElectrodeColorInfo CreateMediumMoss()
        {
            return new ElectrodeColorInfo()
            {
                ColorName = "MediumMoss",
                ColorId = 100,
                Type = ColorType.MediumMoss
            };
        }

        /// <summary>
        /// 蓝色
        /// </summary>
        /// <returns></returns>
        private ElectrodeColorInfo CreateBlue()
        {
            return new ElectrodeColorInfo()
            {
                ColorName = "Blue",
                ColorId = 211,
                Type = ColorType.Blue
            };
        }

        /// <summary>
        /// 矢车菊
        /// </summary>
        /// <returns></returns>
        private ElectrodeColorInfo CreateCornflower()
        {
            return new ElectrodeColorInfo()
            {
                ColorName = "Cornflower",
                ColorId = 103,
                Type = ColorType.Cornflower
            };
        }
    }
}
