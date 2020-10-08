using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MolexPlugin.Model;
using MolexPlugin.DLL;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 通过序列文件获得Control数据
    /// </summary>
    public class ControlDeserialize
    {
        private static List<ControlEnum> controls = new List<ControlEnum>();
        public static List<ControlEnum> Controls
        {
            get
            {
                if (controls.Count == 0 || controls == null)
                {
                    controls = Deserialize();
                }

                return controls;

            }
        }

        private ControlDeserialize()
        {

        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <returns></returns>
        private static List<ControlEnum> Deserialize()
        {
            string dllPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string contrPath = dllPath.Replace("application\\", "Cofigure\\SerializeContr.dat");
            if (File.Exists(contrPath))
            {
                FileStream fs = new FileStream(contrPath, FileMode.Open, FileAccess.Read);             
                BinaryFormatter bf = new BinaryFormatter();
                List<ControlEnum> control = bf.Deserialize(fs) as List<ControlEnum>;
                fs.Close();
                return control;
            }
            return null;
        }
    }
}
