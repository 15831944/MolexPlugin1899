using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace Basic
{
    public class LayerUtils
    {
        /// <summary>
        /// 移动到层
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="obj"></param>
        public static bool MoveDisplayableObject(int layer, params NXObject[] obj)
        {
            Part workPart = Session.GetSession().Parts.Work;
            List<DisplayableObject> dis = new List<DisplayableObject>();
            foreach (NXObject nx in obj)
            {
                if (!(nx is Edge || nx is Face))
                    dis.Add(nx as DisplayableObject);
            }
            try
            {
                workPart.Layers.MoveDisplayableObjects(layer, dis.ToArray());
                return true;
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("LayerUtils.MoveDisplayableObject            " + ex.Message);
                return false;
            }

        }

        public static NXObject[] GetAllObjectsOnLayer(int layer)
        {
            Part workPart = Session.GetSession().Parts.Work;
            return workPart.Layers.GetAllObjectsOnLayer(layer);
        }
        /// <summary>
        /// 设置层
        /// </summary>
        /// <param name="fitAll"></param>
        /// <param name="layers"></param>
        public static void SetLayerSelectable( bool fitAll,params int[] layers)
        {
            Part workPart = Session.GetSession().Parts.Work;
            List<NXOpen.Layer.StateInfo> stateArrays = new List<NXOpen.Layer.StateInfo>();
            try
            {
                foreach (int k in layers)
                {
                    stateArrays.Add(new NXOpen.Layer.StateInfo(k, NXOpen.Layer.State.Selectable));

                }
                workPart.Layers.ChangeStates(stateArrays.ToArray(), fitAll);
            }

            catch (NXException ex)
            {
                LogMgr.WriteLog("LayerUtils:SetLayer           错误！" + ex.Message);
                throw ex;
            }




        }
    }
}
