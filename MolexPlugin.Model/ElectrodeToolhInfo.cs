using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;

namespace MolexPlugin.Model
{
    /// <summary>
    /// 电极齿信息
    /// </summary>
    public class ElectrodeToolhInfo
    {
        private List<Body> bodys = new List<Body>();
        private List<BodyInfo> infos = new List<BodyInfo>();

        /// <summary>
        /// 电极齿名
        /// </summary>
        public string ToolhName { get; private set; }
        /// <summary>
        /// 电极头信息
        /// </summary>
        public List<BodyInfo> BodyInfos
        {
            get
            {
                if (infos.Count == 0)
                {
                    foreach (Body by in bodys)
                    {
                        infos.Add(BodyInfo.GetAttribute(by));
                    }

                }
                return infos;
            }
        }
        /// <summary>
        /// 是否有放电面积
        /// </summary>
        public bool IsInfoOk
        {
            get
            {
                if (infos.Count == 0)
                {
                    foreach (Body by in bodys)
                    {
                        infos.Add(BodyInfo.GetAttribute(by));
                    }

                }
                foreach (BodyInfo bi in infos)
                {
                    if (bi.ContactArea == 0)
                        return false;
                }
                return true;
            }
        }
        /// <summary>
        /// 偏移量
        /// </summary>
        public double[] Offset { get; private set; } = new double[2];
        public ElectrodeToolhInfo(double[] offset, params Body[] bodys)
        {
            this.bodys = bodys.ToList();
            this.Offset = offset;
            SetAttribute();
        }

        private ElectrodeToolhInfo(params Body[] bodys)
        {
            this.bodys = bodys.ToList();
        }

        private void SetAttribute()
        {
            string time = GetTimeStamp();
            int i = 1;
            foreach (Body by in bodys)
            {
                AttributeUtils.AttributeOperation("ToolhName", time, by);
                AttributeUtils.AttributeOperation("ToolhNumber", i, by);
                AttributeUtils.AttributeOperation("Offset", Offset, by);

                AttributeUtils.AttributeOperation("ToolhName", time, by.GetFaces());
                AttributeUtils.AttributeOperation("ToolhNumber", i, by.GetFaces());
                AttributeUtils.AttributeOperation("Offset", Offset, by.GetFaces());

                i++;
            }
        }
        /// <summary>
        /// 通过属性获得
        /// </summary>     
        /// <param name="bodys">一种齿（没阵列）</param>
        /// <returns></returns>
        public static ElectrodeToolhInfo GetToolhInfoForAttribute(params Body[] bodys)
        {
            ElectrodeToolhInfo info = new ElectrodeToolhInfo(bodys);
            info.ToolhName = AttributeUtils.GetAttrForString(bodys[0], "ToolhName");
            for (int i = 0; i < 2; i++)
            {
                info.Offset[i] = AttributeUtils.GetAttrForDouble(bodys[0], "Offset", i);
            }
            foreach (Body by in bodys)
            {
                BodyInfo byInfo = BodyInfo.GetAttribute(by);
                info.BodyInfos.Add(byInfo);
            }
            return info;
        }
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        private string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }


        public bool SetToolhName(string name)
        {
            try
            {
                foreach (Body by in bodys)
                {
                    AttributeUtils.AttributeOperation("ToolhName", name, by);
                    AttributeUtils.AttributeOperation("ToolhName", name, by.GetFaces());

                }
                return true;
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("电极头写入名字属性错误！" + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 获取全部接触面积
        /// </summary>
        /// <returns></returns>
        public double GetAllContactArea()
        {
            double all = 0;
            foreach (BodyInfo bi in this.BodyInfos)
            {
                all += bi.ContactArea;
            }
            return all;
        }

        /// <summary>
        /// 获取全部投影面积
        /// </summary>
        /// <returns></returns>
        public double GetProjectedArea( Matrix4 matr)
        {
            double all = 0;
            Matrix4 inv = matr.GetInversMatrix();
            CartesianCoordinateSystem csys = BoundingBoxUtils.CreateCoordinateSystem(matr, inv);
            foreach (BodyInfo bi in this.BodyInfos)
            {
                all += bi.GetProjectedArea(csys, matr);
            }
            return all;
        }
    }
}
