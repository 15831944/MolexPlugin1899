using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MolexPlugin.Model;
using MolexPlugin.DLL;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Basic;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 通过序列文件获得User数据
    /// </summary>
    public class UserInfoDeserialize
    {
        private static bool auth = AuthenticatedUser.GetAuthenticated();
        /// <summary>
        /// 通过反序列化获得
        /// </summary>
        /// <returns></returns>
        public static UserInfo GetDeserializeToUser()
        {
            if (!auth)
                return null;
            string userAccount = Environment.UserName;//获取电脑用户名
            List<UserInfo> users = Deserialize();
            if (users == null || users.Count == 0)
            {
                LogMgr.WriteLog("用户反序列化错误");
                return null;
            }
            return users.Find(a => a.UserAccount.Equals(userAccount, StringComparison.CurrentCultureIgnoreCase));
        }


        /// <summary>
        /// 反序列化
        /// </summary>
        /// <returns></returns>
        public static List<UserInfo> Deserialize()
        {
            if (!auth)
                return null;
            string dllPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string userPath = dllPath.Replace("application\\", "Cofigure\\SerializeUser.dat");
            if (File.Exists(userPath))
            {
                FileStream fs = new FileStream(userPath, FileMode.Open, FileAccess.Read);
                BinaryFormatter bf = new BinaryFormatter();
                List<UserInfo> infos = bf.Deserialize(fs) as List<UserInfo>;
                fs.Close();
                return infos;
            }
            return null;
        }
    }
}
