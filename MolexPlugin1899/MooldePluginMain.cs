using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.Utilities;
using NXOpen.UF;
using MolexPlugin;
using MolexPlugin.DAL;

namespace MolexPlugin
{
    public class MooldePluginMain
    {
        private static Session theSession;
        private static NXOpen.UI theUI;
        private static UFSession theUfSession;
        public static MooldePluginMain theMooldePluginMain;
        public static bool isDisposeCalled;

        public MooldePluginMain()
        {
            try
            {
                theSession = Session.GetSession();
                theUI = NXOpen.UI.GetUI();
                theUfSession = UFSession.GetUFSession();
                isDisposeCalled = false;
            }
            catch (NXOpen.NXException ex)
            {
                // ---- Enter your exception handling code here -----
                NXOpen.UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, ex.Message);
            }
        }
        public static int Main(string[] args)
        {

            #region  公共工具
            if (args[0] == "MENU_MoveObject")
            {
                MoveObject move = new MoveObject();
                move.Show();
            }
            if (args[0] == "MENU_MoveComponent")
            {
                MoveComponent move = new MoveComponent();
                move.Show();
            }

            if (args[0] == "MENU_MoveObjectMin")
            {
                VisibleObjects vis = new VisibleObjects();
                Point3d endPt = new Point3d(vis.Aoo.CenterPt.X, vis.Aoo.CenterPt.Y, vis.Aoo.CenterPt.Z - vis.Aoo.DisPt.Z);
                IMoveBulider move = new MovePointToPointBuilder(new Point3d(0, 0, 0), endPt);
                move.Move(vis.VisObjs.ToArray());
            }
            if (args[0] == "MENU_MoveObjectMax")
            {
                VisibleObjects vis = new VisibleObjects();
                Point3d endPt = new Point3d(vis.Aoo.CenterPt.X, vis.Aoo.CenterPt.Y, vis.Aoo.CenterPt.Z + vis.Aoo.DisPt.Z);
                IMoveBulider move = new MovePointToPointBuilder(new Point3d(0, 0, 0), endPt);
                move.Move(vis.VisObjs.ToArray());
            }
            if (args[0] == "MENU_MoveObjectRotateX")
            {
                VisibleObjects vis = new VisibleObjects();
                IMoveBulider move = new MoveRotateBuilder(new Vector3d(1, 0, 0), 90);
                move.Move(vis.VisObjs.ToArray());
            }
            if (args[0] == "MENU_MoveObjectRotateY")
            {
                VisibleObjects vis = new VisibleObjects();
                IMoveBulider move = new MoveRotateBuilder(new Vector3d(0, 1, 0), 90);
                move.Move(vis.VisObjs.ToArray());
            }
            if (args[0] == "MENU_MoveObjectRotateZ")
            {
                VisibleObjects vis = new VisibleObjects();
                IMoveBulider move = new MoveRotateBuilder(new Vector3d(0, 0, 1), 90);
                move.Move(vis.VisObjs.ToArray());
            }
            if (args[0] == "MENU_AnalyzeBodyAndFace")
            {
                AnalyzeBodyAndFace analyze = new AnalyzeBodyAndFace();
                analyze.Show();
            }

            #endregion

            #region 电极设计
            if (args[0] == "MENU_SuperBox")
            {
                SuperBox superBox = new SuperBox();
                superBox.Show();
            }

            if (args[0] == "MENU_AddEdmAsm")
            {
                AddEdmAsm add = new AddEdmAsm();
                add.Show();
            }
            if (args[0] == "MENU_AddWorkpiece")
            {
                AddWorkpiece add = new AddWorkpiece();
                add.Show();
            }

            if (args[0] == "MENU_AddWork")
            {
                AddWork add = new AddWork();
                add.Show();
            }
            if (args[0] == "MENU_CheckElectrode")
            {
                CheckElectrode check = new CheckElectrode();
                check.Show();
            }
            if (args[0] == "MENU_MoveBody")
            {
                MoveBody move = new MoveBody();
                move.Show();
            }

            if (args[0] == "MENU_EleStandardSeatZ+")
            {
                ElectrodeCreateExpAndMatr expAndMatr = new ElectrodeCreateExpAndMatr(new ZPositiveElectrodeMatrix());
                CreateEleStandardSeatForm form = new CreateEleStandardSeatForm(expAndMatr);
                form.Show();

            }
            if (args[0] == "MENU_EleStandardSeatX+")
            {
                ElectrodeCreateExpAndMatr expAndMatr = new ElectrodeCreateExpAndMatr(new XNegativeElectrodeMatrix());
                CreateEleStandardSeatForm form = new CreateEleStandardSeatForm(expAndMatr);
                form.Show();

            }
            if (args[0] == "MENU_EleStandardSeatY+")
            {
                ElectrodeCreateExpAndMatr expAndMatr = new ElectrodeCreateExpAndMatr(new YPositiveElectrodeMatrix());
                CreateEleStandardSeatForm form = new CreateEleStandardSeatForm(expAndMatr);
                form.Show();

            }
            if (args[0] == "MENU_EleStandardSeatX-")
            {
                ElectrodeCreateExpAndMatr expAndMatr = new ElectrodeCreateExpAndMatr(new XPositiveElectrodeMatrix());
                CreateEleStandardSeatForm form = new CreateEleStandardSeatForm(expAndMatr);
                form.Show();

            }
            if (args[0] == "MENU_EleStandardSeatY-")
            {
                ElectrodeCreateExpAndMatr expAndMatr = new ElectrodeCreateExpAndMatr(new YNegativeElectrodeMatrix());
                CreateEleStandardSeatForm form = new CreateEleStandardSeatForm(expAndMatr);
                form.Show();
            }

            if (args[0] == "MENU_ElectrodeColor")
            {
                ElectrodeColor color = new ElectrodeColor();
                color.Show();
            }
            
            if (args[0] == "MENU_DeleteEle")
            {

                DeleteEle delete = new DeleteEle();
                delete.Show();
            }
            if (args[0] == "MENU_CopyElectrode")
            {
                CopyElectrode copy = new CopyElectrode();
                copy.Show();
            }
            if (args[0] == "MENU_PositionEle")
            {
                PositionEle posit = new PositionEle();
                posit.Show();
            }
            if (args[0] == "MENU_MoovElectrode")
            {
                new MoovElectrode().Show();
            }

            if (args[0] == "MENU_Interference")
            {
                Interference inter = new Interference();
                inter.Show();
            }

            if (args[0] == "MENU_WorkpieceDrawing")
            {
                new WorkpieceDrawingCreateForm().Show();
            }

            if (args[0] == "MENU_ElectrodeDrawing")
            {
                new ElectrodeDrawingCreateForm().Show();
            }

            if (args[0] == "MENU_Bom")
            {
                new BomCreateForm().Show();
            }
            if (args[0] == "MENU_AlterComponent")
            {
                new AlterComponent().Show();
            }
            if (args[0] == "MENU_CopyAsm")
            {
                new CopyAsm().Show();
            }
            if (args[0] == "MENU_ExportEleCam")
            {
                new ExportEleCamCreateForm().Show();
            }
            if (args[0] == "MENU_ExportEleCMM")
            {
                new ExportEleCMMCreateForm().Show();
            }
            #endregion

            if (args[0] == "MENU_EleProgram")
            {
                EleProgramCreateForm mode = new EleProgramCreateForm();
                mode.Show();
            }
            if (args[0] == "MENU_AddProgram")
            {
                AddProgram ele = new AddProgram();
                ele.Show();
            }

            if (args[0] == "MENU_Program")
            {
                UserOperation.CreateUserOper();
            }
            if (args[0] == "MENU_OffsetBodyGapVauleUi")
            {
                new OffsetBodyGapVauleUi().Show();
            }

            if (args[0] == "MENU_PostShopdoc")
            {
                PostShopdocCreateForm.Show();
            }

           //  Test.User();
            return 1;
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

        /// <summary>
        /// 卸载函数
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static int GetUnloadOption(string arg)
        {
            // return System.Convert.ToInt32(Session.LibraryUnloadOption.Explicitly);
            return System.Convert.ToInt32(Session.LibraryUnloadOption.Immediately);
            // return System.Convert.ToInt32(Session.LibraryUnloadOption.AtTermination);
        }

    }
}
