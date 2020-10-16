using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 用户模板电极
    /// </summary>
    public class UserEleTemplate : AbstractElectrodeTemplate
    {
        public UserEleTemplate(CompterToolName tool) : base(tool)
        {
            this.Type = ElectrodeTemplate.User;

        }

        public override void CreateProgramName()
        {
            int count = 1;

            AbstractCreateOperation ro = new RoughCreateOperation(count, "EM8");  //开粗
            ro.CreateOperationName(1);
            Programs.Add(new ProgramOperationName(count, ro));
            count++;

            TwiceRoughCreateOperation to = new TwiceRoughCreateOperation(count, "EM3");
            to.CreateOperationName(1);
            to.SetReferencetool("EM8");
            RounghSurfaceCreateOperation rso = new RounghSurfaceCreateOperation(count, "EM3");
            rso.CreateOperationName(2);
            Programs.Add(new ProgramOperationName(count, new AbstractCreateOperation[] { to, rso }));    
            count++;

            AbstractCreateOperation fo1 = new FaceMillingCreateOperation(count, "EM2.98"); //光平面
            fo1.CreateOperationName(1);
            AbstractCreateOperation po1 = new PlanarMillingCreateOperation(count, "EM2.98"); //光侧面
            po1.CreateOperationName(2);
            Programs.Add(new ProgramOperationName(count, new AbstractCreateOperation[] { fo1, po1 }));
            count++;


            ZLevelMillingCreateOperation zo = new ZLevelMillingCreateOperation(count, "BN0.98"); //等高
            zo.CreateOperationName(1);
            SurfaceContourCreateOperation so = new SurfaceContourCreateOperation(count, "BN0.98"); //等宽
            so.CreateOperationName(2);
            Programs.Add(new ProgramOperationName(count, new AbstractCreateOperation[] { zo, so }));
            count++;

            AbstractCreateOperation bfo = new BaseFaceCreateOperation(count, "EM7.98"); //光基准平面
            bfo.CreateOperationName(1);
            Programs.Add(new ProgramOperationName(count, bfo));
            count++;
            AbstractCreateOperation bsf = new BaseStationCreateOperation(count, "EM7.98"); //光基准台
            bsf.CreateOperationName(1);
            Programs.Add(new ProgramOperationName(count, bsf));

        }
    }
}
