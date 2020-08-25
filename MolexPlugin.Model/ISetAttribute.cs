using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace MolexPlugin.Model
{
    public interface ISetAttribute
    {
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="objs"></param>
        bool SetAttribute(params NXObject[] objs);
    }
}
