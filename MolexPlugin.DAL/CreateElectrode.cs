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
    /// 创建电极
    /// </summary>
    public class CreateElectrode
    {
        private ElectrodeAllInfo allInfo;
        private ElectrodeCreateCondition condition;
        private ParentAssmblieInfo parent;
        public bool zDatum;

        public CreateElectrode(ElectrodeAllInfo allInfo, ParentAssmblieInfo parent, ElectrodeCreateCondition condition, bool zDatum)
        {
            this.allInfo = allInfo;
            this.parent = parent;
            this.condition = condition;
            this.zDatum = zDatum;
        }

        /// <summary>
        /// 反求单齿设置值
        /// </summary>
        /// <param name="setValue"></param>
        /// <param name="pitch"></param>
        /// <param name="pre"></param>
        /// <param name="zDatum"></param>
        /// <returns></returns>
        private Point3d GetSingleHeadSetValue()
        {
            Point3d temp = new Point3d(allInfo.SetValue.EleSetValue[0], allInfo.SetValue.EleSetValue[1], allInfo.SetValue.EleSetValue[2]);
            double x1 = temp.X - (allInfo.Pitch.PitchXNum - 1) * allInfo.Pitch.PitchX / 2;
            double y1 = temp.Y - (allInfo.Pitch.PitchYNum - 1) * allInfo.Pitch.PitchY / 2;
            if (zDatum)
            {
                if (allInfo.Preparetion.Preparation[0] > allInfo.Preparetion.Preparation[1])
                {
                    x1 = temp.X - (allInfo.Pitch.PitchXNum - 2) * allInfo.Pitch.PitchX / 2;
                }
                else
                {
                    y1 = temp.Y - (allInfo.Pitch.PitchYNum - 2) * allInfo.Pitch.PitchY / 2;
                }
            }
            return new Point3d(x1, y1, temp.Z);
        }
        /// <summary>
        /// 创建电极Model
        /// </summary>
        /// <returns></returns>
        private ElectrodeInfo GetEleInfo()
        {
            condition.ExpAndMatr.Matr.SetMatrixOrigin(allInfo.Preparetion.Preparation, GetSingleHeadSetValue()); //设置矩阵中心点
            Matrix4 eleMat = condition.ExpAndMatr.Matr.EleMatr;
            Matrix4Info matInfo = new Matrix4Info(eleMat);
            return new ElectrodeInfo(parent.MoldInfo, parent.UserModel, allInfo, matInfo);
        }

        public bool CreateBuider()
        {
            ElectrodePartBuilder part = new ElectrodePartBuilder(GetEleInfo(), condition.Work.WorkpieceDirectoryPath);
            if (part.CreatPart())
            {
                try
                {
                    List<Body> headBodys = part.WaveEleHeadBody(condition.HeadBodys);
                    bool isok = condition.ExpAndMatr.Exp.CreateExp(zDatum);
                    if (headBodys.Count == 0 || !isok)
                        return false;
                    ElectrodeSketchBuilder sketch = new ElectrodeSketchBuilder(allInfo.Preparetion.Preparation[0], allInfo.Preparetion.Preparation[1], -allInfo.Datum.EleHeight);
                    ElectrodeDatumBuilder datum = new ElectrodeDatumBuilder(sketch);
                    ElectrodeMoveBuilder move = new ElectrodeMoveBuilder(headBodys, allInfo.Datum, allInfo.GapValue, allInfo.Pitch);
                    datum.SetParentBuilder(sketch);
                    sketch.SetParentBuilder(move);
                    sketch.CreateBuilder();
                    move.CreateBuilder();
                    datum.CreateBuilder();
                    PartUtils.SetPartDisplay(condition.Work.PartTag);
                    part.MoveEleComp(condition.Work.Info.Matr, part.GetMove(zDatum));
                    return true;
                }
                catch (NXException ex)
                {
                    ClassItem.WriteLogFile("创建特征失败！" + ex.Message);
                }

            }
            return false;

        }
    }
}
