using NXOpen.BlockStyler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolexPlugin.Model
{
    /// <summary>
    /// 显示选项接口
    /// </summary>
    public interface IDisplayObject
    {
        void Highlight(bool highlight);
        Node Node { get; set; }
    }
}
