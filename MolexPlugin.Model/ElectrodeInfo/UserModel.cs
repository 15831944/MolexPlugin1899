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
    [Serializable]
    public class UserModel : ISetAttribute
    {
        public string CreatorName { get; set; }

        public string CreatedDate { get; set; }

        public UserModel()
        {

        }
        public UserModel(string name, string date)
        {
            this.CreatedDate = date;
            this.CreatorName = CreatorName;
        }
        /// <summary>
        /// 获取属性创建类
        /// </summary>
        /// <param name="obj"></param>
        public UserModel(NXObject obj)
        {
            this.CreatorName = AttributeUtils.GetAttrForString(obj, "CreatorName");
            this.CreatedDate = AttributeUtils.GetAttrForString(obj, "CreatedDate");
        }

        public static UserModel GetAttribute(NXObject obj)
        {
            UserModel model = new UserModel();
            try
            {
                model.CreatorName = AttributeUtils.GetAttrForString(obj, "CreatorName");
                model.CreatedDate = AttributeUtils.GetAttrForString(obj, "CreatedDate");
                return model;
            }
            catch (NXException ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        public bool SetAttribute(params NXObject[] objs)
        {
            try
            {
                AttributeUtils.AttributeOperation("CreatorName", this.CreatorName, objs);
                AttributeUtils.AttributeOperation("CreatedDate", this.CreatedDate, objs);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
