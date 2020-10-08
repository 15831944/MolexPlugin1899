using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;
using NXOpen.BlockStyler;
using NXOpen.Utilities;
using Basic;
using MolexPlugin.DAL;
namespace MolexPlugin
{
    public partial class MoveComponent
    {
        /// <summary>
        /// 获取点
        /// </summary>
        /// <returns></returns>
        private Point3d GetPoint()
        {
            Point temp = this.selectionPt;
            if (get_point.GetSelectedObjects().Length != 0)
            {
                if (get_point.GetSelectedObjects()[0] is Point)
                {
                    this.selectionPt = get_point.GetSelectedObjects()[0] as Point;
                    this.selectionPt.Highlight();

                }
                if (get_point.GetSelectedObjects()[0] is Face)
                {
                    Face face = get_point.GetSelectedObjects()[0] as Face;
                    face.Highlight();
                    if (this.selectionFace == null)
                    {
                        this.selectionFace = face;
                    }
                    else
                    {
                        if (this.selectionFace.Tag != face.Tag)
                        {
                            this.selectionFace.Unhighlight();
                            this.selectionFace = face;
                        }
                        else
                        {
                            this.selectionFace = null;
                        }
                    }
                }
            }
            else
            {
                this.selectionPt = temp;
            }
            if (this.selectionPt != null)
            {
                if (this.selectionFace == null)
                    return this.selectionPt.Coordinates;
                else
                    return NXObjectAooearancePoint.GetPointToFaceDis(this.selectionPt, this.selectionFace);
            }
            return new Point3d(0, 0, 0);
        }
    }
}
