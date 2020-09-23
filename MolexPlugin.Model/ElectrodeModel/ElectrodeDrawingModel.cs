using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;
using NXOpen.UF;
using NXOpen.Assemblies;

namespace MolexPlugin.Model
{
    /// <summary>
    /// 电极图纸Model
    /// </summary>
    public class ElectrodeDrawingModel : AbstractAssmbileModel
    {

        private List<ElectrodeModel> eleModels;
        private WorkModel work = null;
        private Part asmPart;


        public ElectrodeDrawingInfo Info { get; set; }

        public ElectrodeDrawingModel(List<ElectrodeModel> eleModels, Part asmPart, UserModel user)
        {

            this.eleModels = eleModels;
            this.asmPart = asmPart;

            work = GetWorkModel(eleModels[0]);
            if (work == null)
                throw new Exception("无法找到Work！");
            Info = new ElectrodeDrawingInfo(eleModels[0].Info.MoldInfo, user, eleModels[0].Info.AllInfo);

        }

        public void GetBoundingBox(out Point3d centerPt, out Point3d disPt)
        {
            // Part workPart = Session.GetSession().Parts.Work;
            centerPt = new Point3d();
            disPt = new Point3d();
            List<Body> bodys = new List<Body>();
            Part workpiecePart = this.work.GetHostWorkpiece();
            foreach (Body body in workpiecePart.Bodies.ToArray())
            {
                foreach (NXOpen.Assemblies.Component ct in AssmbliesUtils.GetPartComp(this.PartTag, workpiecePart))
                {
                    Body by = AssmbliesUtils.GetNXObjectOfOcc(ct.Tag, body.Tag) as Body;
                    if (by != null)
                        bodys.Add(by);
                }
            }
            foreach (ElectrodeModel em in eleModels)
            {
                Body by = em.PartTag.Bodies.ToArray()[0];
                foreach (Component eleCt in AssmbliesUtils.GetPartComp(this.PartTag, em.PartTag))
                {
                    Body temp = AssmbliesUtils.GetNXObjectOfOcc(eleCt.Tag, by.Tag) as Body;
                    if (temp != null)
                        bodys.Add(temp);
                }
            }
            Matrix4 invers = this.work.Info.Matr.GetInversMatrix();
            CartesianCoordinateSystem csys = BoundingBoxUtils.CreateCoordinateSystem(this.work.Info.Matr, invers);//坐标
            BoundingBoxUtils.GetBoundingBoxInLocal(bodys.ToArray(), csys, this.work.Info.Matr, ref centerPt, ref disPt);

        }
        /// <summary> 
        /// 获取电极设定点
        /// </summary>
        /// <returns></returns>
        public List<Point> GetEleSetPoint()
        {
            List<Point> pt = new List<Point>();
            foreach (ElectrodeModel em in eleModels)
            {
                Point bo = em.GetSetPoint();
                foreach (Component eleCt in AssmbliesUtils.GetPartComp(this.PartTag, em.PartTag))
                {
                    Point temp = AssmbliesUtils.GetNXObjectOfOcc(eleCt.Tag, bo.Tag) as Point;
                    if (temp != null)
                        pt.Add(temp);
                }
            }
            return pt;
        }
        /// <summary> 
        /// 获取电极X轴线
        /// </summary>
        /// <returns></returns>
        public List<Line> GetXLine()
        {
            List<Line> le = new List<Line>();
            foreach (ElectrodeModel em in eleModels)
            {
                Line bo = em.GetXLine();
                foreach (Component eleCt in AssmbliesUtils.GetPartComp(this.PartTag, em.PartTag))
                {
                    Line temp = AssmbliesUtils.GetNXObjectOfOcc(eleCt.Tag, bo.Tag) as Line;
                    if (temp != null)
                        le.Add(temp);
                }
            }
            return le;
        }

        /// <summary> 
        /// 获取电极X轴线
        /// </summary>
        /// <returns></returns>
        public List<Line> GetYLine()
        {
            List<Line> le = new List<Line>();
            foreach (ElectrodeModel em in eleModels)
            {
                Line bo = em.GetYLine();
                foreach (Component eleCt in AssmbliesUtils.GetPartComp(this.PartTag, em.PartTag))
                {
                    Line temp = AssmbliesUtils.GetNXObjectOfOcc(eleCt.Tag, bo.Tag) as Line;
                    if (temp != null)
                        le.Add(temp);
                }
            }
            return le;
        }

