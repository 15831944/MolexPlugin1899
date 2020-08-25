using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basic;
using NXOpen;
using NXOpen.BlockStyler;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 方块盒
    /// </summary>
    public class SuperBoxCylinder : AbstractSuperBox
    {
        public SuperBoxCylinder(List<NXObject> objs, double[] offset) : base(objs, offset)
        {
        }

        public override void CreateSuperBox()
        {
            if (this.DisPt.X > this.DisPt.Y)
            {
                if ((this.DisPt.X + this.Offset[2]) <= 0 && (2 * this.DisPt.Z + this.Offset[0] + this.Offset[1]) <= 0)
                    return;
            }
            if (this.DisPt.X < this.DisPt.Y)
            {
                if ((this.DisPt.Y + this.Offset[2]) <= 0 && (2 * this.DisPt.Z + this.Offset[0] + this.Offset[1]) <= 0)
                    return;
            }
            else
                base.ToolingBoxFeature = Basic.ToolingBoxFeature.CreateToolingCylinder(this.Matr.GetZAxis(), this.CenterPt, this.Offset, base.ToolingBoxFeature, base.objs.ToArray());
        }

        public override void SetDimForFace(ref LinearDimension ld, Vector3d vec)
        {
            foreach (Face face in this.ToolingBoxFeature.GetBodies()[0].GetFaces())
            {
                if (face.SolidFaceType == Face.FaceType.Cylindrical)
                {
                    Point3d originPt = new Point3d(0, 0, 0);
                    Vector3d normal = new Vector3d(0, 0, 0);
                    FaceUtils.AskFaceOriginAndNormal(face, out originPt, out normal);
                    double angle1 = UMathUtils.Angle(vec, new Vector3d(1, 1, 1));
                    if (UMathUtils.IsEqual(angle1, 0))
                    {
                        ld.HandleOrientation = normal;
                        ld.HandleOrigin = originPt;
                    }
                }
                else
                {
                    FaceData fd = FaceUtils.AskFaceData(face);
                    Vector3d temp = fd.Dir;
                    this.Matr.ApplyVec(ref temp);
                    double angle = UMathUtils.Angle(vec, temp);
                    if (UMathUtils.IsEqual(angle, 0))
                    {
                        ld.HandleOrientation = fd.Dir;
                        ld.HandleOrigin = fd.Point;
                    }

                }
            }
        }

        public override void Update(Matrix4 matr, double[] offset)
        {
            GetBoundingBox();
            if (this.DisPt.X > this.DisPt.Y)
            {
                if ((this.DisPt.X + offset[2]) <= 0 && (2 * this.DisPt.Z + offset[0] + offset[1]) <= 0)
                    return;
            }
            if (this.DisPt.X < this.DisPt.Y)
            {
                if ((this.DisPt.Y + offset[2]) <= 0 && (2 * this.DisPt.Z + offset[0] + offset[1]) <= 0)
                    return;
            }
            this.matr = matr;
            this.Offset = offset;
            CreateSuperBox();
        }

        public override void UpdateSpecify(UIBlock ui)
        {
            if (ui is SpecifyVector)
            {
                SpecifyVector sv = ui as SpecifyVector;
                sv.Vector = this.Matr.GetZAxis();
                sv.Point = this.CenterPt;
            }
        }
    }
}
