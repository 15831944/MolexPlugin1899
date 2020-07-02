using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace MolexPlugin.DAL
{
   public interface IMoveBulider
    {
        NXObject Move(params NXObject[] objs);
    }
}
