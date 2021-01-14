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
    ///Z-向加工
    /// </summary>
    public class ZPositiveElectrodeMatrix : AbstractElectrodeMatrix
    {

        public ZPositiveElectrodeMatrix()
        {
            this.EleProcessDir = "Z+";
        }

        public override double[] GetPreparation(ElectrodePitchInfo pitch, bool zDatum)
        {
            double preX = Math.Ceiling(2 * disPt.X + Math.Abs((pitch.PitchXNum - 1) * pitch.PitchX)) ;
            double preY = Math.Ceiling(2 * disPt.Y + Math.Abs((pitch.PitchYNum - 1) * pitch.PitchY)) ;
            double preZ = Math.Ceiling(Math.Abs(this.centerPt.Z - disPt.Z)) + 35;
            if (zDatum)
            {
                if (preX >= preY)
                {
                    preX = Math.Ceiling(2 * disPt.X + Math.Abs((pitch.PitchXNum) * pitch.PitchX)) ;
                }
                else
                {
                    preY = Math.Ceiling(2 * disPt.Y + Math.Abs((pitch.PitchYNum) * pitch.PitchY));
                }
            }
            if(preX>preY)
            {
                preX = preX + 6;
                preY = preY + 2;
            }
            else
            {
                preX = preX + 2;
                preY = preY + 6;
            }
            return new double[3] { preX, preY, preZ };
        }

        public override Point3d GetSingleHeadSetValue()
        {
            return new Point3d(Math.Round(centerPt.X, 3), Math.Round(centerPt.Y, 3), Math.Round(centerPt.Z - disPt.Z, 3));
        }

        public override double GetZHeight(double exp)
        {
            return Math.Abs(centerPt.Z - disPt.Z) + exp;
        }

        protected override Matrix4 GetEleMatr()
        {
            Vector3d vecY = workMatr.GetYAxis();
            vecY.X = -vecY.X;
            vecY.Y = -vecY.Y;
            vecY.Z = -vecY.Z;
            Matrix4 mat = new Matrix4();
            mat.Identity();
            mat.TransformToZAxis(workMatr.GetCenter(), workMatr.GetXAxis(), vecY);
            return mat;
        }
    }
}
