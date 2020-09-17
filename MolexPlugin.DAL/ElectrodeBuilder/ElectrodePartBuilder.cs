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
                // this.EleModel.Info.SetAttribute(this.EleComp);
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
                List<Body> waveBodys = AssmbliesUtils.WaveBodys(headBodys.ToArray()).GetBodies().ToList();
                Matrix4 inv = this.info.Matr.GetInversMatrix();
                CoordinateSystem csys = BoundingBoxUtils.CreateCoordinateSystem(this.info.Matr, inv);
                NXObject obj = MoveObject.MoveObjectOfCsys(csys, waveBodys.ToArray());
                return waveBodys; ;
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

        public Point3d GetSetValuePoint(bool zDatum)
        {
            double[] setValue = this.EleModel.Info.AllInfo.SetValue.EleSetValue;
            Point3d temp = new Point3d(setValue[0], setValue[1], setValue[2]);
            Vector3d vec = this.GetMove(zDatum);
            temp.X = temp.X - vec.X;
            temp.Y = temp.Y - vec.Y;
            return temp; ;
        }

    }
}
