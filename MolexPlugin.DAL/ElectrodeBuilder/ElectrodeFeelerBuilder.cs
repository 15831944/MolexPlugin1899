using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;
using Basic;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 创建对刀特征
    /// </summary>
    public class ElectrodeFeelerBuilder : IElectrodeBuilder
    {
        private IElectrodeBuilder builder = null;
        private bool isok = false;
        private ElectrodeSketchBuilder sketch;
        private ElectrodeDatumInfo datum;


        public Body FeelerBody { get; private set; }
        public IElectrodeBuilder ParentBuilder { get { return builder; } }

        public bool IsCreateOk { get { return isok; } }


        public ElectrodeFeelerBuilder(ElectrodeSketchBuilder sketch, ElectrodeDatumInfo datum)
        {
            this.datum = datum;
            this.sketch = sketch;
        }
        private bool CreatFeeler()
        {
            double z = this.datum.EleHeight;
            try
            {
                this.FeelerBody = ExtrudedUtils.CreateExtruded(new Vector3d(0, 0, 1), "0", z.ToString(), null, sketch.Center).GetBodies()[0];
                MoveObject.CreateMoveObjToXYZ("moveBoxX", "moveBoxY", "moveBoxZ", null, FeelerBody);
                return true;
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("创建对刀台错误！" + ex.Message);
                return false;
            }

        }
        public bool CreateBuilder()
        {
            if (ParentBuilder != null)
            {
                if (!ParentBuilder.IsCreateOk)
                    ParentBuilder.CreateBuilder();
                if (!isok && ParentBuilder.IsCreateOk)
                    isok = CreatFeeler();
            }
            else if (!isok)
                isok = CreatFeeler();
            return isok;
        }

        public void SetParentBuilder(IElectrodeBuilder builder)
        {
            this.builder = builder;
        }



    }
}
