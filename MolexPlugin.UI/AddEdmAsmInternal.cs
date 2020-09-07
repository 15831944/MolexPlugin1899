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
    public partial class AddEdmAsm
    {
        /// <summary>
        /// 设置对话框
        /// </summary>
        private void SetUiInfo()
        {
            Part workPart = theSession.Parts.Work;
            this.PartNumber.Value = workPart.Name;
            List<string> modeType = new List<string>();
            List<string> clientName = new List<string>();
            foreach (ControlEnum control in ControlDeserialize.Controls)
            {
                if (control.ControlType == "MoldType")
                    modeType.Add(control.EnumName);
                if (control.ControlType == "Client")
                    clientName.Add(control.EnumName);
            }
            this.MoldType.SetListItems(modeType.ToArray());
            this.ClientNumber.SetListItems(clientName.ToArray());
            if (modeType.Count >= 1)
                this.MoldType.Value = modeType[0];
            if (clientName.Count >= 1)
                this.ClientNumber.Value = clientName[0];
        }
        /// <summary>
        /// 创建特征
        /// </summary>
        /// <param name="mold"></param>
        private void CreateBulder(MoldInfo mold, string directoryPath)
        {
            UserSingleton user = UserSingleton.Instance();
            if (user.UserSucceed && user.Jurisd.GetElectrodeJurisd())
            {
                AbstractCreateAssmbile asm = new AsmCreateAssmbile(mold, user.CreatorUser, workPart);
                List<string> err = asm.CreatePart(directoryPath);
                err.AddRange(asm.LoadAssmbile());
                if (err.Count != 0)
                {
                    ClassItem.Print(err.ToArray());
                }

            }

        }
        /// <summary>
        /// 获取文件夹
        /// </summary>
        /// <param name="mold"></param>
        /// <returns></returns>
        private string GetDirectoryPath(MoldInfo mold)
        {
            string name = mold.MoldNumber + mold.WorkpieceNumber + mold.EditionNumber;
            if (workPart.Name.Replace("-", "").Equals(name, StringComparison.CurrentCultureIgnoreCase))
            {
                return Path.GetDirectoryName(workPart.FullPath) + "\\";
            }
            else
                return Path.GetDirectoryName(workPart.FullPath) + "\\" + mold.WorkpieceNumber + "-" + mold.EditionNumber + "\\";
        }
    }
}
