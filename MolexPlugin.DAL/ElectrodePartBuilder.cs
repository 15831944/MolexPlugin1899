using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;
using Basic;
using MolexPlugin.Model;
using System.IO;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 创建草绘特征
    /// </summary>
    public class ElectrodePartBuilder
    {
        private string directoryPath;
        private ElectrodeInfo info;


        public ElectrodeModel EleModel { get; private set; }

        public NXOpen.Assemblies.Component EleComp { get; private set; }


        public ElectrodePartBuilder(ElectrodeInfo eleInfo, string directoryPath)
        {
            this.info = eleInfo;
            this.EleModel = new ElectrodeModel(eleInfo);
            this.directoryPath = directoryPath;
        }
        public bool CreatPart()
        {
            if (File.Exists(this.directoryPath + EleModel.AssembleName + ".prt"))
            {
                ClassItem.MessageBox("电极重名！", NXMessageBox.DialogType.Error);
                return false;
            }
            try
            {
                this.EleComp = this.EleModel.CreateCompPart(this.directoryPath);
                return true;
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("创建Part档错误！" + ex.Message);
                return false;
            }

        }
        /// <summary>
        ///连接电极
        /// </summary>
        /// <param name="headBodys"></param>
        /// <returns></returns>
        public List<Body> WaveEleHeadBody(List<Body> headBodys)
        {

            try
            {
                PartUtils.SetPartWork(this.EleComp);
                List<Body> waveBodys = AssmbliesUtils.WaveAssociativeBodys(headBodys.ToArray()).GetBodies().ToList();             
                PartUtils.SetPartDisplay(EleModel.PartTag);
                return waveBodys;
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("连接体错误！");
                throw ex;
            }
        }
        /// <summary>
        /// 获取移动向量
        /// </summary>
        /// <param name="zDatum"></param>
        /// <returns></returns>
        public Vector3d GetMove(bool zDatum)
        {
            Vector3d temp = new Vector3d();
            temp.X = (info.AllInfo.Pitch.PitchXNum - 1) * info.AllInfo.Pitch.PitchX / 2;
            temp.Y = (info.AllInfo.Pitch.PitchYNum - 1) * info.AllInfo.Pitch.PitchY / 2;
            temp.Z = 0;
            if (zDatum)
            {
                if (info.AllInfo.Preparetion.Preparation[0] >= info.AllInfo.Preparetion.Preparation[1])
                {
                    temp.X = (info.AllInfo.Pitch.PitchXNum - 2) * info.AllInfo.Pitch.PitchX / 2;
                }
                else
                {
                    temp.Y = (info.AllInfo.Pitch.PitchYNum - 2) * info.AllInfo.Pitch.PitchY / 2;
                }

            }
            return temp;
        }
        /// <summary>
        /// 移动装配
        /// </summary>
        /// <param name="workMat"></param>
        /// <param name="vec"></param>
        /// <returns></returns>
        public void MoveEleComp(Matrix4 workMat, Vector3d vec)
        {
            AssmbliesUtils.MoveCompPart(this.EleComp, vec, workMat); //移动装配
        }

    }
}
