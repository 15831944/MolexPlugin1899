using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolexPlugin.DAL
{
    public class ElectrodeTemplateFactory
    {
        public static AbstractElectrodeTemplate CreateOperation(ElectrodeTemplate type, CompterToolName tool)
        {
            AbstractElectrodeTemplate ao = null;
            switch (type)
            {
                case ElectrodeTemplate.SimplenessVerticalEleTemplate:
                    ao = new SimplenessVerticalEleTemplate(tool);
                    break;
                case ElectrodeTemplate.PlanarAndSufaceEleTemplate:
                    ao = new PlanarAndSufaceEleTemplate(tool);
                    break;
                case ElectrodeTemplate.PlanarAndZleveAndSufaceEleTemplate:
                    ao = new PlanarAndZleveAndSufaceEleTemplate(tool);
                    break;
                case ElectrodeTemplate.PlanarAndZleveEleTemplate:
                    ao = new PlanarAndZleveEleTemplate(tool);
                    break;
                case ElectrodeTemplate.User:
                    ao = new UserEleTemplate(tool);
                    break;
                case ElectrodeTemplate.ZleveAndSufaceEleTemplate:
                    ao = new ZleveAndSufaceEleTemplate(tool);
                    break;
                case ElectrodeTemplate.ZleveEleTemplate:
                    ao = new ZleveEleTemplate(tool);
                    break;
                case ElectrodeTemplate.PlanarAndZleveAndSufaceAndFlowCutEleTemplate:
                    ao = new PlanarAndZleveAndSufaceAndFlowCutEleTemplate(tool);
                    break;
                case ElectrodeTemplate.ZleveAndSufaceAndFlowCutEleTemplate:
                    ao = new ZleveAndSufaceAndFlowCutEleTemplate(tool);
                    break;
                default:
                    break;
            }
            return ao;
        }
    }
}
