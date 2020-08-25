using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 以点到点
    /// </summary>
    public class MovePointToPointBuilder : IMoveBulider
    {
        private Point3d startPt;
        private Point3d endPt;
        private UserSingleton user;
        public MovePointToPointBuilder(Point3d startPt, Point3d endPt)
        {
            this.startPt = startPt;
            this.endPt = endPt;
            this.user = UserSingleton.Instance();
        }
        public NXObject Move(params NXObject[] objs)
        {
            if (user.UserSucceed && user.Jurisd.GetComm())
            {
                try
                {
                    NXObject obj = MoveObject.MoveObjectOfPointToPoint(this.startPt, this.endPt, objs);
                    return obj;
                }
                catch(NXException ex)
                {
                    UI.GetUI().NXMessageBox.Show("错误", NXMessageBox.DialogType.Error, "无法移动--" + ex.Message);
                    return null;
                }
            }
            else
            {

                return null;
            }

        }
    }
}
