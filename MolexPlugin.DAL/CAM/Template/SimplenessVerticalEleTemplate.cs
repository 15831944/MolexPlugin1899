using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 直电极
    /// </summary>
    public class SimplenessVerticalEleTemplate : AbstractElectrodeTemplate
    {
        public SimplenessVerticalEleTemplate(CompterToolName tool) : base(tool)
        {
            this.Type = ElectrodeTemplate.SimplenessVerticalEleTemplate;
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
                Programs.Add(new ProgramOperationName(count, to));
                twice = tl;
                count++;
            }


            AbstractCreateOperation fo1 = new FaceMillingCreateOperation(count, tool.GetFinishFlatTool()); //光平面
            fo1.CreateOperationName(1);
            AbstractCreateOperation po1 = new PlanarMillingCreateOperation(count, tool.GetFinishFlatTool()); //光侧面
            po1.CreateOperationName(2);
            Programs.Add(new ProgramOperationName(count, new AbstractCreateOperation[] { fo1, po1 }));
            count++;

            AbstractCreateOperation fo2 = new FaceMillingCreateOperation(count, tool.GetFinishFlatTool()); //去毛刺
            fo2.CreateOperationName(1);
            PlanarMillingCreateOperation po2 = new PlanarMillingCreateOperation(count, tool.GetFinishFlatTool()); //去毛刺
            po2.CreateOperationName(2);
            po2.SetBurringBool(true);
            Programs.Add(new ProgramOperationName(count, new AbstractCreateOperation[] { fo2, po2 }));
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
