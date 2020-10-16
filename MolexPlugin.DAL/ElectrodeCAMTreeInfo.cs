using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolexPlugin.DAL
{
    public class ElectrodeCAMTreeInfo
    {
        public string ProgramName { get; set; }

        public string ToolName { get; set; } = "";

        public List<ElectrodeCAMTreeInfo> Children { get; set; } = new List<ElectrodeCAMTreeInfo>();

        public ElectrodeCAMTreeInfo Parent { get; private set; } = null;
        public string Png { get; set; } = "";

        private object name;

        public object Program { get { return name; } }
        public ElectrodeCAMTreeInfo(ProgramOperationName name)
        {
            this.ProgramName = name.Program;
            this.name = name;
            this.Children = CreateModels();
        }
        private ElectrodeCAMTreeInfo()
        {

        }
        private List<ElectrodeCAMTreeInfo> CreateModels()
        {
            List<ElectrodeCAMTreeInfo> info = new List<ElectrodeCAMTreeInfo>();
            if(name is ProgramOperationName)
            {                
                foreach (AbstractCreateOperation ao in (name as ProgramOperationName).Oper)
                {
                    ElectrodeCAMTreeInfo ei = new ElectrodeCAMTreeInfo();
                    ei.name = ao;
                    ei.ProgramName = ao.NameModel.OperName;
                    ei.ToolName = ao.ToolName;
                    ei.Png = ao.NameModel.PngName;
                    ei.Parent = this;
                    info.Add(ei);
                }
            }
            return info;
        }
    }
}
