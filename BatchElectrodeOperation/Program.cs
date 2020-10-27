using System;
using NXOpen;
using NXOpen.Utilities;
using NXOpen.UF;
using Basic;
using MolexPlugin.DAL;
using MolexPlugin.Model;

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
                    CreateBuider(st);
                }

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
            }
            return ElectrodeTemplate.User;
        }

        private static bool CreateBuider(string partPath)
        {
            Tag ptTag = Tag.Null;
            UFPart.LoadStatus err;
            theUfSession.Part.Open(partPath, out ptTag, out err);
            Part pt = NXObjectManager.Get(ptTag) as Part;
            if (pt != null)
            {
                UserSingleton user = UserSingleton.Instance();
                if (user.UserSucceed && user.Jurisd.GetCAMJurisd())
                {
                    ElectrodeCAMInfo cam = ElectrodeCAMInfo.GetAttribute(pt);
                    CreateElectrodeCAMBuilder builder = new CreateElectrodeCAMBuilder(pt, user.CreatorUser, GetTemplate(cam.CamTemplate));
                    builder.CreateOperationNameModel();
                    builder.CreateOperation();
                    ElectrodeCAMFile file = new ElectrodeCAMFile();
                    string path = file.GetSaveFilePath();
                    if (path != null && path != "")
                    {
                        builder.ExportFile(path, false);
                    }
                    else
                    {

                        builder.ExportFile("C:\\temp\\Electrode\\", false);
                    }
                }
                return true;
            }
            return false;
        }

    }
}

