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
    public partial class SuperBox
    {
        /// <summary>
        /// 设置尺寸显示
        /// </summary>
        /// <param name="show"></param>
        private void SetLinearDimensionShow(bool show)
        {
            this.positiveX.Show = show;
            this.positiveY.Show = show;
            this.positiveZ.Show = show;
            this.negativeX.Show = show;
            this.negativeY.Show = show;
            this.negativeZ.Show = show;
        }

        private void SetLinearDimensionDouble(double value)
        {
            this.positiveX.Value = value;
            this.positiveY.Value = value;
            this.positiveZ.Value = value;
            this.negativeX.Value = value;
            this.negativeY.Value = value;
            this.negativeZ.Value = value;
            this.radial.Value = value;
        }
        /// <summary>
        /// 初始对话显示
        /// </summary>
        private void SetUiInitialize()
        {
            if (type.ValueAsString == "块")
            {
                vector.Show = false;
                manip.Show = true;
                manip.Enable = true;
                for (int i = 0; i < 6; i++)
                {
                    blockOffset[i] = dimOffset.Value;
                }
            }
            if (type.ValueAsString == "圆柱")
            {
                vector.Show = true;
                manip.Show = false;
                for (int i = 0; i < 3; i++)
                {
                    cylinderOffset[i] = dimOffset.Value;
                }
            }


        }
        /// <summary>
        ///设置面上的向量
        /// </summary>
        private void SetDimForFace()
        {
            superBox.SetDimForFace(ref this.positiveX, new Vector3d(1, 0, 0));
            superBox.SetDimForFace(ref this.negativeX, new Vector3d(-1, 0, 0));
            superBox.SetDimForFace(ref this.positiveY, new Vector3d(0, 1, 0));
            superBox.SetDimForFace(ref this.negativeY, new Vector3d(0, -1, 0));
            superBox.SetDimForFace(ref this.positiveZ, new Vector3d(0, 0, 1));
            superBox.SetDimForFace(ref this.negativeZ, new Vector3d(0, 0, -1));
            superBox.SetDimForFace(ref this.radial, new Vector3d(1, 1, 1));
        }
        /// <summary>
        /// 设置相同偏置
        /// </summary>
        /// <param name="dim"></param>
        /// <param name="offset"></param>
        private void SetDimSame(double dim, ref double[] offset)
        {
            for (int i = 0; i < offset.Length; i++)
            {
                offset[i] = dim;
            }
        }
        /// <summary>
        /// 设置拖拉预览
        /// </summary>
        /// <param name="ld"></param>
        private void SetDimFeatuer(LinearDimension ld, int row, ref double[] offset)
        {

            double temp = UMathUtils.GetDis(ld.HandleOrigin, superBox.CenterPt);     //判断拖拉是否超过边界     
            if (Math.Abs(ld.Value) >= 2 * temp && ld.Value < 0)
                return;
            if (boolOffset.Value)
            {
                SetDimSame(ld.Value, ref offset);
                dimOffset.Value = ld.Value;
                negativeX.Value = ld.Value;
                negativeY.Value = ld.Value;
                negativeZ.Value = ld.Value;

                positiveX.Value = ld.Value;
                positiveX.Value = ld.Value;
                positiveZ.Value = ld.Value;
            }
            else
                offset[row] = ld.Value;
            superBox.Update(matr, offset);
        }
        /// <summary>
        /// 创建平面相减
        /// </summary>
        /// <param name="seleTags"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        private List<NXOpen.Features.TrimBody2> Trim(List<NXObject> seleTags, Body body)
        {
            List<Face> planeFace = new List<Face>();
            foreach (NXObject nb in seleTags)
            {
                if (nb is Face)
                {
                    Face face = nb as Face;
                    if (face.SolidFaceType == Face.FaceType.Planar)
                        planeFace.Add(face);
                }
            }
            if (planeFace != null || planeFace.Count != 0)
            {
                BoxTrimBodyBuilder builder = new BoxTrimBodyBuilder(planeFace, body);
                return builder.CreateBuilder();
            }
            return null;
        }
        /// <summary>
        /// 创建布尔减
        /// </summary>
        /// <returns></returns>
        private NXOpen.Features.BooleanFeature CreateBooleanFeature()
        {
            Body toolBody = AskSelectParent();
            if (toolBody != null && this.superBox.ToolingBoxFeature != null)
            {
                return BooleanUtils.CreateBooleanFeature(this.superBox.ToolingBoxFeature.GetBodies()[0], false, true, NXOpen.Features.Feature.BooleanType.Subtract, toolBody);
            }
            return null;
        }
        /// <summary>
        /// 获取选择的父项
        /// </summary>
        /// <returns></returns>
        private Body AskSelectParent()
        {
            Body parentBody = null;
            foreach (NXObject nt in this.seleTags)
            {
                if (nt is Edge)
                {
                    parentBody = (nt as Edge).GetBody();
                    break;
                }
                if (nt is Face)
                {
                    parentBody = (nt as Face).GetBody();
                    break;
                }
                if (nt is Point)
                {
                    Point pt = nt as Point;
                    int parents;
                    Tag[] parentTags;
                    Tag bodyTag = Tag.Null;
                    int type;
                    int subtype;
                    theUFSession.So.AskParents(pt.Tag, UFConstants.UF_SO_ASK_ALL_PARENTS, out parents, out parentTags);
                    for (int i = 0; i < parents; i++)
                    {
                        theUFSession.Obj.AskTypeAndSubtype(parentTags[i], out type, out subtype);
                        if (type == UFConstants.UF_solid_type)
                        {
                            theUFSession.Modl.AskEdgeBody(parentTags[i], out bodyTag);
                            parentBody = NXObjectManager.Get(bodyTag) as Body;
                            break;
                        }
                    }
                    break;
                }

            }

            return parentBody;
        }
    }
}
