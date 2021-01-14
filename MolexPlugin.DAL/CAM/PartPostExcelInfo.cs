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
    /// 
    /// </summary>
    public class PartPostExcelInfo
    {
        private Part pt;

        public PartPostExcelInfo(Part pt)
        {
            this.pt = pt;
        }
        /// <summary>
        /// 获取模具号
        /// </summary>
        /// <returns></returns>
        public MoldInfo GetMoldInfo()
        {
            if (ParentAssmblieInfo.IsParent(this.pt))
            {
                return MoldInfo.GetAttribute(this.pt);
            }
            else
            {
                return new MoldInfo()
                {
                    ClientName = "",
                    EditionNumber = "",
                    MachineType = "",
                    MoldNumber = "",
                    MoldType = "",
                    WorkpieceNumber = ""

                };
            }
        }
        /// <summary>
        /// 判断是否是工件
        /// </summary>
        /// <returns></returns>
        public bool IsWorkpiece()
        {
            return ParentAssmblieInfo.IsWorkpiece(this.pt)|| !ParentAssmblieInfo.IsParent(this.pt);
        }
        /// <summary>
        /// 获取电极号
        /// </summary>
        /// <returns></returns>
        public string GetEleName()
        {
            if (ParentAssmblieInfo.IsElectrode(this.pt))
            {
                ElectrodeNameInfo name = ElectrodeNameInfo.GetAttribute(this.pt);
                if (pt.Name.Equals(name.EleName, StringComparison.CurrentCultureIgnoreCase))
                    return name.EleName;
            }
            return pt.Name;
        }
        /// <summary>
        /// 查询是否扣间隙
        /// </summary>
        /// <returns></returns>
        public bool AskEleIsOffset()
        {
            int offset = AttributeUtils.GetAttrForInt(this.pt, "MdblsShrinkBody");
            if (offset == 0)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="user"></param>
        /// <param name="date"></param>
        public void GetUserInfo(out string user, out string date)
        {
            date = AttributeUtils.GetAttrForString(this.pt, "CreatedCAMDate");
            user = AttributeUtils.GetAttrForString(this.pt, "CreatorCAMName");
            if (date == "")
                date = DateTime.Now.ToString("yyyy-MM-dd");
            if (user == "")
                user = Environment.UserName;//获取电脑用户名
        }
        /// <summary>
        /// 获取间隙
        /// </summary>
        /// <returns></returns>
        public ElectrodeGapValueInfo GetGapValueInfo()
        {
            if (ParentAssmblieInfo.IsElectrode(this.pt))
            {
                return ElectrodeGapValueInfo.GetAttribute(this.pt);
            }
            else
            {
                return new ElectrodeGapValueInfo()
                {
                    CrudeInter = 0,
                    CrudeNum = 0,
                    DuringInter = 0,
                    DuringNum = 0,
                    FineInter = 0,
                    FineNum = 0
                };
            }
        }
    }
}