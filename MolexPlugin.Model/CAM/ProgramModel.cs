using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;
using NXOpen.CAM;
namespace MolexPlugin.Model
{
    public class ProgramModel : IComparable<ProgramModel>
    {
        private List<OperationData> data = new List<OperationData>();
        /// <summary>
        /// 程序组
        /// </summary>
        public NCGroup ProgramGroup { get; private set; }
        /// <summary>
        /// 程序组是否正确
        /// </summary>
        public bool Estimate
        {
            get
            {
                return IsEstimate();
            }
        }
        /// <summary>
        /// 刀具路径数据
        /// </summary>
        public List<OperationData> OperData
        {
            get
            {
                if (data.Count == 0)
                {
                    GetOper();
                }
                return data;
            }
        }
        public ProgramModel(NCGroup program)
        {
            this.ProgramGroup = program;
        }
        /// <summary>
        /// 获取刀具路径数据
        /// </summary>
        private void GetOper()
        {

            foreach (CAMObject np in this.ProgramGroup.GetMembers())
            {
                if (np is NXOpen.CAM.Operation)
                {
                    AbstractOperationModel model = CreateOperationFactory.GetOperation(np as NXOpen.CAM.Operation);
                    OperationData od = model.GetOperationData();
                    data.Add(od);
                }

            }
        }
        /// <summary>
        /// 判断刀具是否一把
        /// </summary>
        private bool IsEstimate()
        {
            if (data.Count == 0)
            {
                GetOper();
            }
            string toolName = "";
            foreach (OperationData od in data)
            {
                if (toolName == "")
                    toolName = od.Tool.ToolName;
                else if (!toolName.Equals(od.Tool.ToolName, StringComparison.CurrentCultureIgnoreCase)) //比较同一程式组是否有两把刀
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 获取程序组时间
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public double GetProgramTime()
        {
            double time = 0;
            foreach (OperationData od in data)
            {
                time += od.OperTime;
            }
            time = time / 1440.0 + 0.00014;
            return time;
        }

        /// <summary>
        /// 过滤复制刀路
        /// </summary>
        /// <returns></returns>
        public List<OperationData> GetOperationFiltrationOrher()
        {
            List<OperationData> newData = new List<OperationData>();
            foreach (OperationData od in data)
            {
                if (od.Oper.HasOtherInstances)
                {
                    bool isok = false;
                    foreach (NXOpen.CAM.Operation op in od.Oper.GetOtherInstances())
                    {
                        if (newData.Exists(a => a.Oper.Equals(op)))
                        {
                            isok = true;
                            break;
                        }
                    }
                    if (!isok)
                        newData.Add(od);
                }
                else
                {
                    bool isok = false;
                    foreach (OperationData op in newData)
                    {
                        if (op.Equals(od))
                        {
                            isok = true;
                            break;
                        }
                    }
                    if (!isok)
                        newData.Add(od);
                }

            }
            return newData;
        }

        /// <summary>
        /// 判断是否全是操作
        /// </summary>
        /// <returns></returns>
        public bool IsOperation()
        {
            bool isok = true;
            foreach (CAMObject ct in this.ProgramGroup.GetMembers())
            {
                if (!(ct is NXOpen.CAM.Operation))
                {
                    isok = false;
                    break;
                }
            }
            return isok;
        }
        /// <summary>
        ///获取程序组
        /// </summary>
        /// <returns></returns>
        public List<NCGroup> GetProgram()
        {
            List<NCGroup> programGroup = new List<NCGroup>();
            foreach (CAMObject ct in this.ProgramGroup.GetMembers())
            {
                if (ct is NCGroup)
                {
                    programGroup.Add(ct as NCGroup);
                }
            }
            return programGroup;
        }
        /// <summary>
        /// 过切检查
        /// </summary>
        /// <returns></returns>
        public List<string> Gouged()
        {
            List<string> err = new List<string>();
            UFSession theUFSession = UFSession.GetUFSession();
            foreach (OperationData od in this.OperData)
            {
                if (od.Oper.GougeCheckStatus)
                    continue;
                bool result = false;
                try
                {
                    theUFSession.Oper.IsPathGouged(od.Oper.Tag, out result);
                   // result = true;
                }
                catch
                {

                }
                if (result)
                    err.Add(od.OperName + "        过切！");
            }
            return err;

        }
        /// <summary>
        /// 获取最大最小Z向深度
        /// </summary>
        /// <param name="data"></param>
        /// <param name="zMin"></param>
        /// <param name="zMax"></param>
        public void GetOperationZMinAndZMax(List<OperationData> data, out double zMin, out double zMax)
        {
            zMin = 0;
            zMax = -9999;
            foreach (OperationData da in data)
            {
                if (da.Zmax > zMax)
                    zMax = da.Zmax;
                if (da.Zmin < zMin)
                    zMin = da.Zmin;
            }
        }
        public int CompareTo(ProgramModel other)
        {
            return this.ProgramGroup.Name.CompareTo(other.ProgramGroup.Name);
        }


    }
}
