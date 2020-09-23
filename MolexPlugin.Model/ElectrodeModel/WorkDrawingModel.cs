using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;
using NXOpen.UF;
using NXOpen.Drawings;

namespace MolexPlugin.Model
{
    public class WorkDrawingModel
    {
        private Part workPart;
        public WorkModel Work { get; private set; }

        public List<NXOpen.Assemblies.Component> HostComp { get; private set; } = new List<NXOpen.Assemblies.Component>();

        public List<NXOpen.Assemblies.Component> OtherComp { get; private set; } = new List<NXOpen.Assemblies.Component>();
        public WorkDrawingModel(WorkModel work)
        {
            this.Work = work;
            workPart = Session.GetSession().Parts.Work;
            GetWorkPieceComp();
        }

        private void GetWorkPieceComp()
        {

            Part host = Work.GetHostWorkpiece();
            foreach (NXOpen.Assemblies.Component ct in AssmbliesUtils.GetPartComp(workPart, host))
            {
                if (!ct.IsSuppressed)
                {
                    ct.Unblank();
                    this.HostComp.Add(ct);
                }

            }
            foreach (Part pt in Work.GetAllWorkpiece())
            {
                if (!host.Equals(pt))
                {
                    foreach (NXOpen.Assemblies.Component ct in AssmbliesUtils.GetPartComp(workPart, pt))
                    {
                        ct.Unblank();
                        OtherComp.Add(ct);
                    }

                }
            }
            this.OtherComp.Sort(delegate (NXOpen.Assemblies.Component a, NXOpen.Assemblies.Component b)
            {
                return a.Name.CompareTo(b.Name);
            });
        }
        /// <summary>
        /// 获取视图
        /// </summary>
        /// <returns></returns>
        public void SetViewShow()
        {
            UFSession theUFSession = UFSession.GetUFSession();
            List<NXOpen.Assemblies.Component> allComt = new List<NXOpen.Assemblies.Component>();
            List<NXOpen.Assemblies.Component> show = new List<NXOpen.Assemblies.Component>();
            show.AddRange(HostComp);
            show.AddRange(OtherComp);
            foreach (NXOpen.Assemblies.Component ct in workPart.ComponentAssembly.RootComponent.GetChildren())
            {
                NXOpen.Assemblies.Component[] des = ct.GetChildren();
                if (des.Length > 0)
                {
                    allComt.AddRange(des);
                    foreach (NXOpen.Assemblies.Component com in des)
                    {
                        com.Unblank();
                    }
                    show.Add(ct); //EDM组件
                    ct.Unblank();
                    continue;
                }
                allComt.Add(ct);
                ct.Blank();
            }

            try
            {
                ModelingView workView = ViewUtils.SetWorkViewForName("work");
                if (workView == null)
                {
                    workView = ViewUtils.CreateView("work", this.Work.Info.Matr.GetMatrix3());
                }
                AssmbliesUtils.HideComponent(null, allComt.ToArray());
                AssmbliesUtils.ShowComponent(null, show.ToArray());
                ViewUtils.SetWorkViewForName("Isometric");
            }
            catch (NXException ex)
            {

            }

        }
        /// <summary>
        /// 隐藏
        /// </summary>
        public void Blank()
        {
            foreach (NXOpen.Assemblies.Component ct in workPart.ComponentAssembly.RootComponent.GetChildren())
            {
                NXOpen.Assemblies.Component[] des = ct.GetChildren();
                if (des.Length > 0)
                {

                    foreach (NXOpen.Assemblies.Component com in des)
                    {
                        com.Unblank();
                    }
                    ct.Unblank();
                    continue;
                }

                ct.Blank();
            }

        }
        /// <summary>
        /// 获取电极Comp
        /// </summary>
        /// <returns></returns>
        public List<NXOpen.Assemblies.Component> GetEleComp()
        {
            List<NXOpen.Assemblies.Component> eleComp = new List<NXOpen.Assemblies.Component>();
            foreach (NXOpen.Assemblies.Component ct in workPart.ComponentAssembly.RootComponent.GetChildren())
            {
                NXOpen.Assemblies.Component[] des = ct.GetChildren();
                if (des.Length == 0)
                {
                    eleComp.Add(ct);

                }
            }
            return eleComp;

        }
        /// <summary>
        /// 画中心点和中心线
        /// </summary>
        /// <param name="disPt"></param>
        public void CreateCenterLine(Point3d centerPt, Point3d disPt)
        {

            Point3d temp = new Point3d(0, 0, 0);
            Point3d minX = new Point3d(centerPt.X - disPt.X - 5.0, 0, 0);
            Point3d maxX = new Point3d(centerPt.X + disPt.X + 5.0, 0, 0);
            Point3d minY = new Point3d(0, centerPt.Y - disPt.Y - 5.0, 0);
            Point3d maxY = new Point3d(0, centerPt.Y + disPt.Y + 5.0, 0);

            Matrix4 inver = this.Work.Info.Matr.GetInversMatrix();
            inver.ApplyPos(ref temp);
            inver.ApplyPos(ref minX);
            inver.ApplyPos(ref maxX);
            inver.ApplyPos(ref minY);
            inver.ApplyPos(ref maxY);

            Line line1 = this.Work.PartTag.Curves.CreateLine(minX, maxX);
            Line line2 = this.Work.PartTag.Curves.CreateLine(minY, maxY);
            SetLineObj(201, new Line[2] { line1, line2 });
        }
        /// <summary>
        /// 设置中心线
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="line"></param>
        private void SetLineObj(int layer, params Line[] line)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            foreach (Line temp in line)
            {
                theUFSession.Obj.SetColor(temp.Tag, 186);
                theUFSession.Obj.SetLayer(temp.Tag, layer);
                theUFSession.Obj.SetFont(temp.Tag, 7);
            }

        }
        /// <summary>
        /// 获取其他非仰制。
        /// </summary>
        /// <returns></returns>
        public List<WorkpieceDrawingModel> GetOtherWorkpieceDrawingModel()
        {
            List<WorkpieceDrawingModel> wm = new List<WorkpieceDrawingModel>();
            foreach (NXOpen.Assemblies.Component ct in this.OtherComp)
            {
                if (!ct.IsSuppressed)
                {
                    wm.Add(new WorkpieceDrawingModel(ct.Prototype as Part, this.Work.Info.Matr));
                }
            }
            return wm;
        }
        /// <summary>
        /// 获得主件
        /// </summary>
        /// <returns></returns>
        public WorkpieceDrawingModel GetHostWorkpieceDrawingModel()
        {
            return new WorkpieceDrawingModel(this.HostComp[0].Prototype as Part, this.Work.Info.Matr);
        }
    }
}
