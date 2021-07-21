﻿//==============================================================================
//  WARNING!!  This file is overwritten by the Block UI Styler while generating
//  the automation code. Any modifications to this file will be lost after
//  generating the code again.
//
//       Filename:  C:\Users\ycchen10\OneDrive - kochind.com\Desktop\MolexPlugIn-1899\UI\MoveComponent.cs
//
//        This file was generated by the NX Block UI Styler
//        Created by: ycchen10
//              Version: NX 11
//              Date: 09-23-2020  (Format: mm-dd-yyyy)
//              Time: 13:12 (Format: hh-mm)
//
//==============================================================================

//==============================================================================
//  Purpose:  This TEMPLATE file contains C# source to guide you in the
//  construction of your Block application dialog. The generation of your
//  dialog file (.dlx extension) is the first step towards dialog construction
//  within NX.  You must now create a NX Open application that
//  utilizes this file (.dlx).
//
//  The information in this file provides you with the following:
//
//  1.  Help on how to load and display your Block UI Styler dialog in NX
//      using APIs provided in NXOpen.BlockStyler namespace
//  2.  The empty callback methods (stubs) associated with your dialog items
//      have also been placed in this file. These empty methods have been
//      created simply to start you along with your coding requirements.
//      The method name, argument list and possible return values have already
//      been provided for you.
//==============================================================================

//------------------------------------------------------------------------------
//These imports are needed for the following template code
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.BlockStyler;
using MolexPlugin.DAL;
using NXOpen.UF;
using Basic;
namespace MolexPlugin
{

    //------------------------------------------------------------------------------
    //Represents Block Styler application class
    //------------------------------------------------------------------------------
    public partial class MoveComponent
    {
        //class members
        private static Session theSession = null;
        private static NXOpen.UI theUI = null;
        private Part workPart;
        private string theDlxFileName;
        private NXOpen.BlockStyler.BlockDialog theDialog;
        private NXOpen.BlockStyler.Group group0;// Block type: Group
        private NXOpen.BlockStyler.SelectObject selectionBody;// Block type: Selection
        private NXOpen.BlockStyler.Group group2;// Block type: Group
        private NXOpen.BlockStyler.SelectObject get_point;// Block type: Selection
        private NXOpen.BlockStyler.Group group;// Block type: Group
        private NXOpen.BlockStyler.Button rotation_x;// Block type: Button
        private NXOpen.BlockStyler.Button rotation_y;// Block type: Button
        private NXOpen.BlockStyler.Button rotation_z;// Block type: Button
        private Point selectionPt = null;
        private Face selectionFace = null;
        private List<NXObject> nxObjects = new List<NXObject>();
        private List<NXObject> points = new List<NXObject>();
        private NXObjectAooearancePoint aoo;
        //------------------------------------------------------------------------------
        //Constructor for NX Styler class
        //------------------------------------------------------------------------------
        public MoveComponent()
        {
            try
            {
                theSession = Session.GetSession();
                theUI = NXOpen.UI.GetUI();
                workPart = theSession.Parts.Work;
                theDlxFileName = "MoveComponent.dlx";
                theDialog = theUI.CreateDialog(theDlxFileName);
                theDialog.AddApplyHandler(new NXOpen.BlockStyler.BlockDialog.Apply(apply_cb));
                theDialog.AddOkHandler(new NXOpen.BlockStyler.BlockDialog.Ok(ok_cb));
                theDialog.AddUpdateHandler(new NXOpen.BlockStyler.BlockDialog.Update(update_cb));
                theDialog.AddFilterHandler(new NXOpen.BlockStyler.BlockDialog.Filter(filter_cb));
                theDialog.AddInitializeHandler(new NXOpen.BlockStyler.BlockDialog.Initialize(initialize_cb));
                theDialog.AddDialogShownHandler(new NXOpen.BlockStyler.BlockDialog.DialogShown(dialogShown_cb));
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                throw ex;
            }
        }

