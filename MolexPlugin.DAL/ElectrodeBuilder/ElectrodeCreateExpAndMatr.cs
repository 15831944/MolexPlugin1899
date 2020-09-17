using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 创建表达式和矩阵
    /// </summary>
    public class ElectrodeCreateExpAndMatr
    {
        public ElectrodeExpression Exp { get; private set; }

        public AbstractElectrodeMatrix Matr { get; private set; }


        public ElectrodeCreateExpAndMatr(AbstractElectrodeMatrix matr)
        {
            this.Exp = new ElectrodeExpression();
            this.Matr = matr;
        }


    }
}
