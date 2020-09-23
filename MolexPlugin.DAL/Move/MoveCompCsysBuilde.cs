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
    public class MoveCompCsysBuilde : IMoveBulider
    {
        private CoordinateSystem csys;

        private UserSingleton user;
        public MoveCompCsysBuilde(CoordinateSystem csys)
        {
            this.csys = csys;
            this.user = UserSingleton.Instance();
        }
        public NXObject Move(params NXObject[] objs)
        {
            if (user.UserSucceed && user.Jurisd.GetComm())
            {
                List<NXOpen.Assemblies.Component> cts = new List<NXOpen.Assemblies.Component>();

                foreach (NXObject obj in objs)
                {
                    if (obj is NXOpen.Assemblies.Component)
                        cts.Add(obj as NXOpen.Assemblies.Component);
                }
                Matrix4 mat = new Matrix4();
                mat.Identity();
                mat.TransformToCsys(csys, ref mat);
                try
                {
                    AssmbliesUtils.MoveCompPartForCsys(mat, cts.ToArray());
                    return null;
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
