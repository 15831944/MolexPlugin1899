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
            if (user.Jurisd.GetComm())
            {
                return MoveObject.MoveObjectOfCsys(csys, objs);
            }
            else
            {
                return null;
            }

        }
    }
}
