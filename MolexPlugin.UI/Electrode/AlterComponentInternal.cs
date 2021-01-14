using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;
using NXOpen.BlockStyler;
using NXOpen.Utilities;
using Basic;
using MolexPlugin.DAL;
using MolexPlugin.Model;
using System.IO;

namespace MolexPlugin
{
    public partial class AlterComponent
    {
        /// <summary>
        /// 设置对话框显示
        /// </summary>
        private void SetDisp(NXOpen.Assemblies.Component ct)
        {
            if (ParentAssmblieInfo.IsElectrode(ct))
            {
                this.groupEle.Show = true;
                info = ElectrodeInfo.GetAttribute(ct);
                //this.strMoldNumber.Show = false;
                // this.strWorkpieceNumber.Show = false;
                // this.strEditionNumber.Show = false;
                this.groupWorkpiece.Show = false;
                this.groupWork.Show = false;
                ElectrodeInfo eleInfo = info as ElectrodeInfo;
                string temp = info.MoldInfo.MoldNumber + "-" + info.MoldInfo.WorkpieceNumber;
                this.strEleName.Value = temp;
                this.strEleName1.Value = eleInfo.AllInfo.Name.EleName.Substring(temp.Length, eleInfo.AllInfo.Name.EleName.Length - temp.Length);
                this.strEleEditionNumber.Value = eleInfo.AllInfo.Name.EleEditionNumber;
            }
            else if (ParentAssmblieInfo.IsWorkpiece(ct))
            {
                info = WorkPieceInfo.GetAttribute(ct);
                this.groupWorkpiece.Show = true;
                this.groupEle.Show = false;
                this.groupWork.Show = false;
                this.strMoldNumber.Value = info.MoldInfo.MoldNumber;
                this.strWorkpieceNumber.Value = info.MoldInfo.WorkpieceNumber;
                this.strEditionNumber.Value = info.MoldInfo.EditionNumber;
            }
            else if (ParentAssmblieInfo.IsWork(ct))
            {
                info = WorkInfo.GetAttribute(ct);
                this.groupWork.Show = true;
                this.groupEle.Show = false;
                this.groupWorkpiece.Show = false;
                this.intWorkNumber.Value = (info as WorkInfo).WorkNumber;
            }
            else if (!ParentAssmblieInfo.IsParent(ct))
            {
                this.groupWorkpiece.Show = true;
                this.groupEle.Show = false;
                this.strWorkpieceNumber.Value = ct.Name;
            }
        }

        private void AlterEle(NXOpen.Assemblies.Component ct)
        {
            Part workPart = theSession.Parts.Work;
            Part pt = ct.Prototype as Part;
            ElectrodeNameInfo newNameInfo = new ElectrodeNameInfo()
            {
                EleEditionNumber = this.strEleEditionNumber.Value.ToUpper(),
                EleName = this.strEleName.Value + this.strEleName1.Value.ToUpper(),
            };
            newNameInfo.EleNumber = newNameInfo.GetEleNumber(newNameInfo.EleName);
            ReplaceElectrode el = new ReplaceElectrode(pt, newNameInfo);
            Part newPart = null;
            List<string> err = el.AlterEle(out newPart);
            if (newPart != null)
            {
                err.AddRange(el.AlterEleDra());
                List<NXObject> objs = AskEleAllInstance(newPart);
                if (objs.Count > 0)
                {
                    newNameInfo.SetAttribute(objs.ToArray());
                }
            }

            PartUtils.SetPartDisplay(workPart);

            if (err.Count > 0)
                ClassItem.Print(err.ToArray());
        }
        /// <summary>
        /// 替换工件
        /// </summary>
        /// <param name="ct"></param>
        private void AlterWorkpiece(NXOpen.Assemblies.Component ct, UserModel user)
        {
            MoldInfo mold;
            if (!ParentAssmblieInfo.IsParent(ct))
            {
                mold = new MoldInfo()
                {
                    MoldNumber = this.strMoldNumber.Value.ToUpper(),
                    WorkpieceNumber = this.strWorkpieceNumber.Value.ToUpper(),
                    EditionNumber = this.strEditionNumber.Value.ToUpper()
                };
            }
            else
            {
                mold = MoldInfo.GetAttribute(ct);
                mold.MoldNumber = this.strMoldNumber.Value;
                mold.WorkpieceNumber = this.strWorkpieceNumber.Value;
                mold.EditionNumber = this.strEditionNumber.Value;
            }
            WorkPieceInfo wk = new WorkPieceInfo(mold, user);
            string newName = mold.MoldNumber + "-" + mold.WorkpieceNumber + "-" + mold.EditionNumber;
            Part pt = ct.Prototype as Part;
            ReplaceOther ot = new ReplaceOther(pt, wk);
            List<string> err = ot.Alter(newName);
            if (err.Count > 0)
                ClassItem.Print(err.ToArray());
        }

        /// <summary>
        /// 替换工件
        /// </summary>
        /// <param name="ct"></param>
        private void AlterWork(NXOpen.Assemblies.Component ct, UserModel user)
        {
            int workNumber = this.intWorkNumber.Value;
            string newName = info.MoldInfo.MoldNumber + "-" + info.MoldInfo.WorkpieceNumber + "-WORK" + workNumber.ToString(); ;
            WorkInfo workInfo = new WorkInfo(info.MoldInfo, user, workNumber, (info as WorkInfo).Matr);
            Part pt = ct.Prototype as Part;
            ReplaceOther ot = new ReplaceOther(pt, workInfo);
            List<string> err = ot.Alter(newName);
            if (err.Count > 0)
                ClassItem.Print(err.ToArray());
        }

        private List<NXObject> AskEleAllInstance(Part ele)
        {
            List<NXObject> objs = new List<NXObject>();
            foreach (NXOpen.Assemblies.Component ct in AssmbliesUtils.GetPartComp(asm.PartTag, ele))
            {
                NXObject nt = null;
                try
                {
                    nt = AssmbliesUtils.GetOccOfInstance(ct.Tag);
                }
                catch
                {

                }
                if (nt != null)
                {
                    objs.Add(nt);
                }
            }
            return objs;
        }
    }
}