        /// <summary> 
        /// 获取电极体
        /// </summary>
        /// <returns></returns>
        public List<Body> GetEleBody()
        {
            //  Part workPart = Session.GetSession().Parts.Work;
            List<Body> by = new List<Body>();
            foreach (ElectrodeModel em in eleModels)
            {
                Body bo = em.PartTag.Bodies.ToArray()[0];
                foreach (Component eleCt in AssmbliesUtils.GetPartComp(this.PartTag, em.PartTag))
                {
                    Body temp = AssmbliesUtils.GetNXObjectOfOcc(eleCt.Tag, bo.Tag) as Body;
                    if (temp != null)
                        by.Add(temp);
                }
            }
            return by;
        }
        /// <summary>
        /// 获取电极+工件视图比例
        /// </summary>
        /// <param name="xMax"></param>
        /// <param name="yMax"></param>
        /// <returns></returns>
        public double GetScale(double xMax, double yMax, Point3d dis)
        {
            double x = xMax / (dis.X * 2) - 0.1;
            double y = yMax / (dis.Y * 2 + dis.Z * 2) - 0.1;
            if (x > y)
            {
                return Math.Round(y, 1);
            }
            else
            {
                return Math.Round(x, 1);
            }
        }

        /// <summary>
        /// 获取电极视图比例
        /// </summary>
        /// <param name="xMax"></param>
        /// <param name="yMax"></param>
        /// <returns></returns>
        public double GetEleScale(double xMax, double yMax)
        {
            double x = xMax / (this.Info.AllInfo.Preparetion.Preparation[0]);
            double y = yMax / (this.Info.AllInfo.Preparetion.Preparation[1] + this.Info.AllInfo.Preparetion.Preparation[2]);
            if (x > y)
            {
                if (y > 1)
                    return Math.Floor(y);
                else
                    return Math.Round(y, 1);
            }
            else
            {
                if (x > 1)
                    return Math.Floor(x);
                else
                    return Math.Round(x, 1);
            }
        }

        /// <summary>
        /// 获取work
        /// </summary>
        /// <param name="model"></param>
        /// <param name="asmPart"></param>
        /// <returns></returns>
        private WorkModel GetWorkModel(ElectrodeModel model)
        {
            List<NXOpen.Assemblies.Component> eleComs = AssmbliesUtils.GetPartComp(this.asmPart, model.PartTag);
            List<WorkModel> works = new List<WorkModel>();
            foreach (NXOpen.Assemblies.Component ct in eleComs)
            {
                NXOpen.Assemblies.Component parent = ct.Parent;
                if (parent != null)
                {
                    Part pt = parent.Prototype as Part;
                    if (WorkModel.IsWork(pt) && !works.Exists(a => a.PartTag.Equals(pt)) && ParentAssmblieInfo.GetAttribute(pt).MoldInfo.Equals(model.Info.MoldInfo))
                        works.Add(new WorkModel(pt));
                }
            }
            works.Sort();
            if (works.Count != 0)
            {
                return works[0];
            }
            else
                return null;
        }

        public override bool SetAttribute(NXObject obj)
        {
            return this.Info.SetAttribute(obj);
        }

        protected override void GetModelForAttribute(NXObject obj)
        {
            this.Info = ElectrodeDrawingInfo.GetAttribute(obj);
        }

        public override string GetAssembleName()
        {
            return Info.AllInfo.Name.EleName + "_dwg";
        }

        public override Component Load(Part parentPart)
        {
            return null;
        }
        /// <summary>
        /// 装配Work
        /// </summary>
        public void LoadWork()
        {
            Matrix4 mat = new Matrix4();
            mat.Identity();
            PartUtils.SetPartDisplay(this.PartTag);
            AssmbliesUtils.PartLoad(this.PartTag, work.WorkpiecePath, work.AssembleName, mat, new Point3d(0, 0, 0));
        }
        /// <summary>
        /// 获得work矩阵
        /// </summary>
        /// <returns></returns>
        public Matrix4 GetWorkMatr()
        {
            return this.work.Info.Matr;
        }
        /// <summary>
        /// 获得电极矩阵
        /// </summary>
        /// <returns></returns>
        public Matrix4 GetEleMatr()
        {
            return this.eleModels[0].Info.Matr;
        }
        /// <summary>
        /// 获取隐藏工件
        /// </summary>
        /// <param name="setValueHidden"></param>
        /// <param name="eleHidden"></param>
        public void GetHidden(out List<Component> setValueHidden, out List<Component> eleHidden)
        {
            List<Component> all = new List<Component>();
            setValueHidden = new List<Component>();
            eleHidden = new List<Component>();
            foreach (NXOpen.Assemblies.Component ct in this.PartTag.ComponentAssembly.RootComponent.GetChildren())
            {
                //  all.Add(ct);
                ct.Blank();
                foreach (Component co in ct.GetChildren())
                {
                    co.Unblank();
                    Component[] coms = co.GetChildren();
                    if (coms.Length > 0)
                    {
                        all.AddRange(coms);
                        continue;
                    }
                    all.Add(co);
                }
            }
            setValueHidden.AddRange(all);
            eleHidden.AddRange(all);
            foreach (ElectrodeModel em in eleModels)
            {
                List<Component> elecom = AssmbliesUtils.GetPartComp(this.PartTag, em.PartTag);
                foreach (Component eleCt in elecom)
                {
                    eleCt.Unblank();
                    setValueHidden.Remove(eleCt);
                    if (em.Info.AllInfo.SetValue.Positioning == "" || em.Info.AllInfo.SetValue.Positioning.Equals("A", StringComparison.CurrentCultureIgnoreCase))
                    {
                        ElectrodeSetValueInfo value = ElectrodeSetValueInfo.GetAttribute(eleCt);
                        if (value.Positioning == "" || value.Positioning.Equals("A", StringComparison.CurrentCultureIgnoreCase))
                            eleHidden.Remove(eleCt);
                    }
                }
            }
            Part workpiecePart = this.work.GetHostWorkpiece();
            foreach (Component ct in AssmbliesUtils.GetPartComp(this.PartTag, workpiecePart))
            {
                setValueHidden.Remove(ct);
            }

        }

