using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;
using System.Data;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
namespace MolexPlugin.Model
{
    /// <summary>
    /// Work信息
    /// </summary>
    [Serializable]
    public class WorkInfo : ParentAssmblieInfo
    {
        public int WorkNumber { get; set; }
        /// <summary>
        /// 是否过切检查
        /// </summary>
        public bool Interference { get; set; } = false;
        public Matrix4Info MatrInfo { get; set; }
        /// <summary>
        /// 矩阵
        /// </summary>
        public Matrix4 Matr { get; set; }
        public WorkInfo(MoldInfo mold, UserModel user, int workNum, Matrix4 mat) : base(mold, user)
        {
            this.Type = PartType.Work;
            this.WorkNumber = workNum;
            this.Matr = mat;
            this.MatrInfo = new Matrix4Info(mat);
        }
        /// <summary>
        /// 以属性得到实体
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public new static WorkInfo GetAttribute(NXObject obj)
        {
            int num = 0;

            Matrix4 mat = new Matrix4();
            mat.Identity();
            try
            {
                num = AttributeUtils.GetAttrForInt(obj, "WorkNumber");
                WorkInfo info = new WorkInfo(MoldInfo.GetAttribute(obj), UserModel.GetAttribute(obj), num, Matrix4Info.GetAttribute(obj).Matr);
                info.Interference = AttributeUtils.GetAttrForBool(obj, "Interference");
                return info;

            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("未获取到属性" + ex.Message);
                return new WorkInfo(MoldInfo.GetAttribute(obj), UserModel.GetAttribute(obj), num, mat);
            }
        }
        public override bool SetAttribute(params NXObject[] objs)
        {
            try
            {
                AttributeUtils.AttributeOperation("WorkNumber", this.WorkNumber, objs);
                AttributeUtils.AttributeOperation("Interference", this.Interference, objs);
                return base.SetAttribute(objs) && this.MatrInfo.SetAttribute(objs);
            }
            catch
            {
                return false;
            }

        }



    }
}
