using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basic;
using MolexPlugin.DLL;
using System.IO;

namespace MolexPlugin.DAL
{
    public class AuthenticatedUser
    {
        private static bool Authenticated()
        {
            string adPath = "LDAP://molex.com";
            DirectoryEntry de = new DirectoryEntry(adPath);
            DirectorySearcher deSearch = new DirectorySearcher(de);
            deSearch.Filter = "(&(&(objectCategory=person)(objectClass=user))(sAMAccountName=" +
                "ycchen10" + "))";       // LDAP 查询串
            SearchResult results = deSearch.FindOne();
            if (results != null)
            {
                ResultPropertyCollection coll = results.Properties;
                ResultPropertyValueCollection myCollection = coll["description"];
                if (myCollection.Count == 1)
                {
                    return myCollection[0].Equals("169723");
                }

            }
            return false;

        }
        public static bool GetAuthenticated()
        {
            if (!Authenticated())
            {
                DeleControlData();
                DeleUserData();
                return false;
            }
            return true;
        }
        private static void DeleUserData()
        {
            string dllPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string userPath = dllPath.Replace("application\\", "Cofigure\\SerializeUser.dat");
            if (File.Exists(userPath))
                File.Delete(userPath);
            UserInfoDll user = new UserInfoDll();
            user.Delete(user.GetList());
        }
        private static void DeleControlData()
        {
            string dllPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string contr = dllPath.Replace("application\\", "Cofigure\\SerializeContr.dat");
            if (File.Exists(contr))
                File.Delete(contr);
            ControlEnumNameDll contrdata = new ControlEnumNameDll();
            contrdata.Delete(contrdata.GetList());
        }
    }
}
