using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    public class ElectrodeCAMFactory
    {
        public static AbstractElectrodeCAM CreateCAM(Part pt, UserModel user)
        {
            AbstractElectrodeCAM cam = null;
            if (ParentAssmblieInfo.IsElectrode(pt))
            {
                ElectrodeModel em = new ElectrodeModel(pt);
                ElectrodeGapValueInfo gap = em.Info.AllInfo.GapValue;
                if ((gap.ERNum[0] != 0 || gap.ERNum[1] != 0) && (gap.FineInter != 0 && gap.FineNum != 0) &&
                    (gap.CrudeInter != 0 && gap.CrudeNum == 0))
                {
                    try
                    {
                        cam = new ErAndEfElectrodeCAM(em, user);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                else if (((gap.FineInter != 0 && gap.FineNum != 0) && (gap.CrudeInter != 0 && gap.CrudeNum != 0)) ||
                    ((gap.FineInter != 0 && gap.FineNum != 0) && (gap.DuringInter != 0 && gap.DuringNum != 0)) ||
                    ((gap.CrudeInter != 0 && gap.CrudeNum != 0) && (gap.DuringInter != 0 && gap.DuringNum != 0)))
                {
                    try
                    {
                        cam = new ManyInterElectrodeCAM(em, user);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                else if (((gap.FineInter != 0 && gap.FineNum != 0) && (gap.CrudeInter == 0 && gap.CrudeNum == 0) && (gap.DuringNum == 0 && gap.DuringNum == 0)) ||
                    ((gap.FineInter == 0 && gap.FineNum == 0) && (gap.CrudeInter != 0 && gap.CrudeNum != 0) && (gap.DuringNum == 0 && gap.DuringNum == 0)) ||
                    ((gap.FineInter == 0 && gap.FineNum == 0) && (gap.CrudeInter == 0 && gap.CrudeNum == 0) && (gap.DuringNum != 0 && gap.DuringNum != 0)))
                {
                    try
                    {
                        cam = new OnlyInterElectrodeCAM(em, user);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                else
                {
                    try
                    {
                        cam = new NonStandardElectrodeCAM(pt, user);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            else
            {
                try
                {
                    cam = new NonStandardElectrodeCAM(pt, user);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return cam;
        }
    }
}
