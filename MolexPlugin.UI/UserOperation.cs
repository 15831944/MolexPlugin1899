using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;
using MolexPlugin.DAL;
using MolexPlugin.Model;


namespace MolexPlugin
{
    /// <summary>
    /// 用户定义刀路
    /// </summary>
    public class UserOperation
    {

        public static void CreateUserOper()
        {
            UserSingleton user = UserSingleton.Instance();
            if (user.UserSucceed && user.Jurisd.GetCAMJurisd())
            {
                Part workPart = Session.GetSession().Parts.Work;
                CreateElectrodeCAMBuilder cam = new CreateElectrodeCAMBuilder(workPart, user.CreatorUser);
                cam.CreateOperationNameModel(ElectrodeTemplate.User);
                List<string> err = cam.CreateUserOperation();
                if (err.Count > 0)
                {
                    ClassItem.Print(err.ToArray());
                }
            }
        }
    }
}
