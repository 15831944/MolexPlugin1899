using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    public abstract class AbstractCreateAssmbile
    {
        protected MoldInfo moldInfo;
        protected UserModel userModel;
        protected List<AbstractAssmbileModel> models = new List<AbstractAssmbileModel>();
        public AbstractCreateAssmbile(MoldInfo mold, UserModel user)
        {
            this.moldInfo = mold;
            this.userModel = user;
        }
        /// <summary>
        /// 新建装配档
        /// </summary>
        /// <param name="directoryPath">文件夹位置需夹\\</param>
        /// <returns></returns>
        public virtual List<string> CreatePart(string directoryPath)
        {
            List<string> err = new List<string>();
            foreach (AbstractAssmbileModel am in models)
            {
                if (am.PartTag == null)
                    if (!am.CreatePart(directoryPath))
                        err.Add(am.AssembleName + "创建失败");
            }
            return err;
        }
        /// <summary>
        /// 装配
        /// </summary>
        /// <returns></returns>
        public virtual List<string> LoadAssmbile()
        {
            List<string> err = new List<string>();
            foreach (AbstractAssmbileModel am in models)
            {
                string name = am.AssembleName;
                try
                {
                    am.Load();
                }
                catch
                {
                    err.Add(name + "装配失败");
                }
            }
            return err;
        }
    }
}