        /// <summary>
        /// 获取边
        /// </summary>
        /// <param name="xEdge"></param>
        /// <param name="yEdge"></param>
        public void GetEdge(out List<Edge> xEdge, out Point centerCom)
        {
            xEdge = new List<Edge>();
            centerCom = null;
            string str = "";
            ElectrodeModel ele = null;
            foreach (ElectrodeModel em in eleModels)
            {

                if (em.Info.AllInfo.SetValue.Positioning == "" || em.Info.AllInfo.SetValue.Positioning.Equals("A", StringComparison.CurrentCultureIgnoreCase))
                {
                    ele = em;
                }

            }
            if (ele == null)
                return;

            List<Face> faces = new List<Face>();
            faces.AddRange(ele.PartTag.Bodies.ToArray()[0].GetFaces());
            Point elePoint = ele.GetSetPoint();
            faces.Sort(delegate (Face a, Face b)
            {
                FaceData data1 = FaceUtils.AskFaceData(a);
                FaceData data2 = FaceUtils.AskFaceData(b);
                return data1.Point.Z.CompareTo((data2.Point.Z));
            });
            foreach (Component ct in AssmbliesUtils.GetPartComp(this.PartTag, ele.PartTag))
            {
                ElectrodeSetValueInfo value = ElectrodeSetValueInfo.GetAttribute(ct);
                if (value.Positioning == "" || value.Positioning.Equals("A", StringComparison.CurrentCultureIgnoreCase))
                {
                    centerCom = AssmbliesUtils.GetNXObjectOfOcc(ct.Tag, elePoint.Tag) as Point;
                    foreach (Edge eg in faces[10].GetEdges())
                    {
                        NXOpen.UF.UFEval.Line lineData;
                        if (EdgeUtils.GetLineData(eg, out lineData, ref str))
                        {
                            if (UMathUtils.IsEqual(lineData.start[1], lineData.end[1]))
                            {
                                xEdge.Add(AssmbliesUtils.GetNXObjectOfOcc(ct.Tag, eg.Tag) as Edge);
                            }
                        }
                    }
                }
            }

        }

        /// <summary>
        /// 获取Work下原点
        /// </summary>
        /// <returns></returns>
        public Point GetWorkCompPoint()
        {
            Part workPart = Session.GetSession().Parts.Work;
            UFSession theUFSession = UFSession.GetUFSession();
            Component ct = AssmbliesUtils.GetPartComp(this.PartTag, work.PartTag)[0];
            Point pt = work.CreateCenterPoint();
            return AssmbliesUtils.GetNXObjectOfOcc(ct.Tag, pt.Tag) as Point;
        }
        /// <summary>
        /// 获取工件体
        /// </summary>
        /// <returns></returns>
        public List<Body> GetWorkpieceBody()
        {
            List<Body> bodys = new List<Body>();
            Part workpiecePart = this.work.GetHostWorkpiece();
            foreach (Body body in workpiecePart.Bodies.ToArray())
            {
                Component workpieceCom = AssmbliesUtils.GetPartComp(this.PartTag, workpiecePart)[0];
                Body by = AssmbliesUtils.GetNXObjectOfOcc(workpieceCom.Tag, body.Tag) as Body;
                if (by != null)
                    bodys.Add(by);
            }
            return bodys;
        }

        /// <summary>
        /// 创建中心点
        /// </summary>
        public Point CreateCenterPoint()
        {
            foreach (Point pt in PartTag.Points)
            {
                if (pt.Name.ToUpper().Equals("CenterPoint".ToUpper()))
                {
                    return pt;
                }
            }
            Point3d temp = new Point3d(0, 0, 0);
            Matrix4 inver = this.work.Info.Matr.GetInversMatrix();
            inver.ApplyPos(ref temp);
            Point originPoint = PointUtils.CreatePoint(temp);
            UFSession theUFSession = UFSession.GetUFSession();

            theUFSession.Obj.SetColor(originPoint.Tag, 186);
            theUFSession.Obj.SetLayer(originPoint.Tag, 201);
            theUFSession.Obj.SetName(originPoint.Tag, "CenterPoint");
            return originPoint;
        }
    }
}
