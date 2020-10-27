using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.Utilities;
using Basic;
using MolexPlugin.Model;
using NXOpen.UF;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 体扣间隙
    /// </summary>
    public class OffsetBodyGapVaule
    {
        private Part pt;
        private UserModel user;
        public OffsetBodyGapVaule(Part pt, UserModel user)
        {
            this.pt = pt;
            this.user = user;
        }
        public void SetAttribute(bool inters)
        {
            AttributeUtils.AttributeOperation("CreatedCAMDate", user.CreatedDate, pt);
            AttributeUtils.AttributeOperation("CreatorCAMName", user.CreatorName, pt);
            if (inters)
            {
                AttributeUtils.AttributeOperation("MdblsShrinkBody", "0", pt);
            }
            else
                AttributeUtils.AttributeOperation("MdblsShrinkBody", "1", pt);
        }
        /// <summary>
        /// 偏置电极间隙
        /// </summary>
        /// <param name="part"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        public bool SetOffsetInter(List<Face> faces, double side)
        {

            string mb = AttributeUtils.GetAttrForString(pt, "MdblsShrinkBody");
            if (mb.Equals("0", StringComparison.CurrentCultureIgnoreCase))
                return true;
            if (mb.Equals("1", StringComparison.CurrentCultureIgnoreCase))
                return false;
            bool isok = false;
            UFSession theUFSession = UFSession.GetUFSession();

            List<Tag> featureTags = new List<Tag>();
            Tag groupTag;
            foreach (NXOpen.Features.Feature fe in pt.Features)
            {
                featureTags.Add(fe.Tag);
            }
            theUFSession.Modl.CreateSetOfFeature("电极特征", featureTags.ToArray(), featureTags.Count, 1, out groupTag);
            try
            {
                NXObject obj = OffsetRegionUtils.Offset(-side, out isok, faces.ToArray());
                if (isok)
                {
                    obj.SetName((-side).ToString());

                }
                this.SetAttribute(isok);
                return isok;
            }
            catch
            {
                isok = false;
                this.SetAttribute(isok);
                return isok;
            }
        }
        public bool SetOffsetInter(List<Face> ErFace, double ErSide, List<Face> EfFace, double EfSide)
        {

            string mb = AttributeUtils.GetAttrForString(pt, "MdblsShrinkBody");
            if (mb.Equals("0", StringComparison.CurrentCultureIgnoreCase))
                return true;
            if (mb.Equals("1", StringComparison.CurrentCultureIgnoreCase))
                return false;
            bool isokEr = false;
            bool isokEf = false;
            UFSession theUFSession = UFSession.GetUFSession();
            List<Tag> featureTags = new List<Tag>();
            Tag groupTag;
            foreach (NXOpen.Features.Feature fe in pt.Features)
            {
                featureTags.Add(fe.Tag);
            }
            theUFSession.Modl.CreateSetOfFeature("电极特征", featureTags.ToArray(), featureTags.Count, 1, out groupTag);
            NXObject objEr = null;
            try
            {
                objEr = OffsetRegionUtils.Offset(-ErSide, out isokEr, ErFace.ToArray());
                if (isokEr)
                {
                    objEr.SetName((-ErSide).ToString());
                }

            }
            catch
            {
                this.SetAttribute(true);
                return false;
            }
            try
            {
                NXObject obj = OffsetRegionUtils.Offset(-EfSide, out isokEf, EfFace.ToArray());
                if (isokEf)
                {
                    obj.SetName((-EfSide).ToString());
                }
                this.SetAttribute(true);
                return true;
            }
            catch
            {
                if (objEr != null)
                    DeleteObject.Delete(objEr);
                this.SetAttribute(false);
                return false;
            }

        }
    }
}

