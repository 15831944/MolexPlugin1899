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
    /// X+向加工
    /// </summary>
    public class XPositiveElectrodeMatrix : AbstractElectrodeMatrix
    {
        private Point3d workpieceCenterPt = new Point3d();
        private Point3d workpieceDisPt = new Point3d();
        public XPositiveElectrodeMatrix()
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
            originPt.Z = originPt.Z + (pre[0] / 2 - 1.2);
            base.SetMatrixOrigin(pre, originPt);
        }
        public override double[] GetPreparation(ElectrodePitchInfo pitch, bool zDatum)
        {
            double preX = Math.Ceiling(2 * disPt.Z);
            double preY = Math.Ceiling(2 * disPt.Y + Math.Abs((pitch.PitchYNum - 1) * pitch.PitchY));
            double preZ = Math.Ceiling(2 * disPt.X) + 45;
            if (zDatum)
            {
                preY = Math.Ceiling(2 * this.disPt.Y + Math.Abs((pitch.PitchYNum) * pitch.PitchY));
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
            return new Point3d(Math.Round(centerPt.X + disPt.X, 3), Math.Round(centerPt.Y, 3), Math.Round(centerPt.Z - disPt.Z, 3));
        }

        public override double GetZHeight(double exp)
        {
            double x1 = Math.Round(workpieceCenterPt.X - workpieceDisPt.X, 3);
            double x2 = Math.Round(centerPt.X + disPt.X, 3);
            return (x2 - x1) + exp;
        }

        protected override Matrix4 GetEleMatr()
        {
            Matrix4 mat = new Matrix4();
            mat.Identity();
            UVector vecX = new UVector();
            UVector vecY = new UVector();
            UVector vecZ = new UVector();
            Point3d center = workMatr.GetCenter();
            workMatr.GetYAxis(ref vecY);
            workMatr.GetXAxis(ref vecZ);
            vecX = vecY ^ vecZ;
            UVector point = new UVector(center.X, center.Y, center.Z);
            mat.TransformToZAxis(point, vecX, vecY);
            return mat;
        }
    }
}
