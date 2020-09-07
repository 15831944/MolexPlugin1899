using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.Utilities;
using NXOpen.UF;

namespace Basic
{
    /// <summary>
    /// 抽取面
    /// </summary>
    public class ExtraetUtils : ClassItem
    {
        /// <summary>
        /// 抽取面
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public static Body ExtraetFace(Face face)
        {
            Tag sheetBody = Tag.Null;
            try
            {
                theUFSession.Modl.ExtractFace(face.Tag, 0, out sheetBody);
                return NXObjectManager.Get(sheetBody) as Body;
            }
            catch (NXException ex)
            {
               // LogMgr.WriteLog("ExtraetUtils:ExtraetFace          " + ex.Message);
                throw ex;
            }
        }
        /// <summary>
        ///  创建面链
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public static List<Body> ExtractFaceBuilder(params Face[] face)
        {
            Part workPart = theSession.Parts.Work;
            NXOpen.Features.ExtractFaceBuilder extractFaceBuilder1;
            extractFaceBuilder1 = workPart.Features.CreateExtractFaceBuilder(null);
            extractFaceBuilder1.FaceOption = NXOpen.Features.ExtractFaceBuilder.FaceOptionType.FaceChain;
            extractFaceBuilder1.FeatureOption = NXOpen.Features.ExtractFaceBuilder.FeatureOptionType.SeparateFeatureForEachBody;
            extractFaceBuilder1.ParentPart = NXOpen.Features.ExtractFaceBuilder.ParentPartType.WorkPart;
            extractFaceBuilder1.Associative = true;
            extractFaceBuilder1.FixAtCurrentTimestamp = false;
            extractFaceBuilder1.HideOriginal = false;
            extractFaceBuilder1.DeleteHoles = false;
            extractFaceBuilder1.InheritDisplayProperties = false;
            extractFaceBuilder1.Type = NXOpen.Features.ExtractFaceBuilder.ExtractType.Face;
            ISelectionRule rule = new SelectionFaceRule(face.ToList());
            NXOpen.SelectionIntentRule[] rules = { rule.CreateSelectionRule() };
            extractFaceBuilder1.FaceChain.ReplaceRules(rules, false);
            try
            {
                return extractFaceBuilder1.CommitFeature().GetBodies().ToList();
            }
            catch (NXException ex)
            {
              //  LogMgr.WriteLog("ExtractFaceBuilder:ExtraetFace          " + ex.Message);
                throw ex;
            }
            finally
            {
                extractFaceBuilder1.Destroy();
            }

        }
    }
}
