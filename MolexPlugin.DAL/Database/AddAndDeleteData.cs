using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MolexPlugin.Model;
using System.IO;
using MolexPlugin.DLL;
using System.Runtime.Serialization.Formatters.Binary;

namespace MolexPlugin.DAL
{
    public class AddAndDeleteData
    {
        private UserSingleton users;
        private bool auth;

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
            if (users.UserSucceed && auth && users.Jurisd.GetAdminJurisd())
            {
                OperationData oper = new OperationData();
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
            if (users.UserSucceed && auth && users.Jurisd.GetAdminJurisd())
            {
                OperationData oper = new OperationData();
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
        /// 序列化控件
        /// </summary>
        public void SerializeUserToData()
        {
            if (users.UserSucceed && auth && users.Jurisd.GetAdminJurisd())
            {
                OperationData oper = new OperationData();
                oper.Serialize("SerializeUser");
            }
        }
        /// <summary>
        /// 删除控件
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool DeleteControl(params ControlEnum[] ce)
        {
            if (users.UserSucceed && auth && users.Jurisd.GetAdminJurisd())
            {
                OperationData oper = new OperationData();
                bool isok = oper.DeleteDatabaseToControl(ce);
                if (isok)
                {
                    SerializeControlToData();
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
            if (users.UserSucceed && auth && users.Jurisd.GetAdminJurisd())
            {
                OperationData oper = new OperationData();
                bool isok = oper.AddDatabaseToControl(ce);
                if (isok)
                {
                    SerializeControlToData();
                }
                return isok;
            }
            else
                return false;
        }
        /// <summary>
        /// 序列化控件
        /// </summary>
        public void SerializeControlToData()
        {
            if (users.UserSucceed && auth && users.Jurisd.GetAdminJurisd())
            {
                string dllPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                string contrPath = dllPath.Replace("application\\", "Cofigure\\SerializeContr.dat");
                if (File.Exists(contrPath))
                    File.Delete(contrPath);
                List<ControlEnum> users = new ControlEnumNameDll().GetList();
                FileStream fs = new FileStream(contrPath, FileMode.Create);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, users);
                fs.Close();
            }
        }
    }
}
