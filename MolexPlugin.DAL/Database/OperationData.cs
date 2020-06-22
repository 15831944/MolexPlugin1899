using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MolexPlugin.Model;
using MolexPlugin.DLL;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 操作数据库
    /// </summary>
    public class OperationData
    {

        public OperationData()
        {

        }

        /// <summary>
        /// 通过数据库获得获得用户
        /// </summary>
        /// <returns></returns>
        public UserInfo GetDatabaseToUser()
        {
            string userAccount = Environment.UserName;//获取电脑用户名
            return new UserInfoDll().GetEntity(userAccount);
        }
        /// <summary>
        /// 添加用户数据
        /// </summary>
        /// <param name="infos"></param>
        /// <returns></returns>
        public bool AddDatabaseToUser(params UserInfo[] infos)
        {
            UserInfoDll user = new UserInfoDll();
            int cout = user.Insert(infos.ToList());
            if (cout == infos.Length)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 删除用户数据
        /// </summary>
        /// <param name="infos"></param>
        /// <returns></returns>
        public bool DeleteDatabaseToUser(params UserInfo[] infos)
        {
            UserInfoDll user = new UserInfoDll();
            int cout = user.Delete(infos.ToList());
            if (cout == infos.Length)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 添加控件数据
        /// </summary>
        /// <param name="infos"></param>
        /// <returns></returns>
        public bool AddDatabaseToControl(params ControlEnum[] ce)
        {
            ControlEnumNameDll control = new ControlEnumNameDll();
            int cout = control.Insert(ce.ToList());
            if (cout == ce.Length)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 删除控件数据
        /// </summary>
        /// <param name="infos"></param>
        /// <returns></returns>
        public bool DeleteDatabaseToControl(params ControlEnum[] ce)
        {
            ControlEnumNameDll control = new ControlEnumNameDll();
            int cout = control.Delete(ce.ToList());
            if (cout == ce.Length)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 序列化
        /// </summary>
        public void Serialize(string name)
        {
            string dllPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string userPath = dllPath.Replace("application\\", "Cofigure\\" + name + "dat");
            if (File.Exists(userPath))
                File.Delete(userPath);
            List<UserInfo> users = new UserInfoDll().GetList();
            FileStream fs = new FileStream(userPath, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, users);
            fs.Close();
        }
    }
}
