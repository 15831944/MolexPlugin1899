using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MolexPlugin.Model;
using Basic;

namespace MolexPlugin.DAL
{
    public class Jurisdiction
    {
        private UserInfo info;
        private Jurisd jus = 0;
        /// <summary>
        /// 权限
        /// </summary>
        private Jurisd Jus
        {
            get
            {
                if (jus == 0)
                {
                    jus = GetJurisd();
                }
                return jus;
            }
        }

        public Jurisdiction(UserInfo info)
        {
            this.info = info;
        }
        /// <summary>
        /// 获取权限
        /// </summary>
        /// <returns></returns>
        private Jurisd GetJurisd()
        {
            Jurisd jd = Jurisd.Comm;

            bool ele = info.Role.Exists(a => a.RoleName.Equals("Electrode", StringComparison.CurrentCultureIgnoreCase));
            bool cam = info.Role.Exists(a => a.RoleName.Equals("CAM", StringComparison.CurrentCultureIgnoreCase));
            bool admin = info.Role.Exists(a => a.RoleName.Equals("Admin", StringComparison.CurrentCultureIgnoreCase));
            if (admin)
            {
                return  Jurisd.Admin;
               
            }
            if (ele && cam)
            {
                return  Jurisd.ElectrodeAndCAM;

            }
            if (ele)
            {
                return  Jurisd.Electrode;

            }
            if (cam)
            {
              return  Jurisd.CAM;

            }
            return jd;
        }
        /// <summary>
        /// 获取电极设计权限
        /// </summary>
        /// <returns></returns>
        public bool GetElectrodeJurisd()
        {
            if ((this.Jus == Jurisd.Electrode) || (this.Jus == Jurisd.ElectrodeAndCAM) || (this.Jus == Jurisd.Admin))
            {
                return true;
            }
            else
            {
                LogMgr.WriteLog("没有电极设计权限!");
                return false;
            }

        }
        /// <summary>
        /// 获取CAM权限
        /// </summary>
        /// <returns></returns>
        public bool GetCAMJurisd()
        {
            if ((this.Jus == Jurisd.CAM) || (this.Jus == Jurisd.ElectrodeAndCAM) || (this.Jus == Jurisd.Admin))
            {
                return true;
            }
            else
            {
                LogMgr.WriteLog("没有CAM权限!");
                return false;
            }

        }
        /// <summary>
        /// 获取管理员权限
        /// </summary>
        /// <returns></returns>
        public bool GetAdminJurisd()
        {
            if (this.Jus == Jurisd.Admin)
            {
                return true;
            }
            else
            {
                LogMgr.WriteLog("没有管理员权限!");
                return false;
            }

        }
        /// <summary>
        /// 获取公共工具权限
        /// </summary>
        /// <returns></returns>
        public bool GetComm()
        {
            if ((this.Jus == Jurisd.Electrode) || this.Jus == Jurisd.CAM || (this.Jus == Jurisd.ElectrodeAndCAM) || (this.Jus == Jurisd.Admin))
            {
                return true;
            }
            else
            {
                LogMgr.WriteLog("没有公共工件权限!");
                return false;
            }

        }
    }
    public enum Jurisd
    {
        Electrode = 1,
        CAM = 2,
        ElectrodeAndCAM = 3,
        Admin = 4,
        Comm = 5
    }

}
