using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.Features;

namespace Basic
{
    public class PatternUtils : ClassItem
    {

        /// <summary>
        /// 阵列几何特征
        /// </summary>
        /// <param name="obj">阵列对象</param>
        /// <param name="xNCopies">x方向个数</param>
        /// <param name="xPitchDistance">x方向尺寸</param>
        /// <param name="yNCopies">Y方向个数</param>
        /// <param name="yPitchDistance">Y方向尺寸</param>
        /// <returns></returns>
        public static PatternGeometry CreatePattern(string xNCopies, string xPitchDistance, string yNCopies, string yPitchDistance, Matrix4 mat = null, params DisplayableObject[] obj)
        {
            Part workPart = theSession.Parts.Work;
            if (mat == null)
            {
                mat = new Matrix4();
                mat.Identity();
                mat.TransformToCsys(workPart.WCS.CoordinateSystem, ref mat);
            }
            NXOpen.Features.PatternGeometry nullNXOpen_Features_PatternGeometry = null;
            NXOpen.Features.PatternGeometryBuilder patternGeometryBuilder1;
            patternGeometryBuilder1 = workPart.Features.CreatePatternGeometryBuilder(nullNXOpen_Features_PatternGeometry);
            Direction xDirection;
            Direction yDirection;
            Vector3d x = mat.GetXAxis();
            Vector3d y = mat.GetYAxis();
            Point3d origin = mat.GetCenter();
            bool added1 = patternGeometryBuilder1.GeometryToPattern.Add(obj); //设置要阵列的对象

            patternGeometryBuilder1.PatternService.RectangularDefinition.UseYDirectionToggle = true;

            patternGeometryBuilder1.ReferencePoint.Point = workPart.Points.CreatePoint(origin); //指定参考点

            xDirection = workPart.Directions.CreateDirection(origin, x, SmartObject.UpdateOption.WithinModeling);   //方向
            yDirection = workPart.Directions.CreateDirection(origin, y, SmartObject.UpdateOption.WithinModeling);   //方向
            patternGeometryBuilder1.PatternService.RectangularDefinition.XDirection = xDirection;
            patternGeometryBuilder1.PatternService.RectangularDefinition.YDirection = yDirection;
            patternGeometryBuilder1.PatternService.RectangularDefinition.XSpacing.NCopies.RightHandSide = xNCopies;  //要阵列的个数（包括本身）                                                                                                                     
            patternGeometryBuilder1.PatternService.RectangularDefinition.XSpacing.PitchDistance.RightHandSide = xPitchDistance; //设置节距                                                                                                                           
            patternGeometryBuilder1.PatternService.RectangularDefinition.YSpacing.NCopies.RightHandSide = yNCopies;  //要阵列的个数（包括本身）
            patternGeometryBuilder1.PatternService.RectangularDefinition.YSpacing.PitchDistance.RightHandSide = yPitchDistance; //设置节距
            try
            {

                return patternGeometryBuilder1.CommitFeature() as PatternGeometry;

            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("PatternUtils:CreatePattern:      " + ex.Message);
                throw ex;
            }
            finally
            {
                patternGeometryBuilder1.Destroy();
            }

        }

        public static PatternGeometry CreatePattern(int xNCopies, double xPitchDistance, int yNCopies, double yPitchDistance, Matrix4 mat = null, PatternGeometry features = null, params DisplayableObject[] obj)
        {
            Part workPart = theSession.Parts.Work;
            if (mat == null)
            {
                mat = new Matrix4();
                mat.Identity();
                mat.TransformToCsys(workPart.WCS.CoordinateSystem, ref mat);
            }
            NXOpen.Features.PatternGeometryBuilder patternGeometryBuilder1;
            patternGeometryBuilder1 = workPart.Features.CreatePatternGeometryBuilder(features);
            Direction xDirection;
            Direction yDirection;
            Vector3d x = mat.GetXAxis();
            Vector3d y = mat.GetYAxis();
            Point3d origin = mat.GetCenter();
            bool added1 = patternGeometryBuilder1.GeometryToPattern.Add(obj); //设置要阵列的对象

            patternGeometryBuilder1.PatternService.RectangularDefinition.UseYDirectionToggle = true;

            patternGeometryBuilder1.ReferencePoint.Point = workPart.Points.CreatePoint(origin); //指定参考点

            xDirection = workPart.Directions.CreateDirection(origin, x, SmartObject.UpdateOption.WithinModeling);   //方向
            yDirection = workPart.Directions.CreateDirection(origin, y, SmartObject.UpdateOption.WithinModeling);   //方向
            patternGeometryBuilder1.PatternService.RectangularDefinition.XDirection = xDirection;
            patternGeometryBuilder1.PatternService.RectangularDefinition.YDirection = yDirection;
            patternGeometryBuilder1.PatternService.RectangularDefinition.XSpacing.NCopies.Value = xNCopies;  //要阵列的个数（包括本身）                                                                                                                     
            patternGeometryBuilder1.PatternService.RectangularDefinition.XSpacing.PitchDistance.Value = xPitchDistance; //设置节距                                                                                                                           
            patternGeometryBuilder1.PatternService.RectangularDefinition.YSpacing.NCopies.Value = yNCopies;  //要阵列的个数（包括本身）
            patternGeometryBuilder1.PatternService.RectangularDefinition.YSpacing.PitchDistance.Value = yPitchDistance; //设置节距
            NXOpen.Session.UndoMarkId markId = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Start ToolingBox");
            try
            {
                return patternGeometryBuilder1.CommitFeature() as PatternGeometry;
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("PatternUtils:CreatePattern:      " + ex.Message);
                throw ex;
            }
            finally
            {
                patternGeometryBuilder1.Destroy();
                theSession.UpdateManager.DoUpdate(markId);
                theSession.DeleteUndoMark(markId, "End ToolingBox");
            }

        }
    }
}
