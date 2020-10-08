using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.Assemblies;
using Basic;
using MolexPlugin.Model;
using System.IO;


namespace MolexPlugin.DAL
{
    /// <summary>
    /// 电极跑位
    /// </summary>
    public class PositionElectrodeBuilder
    {
        private Component eleComp;
        private Part elePart;
        private WorkModel work;
        public PositionElectrodeBuilder(Component eleComp, WorkModel work)
        {
            this.eleComp = eleComp;
            this.elePart = eleComp.Prototype as Part;
            this.work = work;
        }

        private string GetPositionName()
        {
            List<Component> eleComs = GetEleAllComponent();
            if (eleComs != null)
            {
                List<int> names = new List<int>();
                foreach (Component ct in eleComs)
                {
                    ElectrodeSetValueInfo setValue = ElectrodeSetValueInfo.GetAttribute(ct);
                    if (setValue.Positioning != "")
                    {
                        char temp = setValue.Positioning.ToCharArray()[0];
                        names.Add((int)temp);
                    }
                }
                names.Sort();
                return ((char)(names[names.Count - 1] + 1)).ToString();
            }
            return "B";
        }
        public bool PositionBuilder(Vector3d vec)
        {
            try
            {
                Matrix4 inv = this.work.Info.Matr.GetInversMatrix();
                CartesianCoordinateSystem csys = BoundingBoxUtils.CreateCoordinateSystem(this.work.Info.Matr, inv);
                ElectrodeSetValueInfo setValue = ElectrodeSetValueInfo.GetAttribute(eleComp);
                ElectrodeSetValueInfo newSetValue = setValue.Clone() as ElectrodeSetValueInfo;

                Component copyComp = AssmbliesUtils.MoveCompCopyPart(eleComp, vec, work.Info.Matr);
                NXObject instance = AssmbliesUtils.GetOccOfInstance(copyComp.Tag);
                BodyInfo info = GetDischargeFace(csys, copyComp);
                newSetValue.EleSetValue[0] = setValue.EleSetValue[0] + vec.X;
                newSetValue.EleSetValue[1] = setValue.EleSetValue[1] + vec.Y;
                newSetValue.EleSetValue[2] = setValue.EleSetValue[2] + vec.Z;
                newSetValue.Positioning = GetPositionName();
                if (info != null)
                {
                    newSetValue.ContactArea = info.ContactArea;
                    newSetValue.ProjectedArea = info.GetProjectedArea(csys, this.work.Info.Matr);
                }

                newSetValue.SetAttribute(instance);
                return true;
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("电极跑位错误！" + ex.Message);
                return false;

            }
        }
        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public bool MovePositionBuilder(Vector3d vec)
        {
            try
            {
                Matrix4 inv = this.work.Info.Matr.GetInversMatrix();
                CartesianCoordinateSystem csys = BoundingBoxUtils.CreateCoordinateSystem(this.work.Info.Matr, inv);
                ElectrodeSetValueInfo setValue = ElectrodeSetValueInfo.GetAttribute(eleComp);
                ElectrodeSetValueInfo newSetValue = setValue.Clone() as ElectrodeSetValueInfo;

                AssmbliesUtils.MoveCompPart(eleComp, vec, work.Info.Matr);
                NXObject instance = AssmbliesUtils.GetOccOfInstance(eleComp.Tag);
                BodyInfo info = GetDischargeFace(csys, eleComp);
                newSetValue.EleSetValue[0] = setValue.EleSetValue[0] + vec.X;
                newSetValue.EleSetValue[1] = setValue.EleSetValue[1] + vec.Y;
                newSetValue.EleSetValue[2] = setValue.EleSetValue[2] + vec.Z;
                newSetValue.Positioning = GetPositionName();
                newSetValue.PositioningRemark = "设定值改变";
                if (info != null)
                {
                    newSetValue.ContactArea = info.ContactArea;
                    newSetValue.ProjectedArea = info.GetProjectedArea(csys, this.work.Info.Matr);
                }
                newSetValue.SetAttribute(instance);
                return true;
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("电极跑位错误！" + ex.Message);
                return false;

            }
        }
        /// <summary>
        /// 获得电极组件
        /// </summary>
        /// <returns></returns>
        private List<NXOpen.Assemblies.Component> GetEleAllComponent()
        {
            ASMModel asm = ASMCollection.GetAsmModel(this.elePart);
            try
            {
                return AssmbliesUtils.GetPartComp(asm.PartTag, elePart);
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("获取电极组件错误！" + ex.Message);
                return null;
            }
        }

        private BodyInfo GetDischargeFace(CartesianCoordinateSystem csys, Component ct)
        {
            List<string> err = new List<string>();
            Part host = work.GetHostWorkpiece();
            if (host == null)
                return null;
            Body workBody = host.Bodies.ToArray()[0];
            Body eleBody = elePart.Bodies.ToArray()[0];
            Body comBody = AssmbliesUtils.GetNXObjectOfOcc(ct.Tag, eleBody.Tag) as Body;
            ComputeDischargeFace cp = new ComputeDischargeFace(comBody, workBody, work.Info.Matr, csys);
            return cp.GetBodyInfoForInterference(false, out err);
        }
    }
}
