using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 电极设计特征
    /// </summary>
    public interface IElectrodeBuilder
    {
        IElectrodeBuilder ParentBuilder { get; }

        bool IsCreateOk { get; }
        /// <summary>
        /// 创建特征
        /// </summary>
        /// <returns></returns>
        bool CreateBuilder();
        /// <summary>
        /// 设置父本特征
        /// </summary>
        /// <param name="builder"></param>
        void SetParentBuilder(IElectrodeBuilder builder);
    }
}
