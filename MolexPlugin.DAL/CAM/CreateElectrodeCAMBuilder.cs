﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;
using MolexPlugin.Model;
using NXOpen.CAM;
using Basic;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 创建电极程序
    /// </summary>
    public class CreateElectrodeCAMBuilder
    {
        private AbstractElectrodeCAM cam = null;
        private AbstractElectrodeTemplate template = null;
        private Part pt = null;
        public Part Pt { get { return pt; } }
        public AbstractElectrodeTemplate Template { get { return template; } }
        public CreateElectrodeCAMBuilder(Part pt, UserModel user)
        {
            this.pt = pt;
            try
            {
                cam = ElectrodeCAMFactory.CreateCAM(pt, user);
            }
            catch (Exception ex)
            {
                throw ex;
            }
          

        }
        /// <summary>
        /// 计算刀路
        /// </summary>
        public bool SetGenerateToolPath(bool isGenerate)
        {
            if (isGenerate && cam.IsCompute)
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                try
                {
                    theSession.ApplicationSwitchImmediate("UG_APP_MANUFACTURING");
                    NXOpen.CAM.NCGroup nCGroup1 = (NXOpen.CAM.NCGroup)workPart.CAMSetup.CAMGroupCollection.FindObject("AAA");
                    workPart.CAMSetup.GenerateToolPath(new CAMObject[1] { nCGroup1 });
                    pt.Save(BasePart.SaveComponents.False, BasePart.CloseAfterSave.False);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        ///  创建加工名字
        /// </summary>
        public void CreateOperationNameModel(ElectrodeTemplate type)
        {
            try
            {
                CompterToolName tool = cam.GetTool();
                template = ElectrodeTemplateFactory.CreateOperation(type, tool);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            template.CreateProgramName();
        }

        /// <summary>
        /// 创建加工操作
        /// </summary>
        public List<string> CreateOperation()
        {
            List<string> err = new List<string>();
            Session theSession = Session.GetSession();
            UFSession theUFSession = UFSession.GetUFSession();
            PartUtils.SetPartDisplay(pt);
            theSession.ApplicationSwitchImmediate("UG_APP_MODELING");
            cam.CreateOffsetInter();
            theSession.ApplicationSwitchImmediate("UG_APP_MANUFACTURING");
            try
            {
                CreateCamSetup();
            }
            catch (NXException ex)
            {
                err.Add("进入加工环境错误！请检查！                     " + ex.Message);
                return err;
            }
            try
            {
                SetWorkpiece();
            }
            catch (NXException ex)
            {
                err.Add("自动选择加工体错误！请检查加工体！                     " + ex.Message);
            }
            foreach (ProgramOperationName pm in this.template.Programs)
            {
                err.AddRange(pm.CreateOperation(cam));
            }
          //  theUFSession.UiOnt.ExpandView();
            return err;

        }

        public List<string> CreateOperationExe()
        {
            List<string> err = new List<string>();
            Session theSession = Session.GetSession();
            PartUtils.SetPartDisplay(pt);
            theSession.ApplicationSwitchImmediate("UG_APP_MODELING");
            cam.CreateOffsetInter();          
            try
            {
                CreateSetupForUF();
            }
            catch (NXException ex)
            {
                err.Add("进入加工环境错误！请检查！                     " + ex.Message);
                return err;
            }
            try
            {
                SetWorkpiece();
            }
            catch (NXException ex)
            {
                err.Add("自动选择加工体错误！请检查加工体！                     " + ex.Message);
            }
            foreach (ProgramOperationName pm in this.template.Programs)
            {
                err.AddRange(pm.CreateOperation(cam));
            }
            return err;

        }
        /// <summary>
        /// 创建用户定义刀路
        /// </summary>
        /// <returns></returns>
        public List<string> CreateUserOperation()
        {
            List<string> err = new List<string>();
            Session theSession = Session.GetSession();
            PartUtils.SetPartDisplay(pt);
            theSession.ApplicationSwitchImmediate("UG_APP_MODELING");
            cam.CreateOffsetInter();
            theSession.ApplicationSwitchImmediate("UG_APP_MANUFACTURING");
            if (!this.isProgram())
            {
                try
                {
                    CreateCamSetup();
                }
                catch (NXException ex)
                {
                    err.Add("进入加工环境错误！请检查！                     " + ex.Message);
                    return err;
                }
                try
                {
                    SetWorkpiece();
                }
                catch (NXException ex)
                {
                    err.Add("自动选择加工体错误！请检查加工体！                     " + ex.Message);
                }
                foreach (ProgramOperationName pm in this.template.Programs)
                {
                    err.AddRange(pm.CreateUserOperation(cam));
                }
            }
            return err;

        }
        /// <summary>
        /// 设置加工环境
        /// </summary>
        private void CreateCamSetup()
        {
            Session theSession = Session.GetSession();
            UFSession theUFSession = UFSession.GetUFSession();
            Part workPart = theSession.Parts.Work;
            bool result1;
            result1 = theSession.IsCamSessionInitialized();
            theSession.CreateCamSession();
            NXOpen.CAM.CAMSetup cAMSetup1;
            cAMSetup1 = workPart.CreateCamSetup("electrode");            
        }
        private void CreateSetupForUF()
        {
            UFSession theUFSession = UFSession.GetUFSession();
            Session theSession = Session.GetSession();
            theSession.ApplicationSwitchImmediate("UG_APP_MANUFACTURING");
            Part workPart = theSession.Parts.Work;
            bool result1;
            result1 = theSession.IsCamSessionInitialized();
            theSession.CreateCamSession();
            theUFSession.Cam.OptAddTemplatePart(@"S:\NXAPS\8.5\PROD\CAM\tool\MolexPlugIn-1899\resource\template_part\metric\electrode.prt");
            theUFSession.Setup.Create(workPart.Tag, "electrode");
        }
        private bool isProgram()
        {
            try
            {
                if (this.pt.CAMSetup != null)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 设置加工体
        /// </summary>
        private void SetWorkpiece()
        {
            CAMUtils.SetFeatureGeometry("WORKPIECE", this.pt.Bodies.ToArray());
        }

        public List<string> ExportFile(string filePath, bool open)
        {
            List<string> err = new List<string>();
            string name = pt.Name;
            try
            {
                if (cam.CreateNewFile(filePath + "\\", open))
                    err.Add(name + "        电极程序创建成功");
                else
                {
                    err.Add(name + "        电极创建失败");
                }
            }
            catch (Exception ex)
            {
                err.Add(name + "              " + ex.Message);
                err.Add(name + "        电极创建失败");
            }

            return err;
        }

        public ElectrodeTemplate GetElectrodeTemplate()
        {
            return this.cam.GetElectrodeTemplate();
        }
    }
}
