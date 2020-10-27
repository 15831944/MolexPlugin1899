using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Basic;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 电极程序文件操作类
    /// </summary>
    public class ElectrodeCAMFile
    {
        private string camFile;
        private string eleFile = "";

        public string EleFile { get { return eleFile; } }
        public ElectrodeCAMFile()
        {
            camFile = "C:\\temp\\Electrode\\";
            if (!Directory.Exists(camFile))
            {
                Directory.CreateDirectory(camFile);
            }
        }
        /// <summary>
        /// 移动文件
        /// </summary>
        /// <returns></returns>
        public List<string> CopyFile()
        {
            List<string> filePath = new List<string>();
            List<string> oldPath = OpenFileUtils.OpenFiles("添加工件", "部件文件(*.prt*)|");
            if (oldPath.Count > 0)
            {
                eleFile = camFile + GetTimeStamp() + "\\";
                if (!Directory.Exists(eleFile))
                {
                    Directory.CreateDirectory(eleFile);
                }
                foreach (string st in oldPath)
                {
                    string temp = Path.GetFileName(st);
                    File.Copy(st, eleFile + temp);
                    filePath.Add(eleFile + temp);
                }
            }
            return filePath;
        }
        /// <summary>
        /// 添加文件
        /// </summary>
        /// <param name="filePh"></param>
        /// <returns></returns>
        public List<string> AddFile(string filePh)
        {
            List<string> filePath = new List<string>();
            List<string> oldPath = OpenFileUtils.OpenFiles("添加工件", "部件文件(*.prt*)|");
            if (oldPath.Count > 0)
            {
                if (!Directory.Exists(filePh))
                {
                    return filePath;
                }
                foreach (string st in oldPath)
                {
                    string temp = Path.GetFileName(st);
                    File.Copy(st, filePh + temp);
                    filePath.Add(filePh + temp);
                }
            }
            return filePath;
        }
        /// <summary>
        /// 保存文件地址
        /// </summary>
        /// <returns></returns>
        public string SaveFilePath()
        {
            string filePath = OpenFileUtils.OpenFolderBrowser();
            if (filePath != null)
            {
                WriteSaveFilePath(filePath);
            }
            return filePath;
        }

        /// <summary>
        /// 获得记事本地址位置
        /// </summary>
        /// <returns></returns>
        public string GetSaveFilePath()
        {
            string fileName = camFile + "SaveFile\\ElectrodeSavePath.dat";
            if (File.Exists(fileName))
            {
                StreamReader sr = new StreamReader(fileName, Encoding.Default);
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    return line;
                }
            }
            return "";
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath"></param>
        public void DeleteFile()
        {

            if (Directory.Exists(this.eleFile))
            {
                try
                {
                    Directory.Delete(this.eleFile, true);
                }
                catch
                {

                }
            }
        }
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        private string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }
        /// <summary>
        /// 写入保存文件位置
        /// </summary>
        /// <param name="path"></param>
        private void WriteSaveFilePath(string path)
        {
            string filePath = camFile + "SaveFile\\";
            string fileName = filePath + "ElectrodeSavePath.dat";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            FileStream fs = File.Create(fileName);
            byte[] data = System.Text.Encoding.Default.GetBytes(path);
            //开始写入
            fs.Write(data, 0, data.Length);
            //清空缓冲区、关闭流
            fs.Flush();
            fs.Close();
        }
    }
}
