using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 显示部件
    /// </summary>
    public class MoveVisibleObjects
    {
        public List<NXObject> VisObjs { get; private set; } = new List<NXObject>();
        public NXObjectAooearancePoint Aoo { get; private set; }

        public MoveVisibleObjects()
        {
            Part workPart = Session.GetSession().Parts.Work;
            DisplayableObject[] dispObj = workPart.ModelingViews.WorkView.AskVisibleObjects();          
            foreach (DisplayableObject dis in dispObj)
            {
                if (!(dis is Face) && !(dis is Edge) && !(dis is CoordinateSystem))
                    VisObjs.Add(dis);
            }
            Aoo = new NXObjectAooearancePoint(VisObjs.ToArray());
        }
    }
}
