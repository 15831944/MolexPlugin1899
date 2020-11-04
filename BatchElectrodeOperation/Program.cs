using System;
using NXOpen;
using NXOpen.Utilities;
using NXOpen.UF;
using Basic;
using MolexPlugin.DAL;
using MolexPlugin.Model;
using System.Collections.Generic;
using System.IO;

namespace BatchElectrodeOperation
{

    public class Program
    {
        // class members
        private static Session theSession;
        private static UFSession theUfSession;
        public static Program theProgram;
        public static bool isDisposeCalled;

        //------------------------------------------------------------------------------
        // Constructor
        //------------------------------------------------------------------------------
        public Program()
        {
            try
            {
                theSession = Session.GetSession();
                theUfSession = UFSession.GetUFSession();
                // theUfSession.UF.SetVariable("UGS_LICENSE_BUNDLE", "MILLFOUND");
                theUfSession.UF.SetVariable("UGII_CAM_RESOURCE_DIR", "S:\\NXAPS\\8.5\\PROD\\CAM\\tool\\MolexPlugIn-1899\\resource\\");
                isDisposeCalled = false;
            }
            catch (NXOpen.NXException ex)
            {
                // ---- Enter your exception handling code here -----
                // UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, ex.Message);
            }
        }

        //------------------------------------------------------------------------------
        //  Explicit Activation
        //      This entry point is used to activate the application explicitly
        //------------------------------------------------------------------------------
        public static int Main(string[] args)
        {
            int retValue = 0;
            try
            {
                theProgram = new Program();

                // TODO: Add your application code here
                foreach (string st in args)
                {
                    string name = Path.GetFileNameWithoutExtension(st);
                    Console.WriteLine("�缫" + name + "*************���㿪ʼ��");
                    List<string> tp = CreateBuider(st);
                    foreach (string temp in tp)
                    {
                        Console.WriteLine(temp);
                    }
                }
                //string tra;
                //theUfSession.UF.TranslateVariable("UGII_CAM_RESOURCE_DIR", out tra);
                //string path = Console.ReadLine();
                //CreateBuider(path);
                //Console.Write("123");
                Console.ReadKey();
                theProgram.Dispose();
            }
            catch (NXOpen.NXException ex)
            {
                // ---- Enter your exception handling code here -----

            }
            return retValue;
        }

        //------------------------------------------------------------------------------
        // Following method disposes all the class members
        //------------------------------------------------------------------------------
        public void Dispose()
        {
            try
            {
                if (isDisposeCalled == false)
                {
                    //TODO: Add your application code here 
                }
                isDisposeCalled = true;
            }
            catch (NXOpen.NXException ex)
            {
                // ---- Enter your exception handling code here -----

            }
        }


        private static ElectrodeTemplate GetTemplate(string type)
        {
            if (type != null && type != "")
            {
                if (type.Equals("ֱ�缫"))
                {
                    return ElectrodeTemplate.SimplenessVerticalEleTemplate;
                }
                if (type.Equals("ֱ+�ȸ�"))
                {
                    return ElectrodeTemplate.PlanarAndZleveEleTemplate;
                }
                if (type.Equals("ֱ+�ȿ�"))
                {
                    return ElectrodeTemplate.PlanarAndSufaceEleTemplate;
                }
                if (type.Equals("ֱ+�ȸ�+�ȿ�"))
                {
                    return ElectrodeTemplate.PlanarAndZleveAndSufaceEleTemplate;
                }
                if (type.Equals("�ȿ�+�ȸ�"))
                {
                    return ElectrodeTemplate.ZleveAndSufaceEleTemplate;
                }
                if (type.Equals("�ȸߵ缫"))
                {
                    return ElectrodeTemplate.ZleveEleTemplate;
                }
                if (type.Equals("ģ��"))
                {
                    return ElectrodeTemplate.User;
                }
                else
                {
                    return ElectrodeTemplate.User;
                }
            }
            return ElectrodeTemplate.User;
        }

        private static List<string> CreateBuider(string partPath)
        {
            List<string> err = new List<string>();
            Tag ptTag = Tag.Null;
            UFPart.LoadStatus load;
            ElectrodeCAMFile file = new ElectrodeCAMFile();
            List<string> newPath = file.CopyFile(partPath);
            try
            {
                theUfSession.Part.Open(newPath[0], out ptTag, out load);
            }
            catch (NXException ex)
            {
                err.Add("���ļ�ʧ�ܣ�           " + ex.Message);
                return err;
            }
            Part pt = NXObjectManager.Get(ptTag) as Part;
            string name = pt.Name;
            if (pt != null)
            {
                UserModel user = UserModel.GetAttribute(pt);
                ElectrodeCAMInfo cam = ElectrodeCAMInfo.GetAttribute(pt);
                CreateElectrodeCAMBuilder builder = new CreateElectrodeCAMBuilder(pt, user);
                builder.CreateOperationNameModel(GetTemplate(cam.CamTemplate));
                err.AddRange(builder.CreateOperationExe());
                builder.SetGenerateToolPath(true);
                string path = file.GetSaveFilePath();
                if (path != null && path != "")
                {
                    builder.ExportFile(path, false);
                }
                else
                {
                    builder.ExportFile("C:\\temp\\Electrode\\", false);
                }
                err.Add("�缫" + name + "*************�����ɹ���");
            }
            return err;
        }

    }
}

