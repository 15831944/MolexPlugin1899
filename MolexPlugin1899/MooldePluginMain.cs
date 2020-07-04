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
        public static int Main(string[] args)
        {

            #region  公共工具
            if (args[0] == "MENU_MoveObject")
            {
                MoveObject move = new MoveObject();
                move.Show();
            }

            if (args[0] == "MENU_MoveObjectMin")
            {
                MoveVisibleObjects vis = new MoveVisibleObjects();
                Point3d endPt = new Point3d(vis.Aoo.CenterPt.X, vis.Aoo.CenterPt.Y, vis.Aoo.CenterPt.Z - vis.Aoo.DisPt.Z);
                IMoveBulider move = new MovePointToPointBuilder(new Point3d(0, 0, 0), endPt);
                move.Move(vis.VisObjs.ToArray());
            }
            if (args[0] == "MENU_MoveObjectMax")
            {
                MoveVisibleObjects vis = new MoveVisibleObjects();
                Point3d endPt = new Point3d(vis.Aoo.CenterPt.X, vis.Aoo.CenterPt.Y, vis.Aoo.CenterPt.Z + vis.Aoo.DisPt.Z);
                IMoveBulider move = new MovePointToPointBuilder(new Point3d(0, 0, 0), endPt);
                move.Move(vis.VisObjs.ToArray());
            }
            if (args[0] == "MENU_MoveObjectRotateX")
            {
                MoveVisibleObjects vis = new MoveVisibleObjects();
                IMoveBulider move = new MoveRotateBuilder(new Vector3d(1, 0, 0), 90);
                move.Move(vis.VisObjs.ToArray());
            }
            if (args[0] == "MENU_MoveObjectRotateY")
            {
                MoveVisibleObjects vis = new MoveVisibleObjects();
                IMoveBulider move = new MoveRotateBuilder(new Vector3d(0, 1, 0), 90);
                move.Move(vis.VisObjs.ToArray());
            }
            if (args[0] == "MENU_MoveObjectRotateZ")
            {
                MoveVisibleObjects vis = new MoveVisibleObjects();
                IMoveBulider move = new MoveRotateBuilder(new Vector3d(0, 0, 1), 90);
                move.Move(vis.VisObjs.ToArray());
            }
            if (args[0] == "MENU_AnalyzeBodyAndFace")
            {
                AnalyzeBodyAndFace analyze = new AnalyzeBodyAndFace();
                analyze.Show();
            }

            #endregion
            /*
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
            if (args[0] == "MENU_AddWork")
            {
                AddWork add = new AddWork();
                add.Show();
            }
            if (args[0] == "MENU_EleStandardSeatZ+")
            {
                EleStandardSeatCreateForm form = new EleStandardSeatCreateForm("Z+");
                form.Show();

            }
            if (args[0] == "MENU_EleStandardSeatX+")
            {
                EleStandardSeatCreateForm form = new EleStandardSeatCreateForm("X-");
                form.Show();

            }
            if (args[0] == "MENU_EleStandardSeatY+")
            {
                EleStandardSeatCreateForm form = new EleStandardSeatCreateForm("Y+");
                form.Show();

            }
            if (args[0] == "MENU_EleStandardSeatX-")
            {
                EleStandardSeatCreateForm form = new EleStandardSeatCreateForm("X+");
                form.Show();

            }
            if (args[0] == "MENU_EleStandardSeatY-")
            {
                EleStandardSeatCreateForm form = new EleStandardSeatCreateForm("Y-");
                form.Show();

            }
            if (args[0] == "MENU_DeleteEle")
            {

                DeleteEle delete = new DeleteEle();
                delete.Show();
            }
            if (args[0] == "MENU_PositionEle")
            {

                PositionEle posit = new PositionEle();
                posit.Show();
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
            #endregion
          
            if (args[0] == "MENU_EleProgram")
            {
                EleProgram mode = new EleProgram();
                mode.Show();
            }
            if (args[0] == "MENU_Program")
            {
                CrateUserDefinedOperation.Create();
            }
            if (args[0] == "MENU_ExportElectrode")
            {
                ExportElectrode ele = new ExportElectrode();
                ele.Show();
            }
            if (args[0] == "MENU_AddProgram")
            {
                AddProgram ele = new AddProgram();
                ele.Show();
            }
            */
            if (args[0] == "MENU_PostShopdoc")
            {
                MoldeBase mold = new MoldeBase();
                mold.Show();
            }
              Test.cs();

            return 1;
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