        //------------------------------------------------------------------------------
        //This method shows the dialog on the screen
        //------------------------------------------------------------------------------
        public NXOpen.UIStyler.DialogResponse Show()
        {
            try
            {
                UserSingleton user = UserSingleton.Instance();
                if (user.UserSucceed && user.Jurisd.GetComm())
                {
                    CsysUtils.SetWcsToAbs();
                    Component ct = workPart.ComponentAssembly.RootComponent;
                    if (ct == null)
                    {
                        theUI.NXMessageBox.Show("错误", NXMessageBox.DialogType.Error, "显示部件不是装配档!");
                        return 0;
                    }

                    theDialog.Show();
                }

            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
            return 0;
        }

        //------------------------------------------------------------------------------
        //Method Name: Dispose
        //------------------------------------------------------------------------------
        public void Dispose()
        {
            if (theDialog != null)
            {
                theDialog.Dispose();
                theDialog = null;
            }
        }

        //------------------------------------------------------------------------------
        //---------------------Block UI Styler Callback Functions--------------------------
        //------------------------------------------------------------------------------

        //------------------------------------------------------------------------------
        //Callback Name: initialize_cb
        //------------------------------------------------------------------------------
        public void initialize_cb()
        {
            try
            {
                group0 = (NXOpen.BlockStyler.Group)theDialog.TopBlock.FindBlock("group0");
                selectionBody = (NXOpen.BlockStyler.SelectObject)theDialog.TopBlock.FindBlock("selectionBody");
                group2 = (NXOpen.BlockStyler.Group)theDialog.TopBlock.FindBlock("group2");
                get_point = (NXOpen.BlockStyler.SelectObject)theDialog.TopBlock.FindBlock("get_point");
                group = (NXOpen.BlockStyler.Group)theDialog.TopBlock.FindBlock("group");
                rotation_x = (NXOpen.BlockStyler.Button)theDialog.TopBlock.FindBlock("rotation_x");
                rotation_y = (NXOpen.BlockStyler.Button)theDialog.TopBlock.FindBlock("rotation_y");
                rotation_z = (NXOpen.BlockStyler.Button)theDialog.TopBlock.FindBlock("rotation_z");

                Selection.MaskTriple maskComp = new Selection.MaskTriple()
                {
                    Type = 63,
                    Subtype = 1,
                    SolidBodySubtype = 0
                };
                Selection.MaskTriple[] masks1 = { maskComp };
                this.selectionBody.SetSelectionFilter(Selection.SelectionAction.ClearAndEnableSpecific, masks1);//过滤只选择组件
                Selection.MaskTriple maskFace = new Selection.MaskTriple()
                {
                    Type = 70,
                    Subtype = 2,
                    SolidBodySubtype = 20
                };
                Selection.MaskTriple maskPoint = new Selection.MaskTriple()
                {
                    Type = 2,
                    Subtype = 0,
                    SolidBodySubtype = 0
                };
                Selection.MaskTriple[] masks2 = { maskFace, maskPoint };
                get_point.SetSelectionFilter(Selection.SelectionAction.ClearAndEnableSpecific, masks2);//过滤只选择点和面
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
        }

        //------------------------------------------------------------------------------
        //Callback Name: dialogShown_cb
        //This callback is executed just before the dialog launch. Thus any value set 
        //here will take precedence and dialog will be launched showing that value. 
        //------------------------------------------------------------------------------
        public void dialogShown_cb()
        {
            try
            {
                //---- Enter your callback code here -----
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
        }

        //------------------------------------------------------------------------------
        //Callback Name: apply_cb
        //------------------------------------------------------------------------------
        public int apply_cb()
        {
            int errorCode = 0;
            try
            {
                //---- Enter your callback code here -----
                CoordinateSystem wcs = theSession.Parts.Work.WCS.CoordinateSystem;
                Matrix4 mat = new Matrix4();
                mat.Identity();
                mat.TransformToCsys(wcs, ref mat);
                Component ct = selectionBody.GetSelectedObjects()[0] as Component;
                Part parent = ct.Parent.Prototype as Part;
                if (!parent.Equals(workPart))
                    PartUtils.SetPartWork(ct.Parent);
                AssmbliesUtils.MoveCompPartForCsys(mat, ct);
                PartUtils.SetPartWork(null);
                if (points.Count > 0)
                    DeleteObject.Delete(points.ToArray());
                CsysUtils.SetWcsToAbs();
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                errorCode = 1;
                theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
            return errorCode;
        }

        //------------------------------------------------------------------------------
        //Callback Name: update_cb
        //------------------------------------------------------------------------------
        public int update_cb(NXOpen.BlockStyler.UIBlock block)
        {
            try
            {
                if (block == selectionBody)
                {
                    //---------Enter your code here-----------
                    if (points.Count > 0)
                    {
                        DeleteObject.Delete(points.ToArray());
                    }
                    points.Clear();
                    Component ct = selectionBody.GetLastSelectedObjects()[0] as Component;
                    foreach (Body by in (ct.Prototype as Part).Bodies)
                    {
                        if (by.IsBlanked)
                            nxObjects.Add(AssmbliesUtils.GetNXObjectOfOcc(ct.Tag, by.Tag));

                    }
                    this.aoo = new NXObjectAooearancePoint(nxObjects.ToArray());
                    points = aoo.CreatePoint();
                }
                else if (block == get_point)
                {
                    //---------Enter your code here-----------
                    Point3d temp = this.GetPoint();
                    if (this.selectionPt != null)
                    {
                        Basic.CsysUtils.SetWcsOfCentePoint(temp);
                    }

                }
                else if (block == rotation_x)
                {
                    //---------Enter your code here-----------
                    CsysUtils.RotateWcs(WCS.Axis.XAxis, 90);
                }
                else if (block == rotation_y)
                {
                    //---------Enter your code here-----------
                    CsysUtils.RotateWcs(WCS.Axis.YAxis, 90);
                }
                else if (block == rotation_z)
                {
                    //---------Enter your code here-----------
                    CsysUtils.RotateWcs(WCS.Axis.ZAxis, 90);
                }
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
            return 0;
        }

        //------------------------------------------------------------------------------
        //Callback Name: ok_cb
        //------------------------------------------------------------------------------
        public int ok_cb()
        {
            int errorCode = 0;
            try
            {
                errorCode = apply_cb();
                //---- Enter your callback code here -----
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                errorCode = 1;
                theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
            return errorCode;
        }

        //------------------------------------------------------------------------------
        //Callback Name: filter_cb
        //------------------------------------------------------------------------------
        public int filter_cb(NXOpen.BlockStyler.UIBlock block, NXOpen.TaggedObject selectedObject)
        {

            NXOpen.Assemblies.Component ct = selectedObject as Component;
            Component workPartCt = workPart.ComponentAssembly.RootComponent;
            if (ct != null)
            {
                if (ct.Parent.Equals(workPartCt))
                    return (NXOpen.UF.UFConstants.UF_UI_SEL_ACCEPT);
                else
                    return NXOpen.UF.UFConstants.UF_UI_SEL_FAILURE;

            }
            else
                return (NXOpen.UF.UFConstants.UF_UI_SEL_ACCEPT);
        }

        //------------------------------------------------------------------------------
        //Function Name: GetBlockProperties
        //Returns the propertylist of the specified BlockID
        //------------------------------------------------------------------------------
        public PropertyList GetBlockProperties(string blockID)
        {
            PropertyList plist = null;
            try
            {
                plist = theDialog.GetBlockProperties(blockID);
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
            return plist;
        }

    }
}
