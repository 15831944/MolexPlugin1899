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
    /// 创建用户实体
    /// </summary>
    public class UserModel
    {
        public string CreatorName { get; set; }

        public string CreateDate { get; set; }

        public UserModel()
        {

        }
        public UserModel(string name, string date)
        {
            this.CreateDate = date;
            this.CreatorName = CreatorName;
        }
        /// <summary>
        /// 获取属性创建类
        /// </summary>
        /// <param name="obj"></param>
        public UserModel(NXObject obj)
        {
            this.CreatorName = AttributeUtils.GetAttrForString(obj, "CreatorName");
            this.CreateDate = AttributeUtils.GetAttrForString(obj, "CreateDate");
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="objs"></param>
        public void SetAttribute(params NXObject[] objs)
        {
            AttributeUtils.AttributeOperation("CreatorName", this.CreatorName, objs);
            AttributeUtils.AttributeOperation("CreateDate", this.CreateDate, objs);
        }


    }
}
