using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;


namespace MolexPlugin.Model
{
    /// <summary>
    /// 体阵列分类
    /// </summary>
    public class BodyPitchClassify
    {
        private List<BodyBoundingBoxInfo> info;
        private int xNum;
        private int yNum;
        private Body[,] classifyBodys;

        public Body[,] ClassifyBodys { get { return classifyBodys; } }
        public BodyPitchClassify(List<Body> bodys, Matrix4 matr, CartesianCoordinateSystem csys, int xNum, int yNum)
        {
            this.xNum = xNum;
            this.yNum = yNum;
            foreach (Body bt in bodys)
            {
                info.Add(new BodyBoundingBoxInfo(bt, matr, csys));
            }
            classifyBodys = new Body[xNum, yNum];
            Classify();
        }

        private void Classify()
        {
            var xBody = info.GroupBy(a => a.CenterPt.X);
            xBody.OrderByDescending(a => a.Key);
            int h = 0;
            try
            {
                foreach (IGrouping<double, BodyBoundingBoxInfo> k in xBody)
                {
                    k.OrderBy(a => a.CenterPt.X);
                    int i = 0;
                    foreach (BodyBoundingBoxInfo io in k)
                    {
                        classifyBodys[i, h] = io.Body;
                        i++;
                    }
                    h++;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SetAttribute()
        {
            for (int i = 0; i < xNum; i++)
            {
                for (int k = 0; k < yNum; k++)
                {
                    int[] temp = { i + 1, k + 1 };
                    AttributeUtils.AttributeOperation("PitchNum", temp, ClassifyBodys[i, k]);
                    AttributeUtils.AttributeOperation("PitchNum", temp, ClassifyBodys[i, k].GetFaces());
                }
            }
        }
        /// <summary>
        /// 获取体
        /// </summary>
        /// <param name="xNum"></param>
        /// <param name="yNum"></param>
        /// <returns></returns>
        public List<Body> GetBodysForRowColumn(int xNum, int yNum)
        {
            List<Body> bodys = new List<Body>();
            if (xNum <= this.xNum && yNum <= this.yNum)
            {
                for (int row = 0; row < yNum; row++)
                {
                    for (int col = 0; col < xNum; col++)
                    {
                        bodys.Add(this.classifyBodys[col, row]);
                    }
                }
            }
            return bodys;
        }
        /// <summary>
        /// 获取体
        /// </summary>
        /// <param name="xNum"></param>
        /// <param name="yNum"></param>
        /// <returns></returns>
        public List<Body> GetOtherBodysForRowColumn(int xNum, int yNum)
        {
            List<Body> bodys = new List<Body>();
            if (xNum <= this.xNum && yNum <= this.yNum)
            {
                for (int row = yNum + 1; row < this.yNum; row++)
                {
                    for (int col = xNum + 1; col < this.xNum; col++)
                    {
                        bodys.Add(this.classifyBodys[col, row]);
                    }
                }
            }
            return bodys;
        }
    }
    /// <summary>
    /// 体外形信息
    /// </summary>
    public class BodyBoundingBoxInfo
    {
        private Point3d disPt = new Point3d();
        private Point3d centerPt = new Point3d();
        public Point3d DisPt
        {
            get
            {
                return disPt;
            }
        }

        public Point3d CenterPt
        {
            get
            {
                return centerPt;
            }
        }

        public Body Body { get; private set; }
        public BodyBoundingBoxInfo(Body body, Matrix4 matr, CartesianCoordinateSystem csys)
        {
            this.Body = body;
            NXObject[] obj = { body };
            BoundingBoxUtils.GetBoundingBoxInLocal(obj, csys, matr, ref centerPt, ref disPt);
        }


    }
}
