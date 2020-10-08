using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;
using MolexPlugin.DAL;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 电极信息更新
    /// </summary>
    public class ElectrodePitchUpdate
    {
        private ElectrodePitchInfo oldPitch;
        private ElectrodePitchInfo newPitch;

        public ElectrodePitchUpdate(ElectrodePitchInfo oldPitch, ElectrodePitchInfo newPitch)
        {
            this.oldPitch = oldPitch;
            this.newPitch = newPitch;
        }
        /// <summary>
        /// 获取向量增量
        /// </summary>
        /// <param name="oldPitch"></param>
        /// <param name="newPitch"></param>
        /// <returns></returns>
        public Vector3d GetIncrement()
        {
            Vector3d temp = new Vector3d();
            temp.X = ((newPitch.PitchXNum) * (newPitch.PitchX) - (oldPitch.PitchXNum) * (oldPitch.PitchX)) / 2;
            temp.Y = ((newPitch.PitchYNum) * (newPitch.PitchY) - (oldPitch.PitchYNum) * (oldPitch.PitchY)) / 2;
            return temp;
        }
        /// <summary>
        /// 获取备料尺寸
        /// </summary>
        /// <param name="oldPre"></param>
        /// <param name="oldPitch"></param>
        /// <param name="newPitch"></param>
        /// <returns></returns>
        public int[] GetNewPreparation(int[] oldPre, string material)
        {
            EletrodePreparation pre;
            double x = Math.Floor((newPitch.PitchX) * (newPitch.PitchXNum) - (oldPitch.PitchX) * (oldPitch.PitchXNum));
            double y = Math.Floor((newPitch.PitchY) * (newPitch.PitchYNum) - (oldPitch.PitchY) * (oldPitch.PitchYNum));
            oldPre[0] = oldPre[0] + (int)x;
            oldPre[1] = oldPre[1] + (int)y;
            if (material.Equals("紫铜"))
            {
                pre = new EletrodePreparation("CuLength", "CuWidth");
            }
            else
            {
                pre = new EletrodePreparation("WuLength", "WuWidth");
            }
            pre.GetPreparation(ref oldPre);
            return oldPre;
        }
        /// <summary>
        /// 获取新设定值
        /// </summary>
        /// <param name="oldSetValue"></param>
        /// <returns></returns>
        public double[] GetNewSetValue(double[] oldSetValue)
        {
            double[] tem = new double[3];
            Vector3d inc = GetIncrement();
            tem[0] = oldSetValue[0] + inc.X;
            tem[1] = oldSetValue[1] + inc.Y;
            tem[2] = oldSetValue[2];
            return tem;
        }


    }
}
