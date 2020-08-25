using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using MolexPlugin.Model;


namespace MolexPlugin.DAL
{
    /// <summary>
    /// 用户单列
    /// </summary>
    public class UserSingleton
    {
        private static UserSingleton user = null;
        private Jurisdiction jurisd = null;
        private UserModel creatorUser = null;
        private UserInfo info;
        /// <summary>
        /// 获取是否成功
        /// </summary>
        public bool UserSucceed { get; private set; } = true;
        /// <summary>
        /// 创建用户
        /// </summary>
        public UserModel CreatorUser
        {
            get
            {
                if (UserSucceed && creatorUser == null)
                {
                    creatorUser = new UserModel()
                    {
                        CreateDate = DateTime.Now.ToString("yyyy-MM-dd"),
                        CreatorName = info.UserName
                    };
                }
                return creatorUser;
            }
        }
        /// <summary>
        /// 权限
        /// </summary>
        public Jurisdiction Jurisd
        {
            get
            {
                if (UserSucceed && jurisd == null)
                    jurisd = new Jurisdiction(info);
                return jurisd;
            }
        }

        private UserSingleton()
        {
            this.info = UserInfoDeserialize.GetDeserializeToUser();
            if (this.info == null)
            {
                UserSucceed = false;
            }
        }
        public static UserSingleton Instance()
        {
            if (user == null)
                user = new UserSingleton();
            return user;
        }

    }
}
