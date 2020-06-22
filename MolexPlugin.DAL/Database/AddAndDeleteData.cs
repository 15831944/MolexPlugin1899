using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    public class AddAndDeleteData
    {
        private UserSingleton users;
        private bool auth;
        private OperationData oper = new OperationData();
        public AddAndDeleteData()
        {
            this.users = UserSingleton.Instance();
            this.auth = AuthenticatedUser.GetAuthenticated();
        }
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool DeleteUser(params UserInfo[] user)
        {
            if (auth && users.Jurisd.GetAdminJurisd())
            {
                bool isok = oper.DeleteDatabaseToUser(user);
                if (isok)
                {
                    oper.Serialize("SerializeUser");
                }
                return isok;
            }
            else
                return false;
        }
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool AddUser(params UserInfo[] user)
        {
            if (auth && users.Jurisd.GetAdminJurisd())
            {
                bool isok = oper.AddDatabaseToUser(user);
                if (isok)
                {
                    oper.Serialize("SerializeUser");
                }
                return isok;
            }
            else
                return false;
        }
        /// <summary>
        /// 删除控件
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool DeleteControl(params ControlEnum[] ce)
        {
            if (auth && users.Jurisd.GetAdminJurisd())
            {
                bool isok = oper.DeleteDatabaseToControl(ce);
                if (isok)
                {
                    oper.Serialize("SerializeUser");
                }
                return isok;
            }
            else
                return false;
        }
        /// <summary>
        /// 添加控件
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool AddControl(params ControlEnum[] ce)
        {
            if (auth && users.Jurisd.GetAdminJurisd())
            {
                bool isok = oper.AddDatabaseToControl(ce);
                if (isok)
                {
                    oper.Serialize("SerializeUser");
                }
                return isok;
            }
            else
                return false;
        }
    }
}
