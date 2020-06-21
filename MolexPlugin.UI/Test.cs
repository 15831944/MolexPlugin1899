﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basic;
using System.DirectoryServices;
using NXOpen;
using NXOpen.UF;
using NXOpen.Utilities;

namespace MolexPlugin
{
    public class Test
    {

        public static void cs()
        {
            /*AD
               string adPath = "LDAP://molex.com";
               DirectoryEntry de = new DirectoryEntry(adPath);
               DirectorySearcher deSearch = new DirectorySearcher(de);
               deSearch.Filter = "(&(&(objectCategory=person)(objectClass=user))(sAMAccountName=" +
                   "jyang10" + "))";       // LDAP 查询串
               SearchResult results = deSearch.FindOne();
               LogMgr.WriteLog(results.Path);
               ResultPropertyCollection coll = results.Properties;
               foreach (string str in coll.PropertyNames)
               {
                   string tab = str+"==============";

                   foreach (Object myCollection in coll[str])
                   {
                       LogMgr.WriteLog(tab + myCollection);
                   }
               }
               */
            Tag temp = (Tag)51951; Face face = NXObjectManager.Get(temp) as Face;
            FaceData data = FaceUtils.AskFaceData(face);
            LogMgr.WriteLog(data.FaceType.ToString() + "------------" +face.SolidFaceType);

        }
        public static bool IsAuthenticated(string username, string pwd)
        {
            string adPath = "LDAP://molex.com";
            string domain = "10.73.2.20";
            //  string domainUserName = domain + @"\" + username;      //或者可以这样 
            string domainUserName = username;
            DirectoryEntry entry = new DirectoryEntry(adPath, domainUserName, pwd);
            try
            {
                DirectorySearcher deSearch = new DirectorySearcher(entry);
                deSearch.Filter = "(&(objectCategory=Person)(objectClass=User)(SAMAccountName=" + username + "))";
                deSearch.PropertiesToLoad.Add("cn");
                SearchResult result = deSearch.FindOne();
                if (null == result)
                {
                    return false;
                }
                //可以获取相关信息
                string _path = result.Path;
                string _filterAttribute = (string)result.Properties["cn"][0];
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return true;
        }
    }
}