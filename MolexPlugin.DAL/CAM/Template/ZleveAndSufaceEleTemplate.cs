using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 直加等高等宽
    /// </summary>
    public class ZleveAndSufaceEleTemplate : AbstractElectrodeTemplate
    {
        public ZleveAndSufaceEleTemplate(CompterToolName tool) : base(tool)
        {
            this.Type = ElectrodeTemplate.ZleveAndSufaceEleTemplate;
        }

        public override void CreateProgramName()
        {
            int count = 1;

            string rough = tool.GetRoughTool();
            AbstractCreateOperation ro = new RoughCreateOperation(count, rough);  //开粗
            ro.CreateOperationName(1);
            Programs.Add(new ProgramOperationName(count, ro));
            count++;
            string twice = rough;
            foreach (string tl in tool.GetTwiceRoughTool()) //二次开粗
            {
                TwiceRoughCreateOperation to = new TwiceRoughCreateOperation(count, tl);
                to.CreateOperationName(1);
                to.SetReferencetool(twice);
                if (twice.Equals(rough, StringComparison.CurrentCultureIgnoreCase))
                {
                    RounghSurfaceCreateOperation rso = new RounghSurfaceCreateOperation(count, tl);
                    rso.CreateOperationName(2);
                    Programs.Add(new ProgramOperationName(count, new AbstractCreateOperation[] { to, rso }));
                    twice = tl;
                    count++;
                    continue;
                }
                Programs.Add(new ProgramOperationName(count, to));
                twice = tl;
                count++;
            }


            string toolName = tool.GetFinishBallTool();
            ZLevelMillingCreateOperation zo = new ZLevelMillingCreateOperation(count, toolName); //等高
            zo.CreateOperationName(1);
            zo.SetSteep(true, true);
            SurfaceContourCreateOperation so = new SurfaceContourCreateOperation(count, toolName); //等宽
            so.CreateOperationName(2);
            so.SetFlat(true, true);
            Programs.Add(new ProgramOperationName(count, new AbstractCreateOperation[] { zo, so }));
            count++;

            AbstractCreateOperation bfo = new BaseFaceCreateOperation(count, tool.GetBaseFaceTool()); //光基准平面
            bfo.CreateOperationName(1);
            Programs.Add(new ProgramOperationName(count, bfo));
            count++;
            AbstractCreateOperation bsf = new BaseStationCreateOperation(count, tool.GetBaseStationTool()); //光基准台
            bsf.CreateOperationName(1);
            Programs.Add(new ProgramOperationName(count, bsf));

        }
    }
}
