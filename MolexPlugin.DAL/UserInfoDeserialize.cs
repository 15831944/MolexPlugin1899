using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MolexPlugin.Model;
using MolexPlugin.DLL;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 通过序列文件获得User数据
    /// </summary>
    public class UserInfoDeserialize
    {
        /// <summary>
        /// 通过反序列化获得
        /// </summary>
        /// <returns></returns>
        public static UserInfo GetDeserializeToUser()
        {
            string userAccount = Environment.UserName;//获取电脑用户名
            List<UserInfo> users = Deserialize();
            return users.Find(a => a.UserAccount.Equals(userAccount, StringComparison.CurrentCultureIgnoreCase));
        }

 
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <returns></returns>
        private static List<UserInfo> Deserialize()
        {
            string dllPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string userPath = dllPath.Replace("application\\", "Cofigure\\SerializeUser.dat");
            if (File.Exists(userPath))
            {
                FileStream fs = new FileStream(userPath, FileMode.Open, FileAccess.Read);
                BinaryFormatter bf = new BinaryFormatter();
                return bf.Deserialize(fs) as List<UserInfo>;
            }
            return null;
        }
    }
}
