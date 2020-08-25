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
    /// 按CSYS移动
    /// </summary>
    public class MoveCsysBuilder : IMoveBulider
    {
        private CoordinateSystem csys;

        private UserSingleton user;
        public MoveCsysBuilder(CoordinateSystem csys)
        {
            this.csys = csys;
            this.user = UserSingleton.Instance();
        }
        public NXObject Move(params NXObject[] objs)
        {
            if (user.UserSucceed && user.Jurisd.GetComm())
            {
                try
                {
                    NXObject obj = MoveObject.MoveObjectOfCsys(csys, objs);
                    return obj;
                }
                catch (NXException ex)
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
