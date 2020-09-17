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
    /// Y+向加工
    /// </summary>
    public class YPositiveElectrodeMatrix : AbstractElectrodeMatrix
    {
        private Point3d workpieceCenterPt = new Point3d();
        private Point3d workpieceDisPt = new Point3d();
        public YPositiveElectrodeMatrix() 
        {
           
        }
        public override void Initialinze(Matrix4 workMatr, List<Body> eleHead, Part workpiecePart = null)
        {
            base.Initialinze(workMatr, eleHead, workpiecePart);
            if (workpiecePart != null)
                base.GetWorkpieceBounbingBox(workpiecePart, out workpieceCenterPt, out workpieceDisPt);
        }
        public override void SetMatrixOrigin(int[] pre, Point3d originPt)
        {
            originPt.Z = originPt.Z + (pre[1] / 2 - 1.2);
            base.SetMatrixOrigin(pre, originPt);
        }
        public override double[] GetPreparation(ElectrodePitchInfo pitch, bool zDatum)
        {
            double preY = Math.Ceiling(2 * this.disPt.Z) ;
            double preX = Math.Ceiling(2 * this.disPt.X + Math.Abs((pitch.PitchXNum - 1) * pitch.PitchX));
            double preZ = Math.Ceiling(2 * this.disPt.Y) + 45;
            if (zDatum)
            {
                preX = Math.Ceiling(2 * this.disPt.X + Math.Abs((pitch.PitchXNum) * pitch.PitchX));
            }
            if (preX > preY)
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
            return new Point3d(Math.Round(centerPt.X, 3), Math.Round(centerPt.Y - disPt.Y, 3), Math.Round(centerPt.Z - disPt.Z, 3));
        }

        public override double GetZHeight(double exp)
        {
            double x1 = Math.Round(workpieceCenterPt.Y + workpieceDisPt.Y, 3);
            double x2 = Math.Round(centerPt.Y - disPt.Y, 3);
            return (x1 - x2) + exp;
        }


        protected override Matrix4 GetEleMatr()
        {
            Matrix4 mat = new Matrix4();
            mat.Identity();
            Point3d center = workMatr.GetCenter();
            UVector vecX = new UVector();
            UVector vecY = new UVector();
            UVector vecZ = new UVector();
            workMatr.GetYAxis(ref vecZ);
            workMatr.GetXAxis(ref vecX);
            vecY = vecX ^ vecZ;
            UVector point = new UVector(center.X, center.Y, center.Z);
            mat.TransformToZAxis(point, vecX, vecY);
            return mat;
        }
    }
}
