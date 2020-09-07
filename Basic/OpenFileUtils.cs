using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Basic
{
    public class OpenFileUtils
    {
        /// <summary>
        /// 单选文件
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="filter">过滤文件格式</param>
        /// <returns></returns>
        public static string OpenFile(string title, string filter)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = filter;
            dialog.AutoUpgradeEnabled = true;
            dialog.Title = title;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.FileName;
            }
            return null;
        }
        /// <summary>
        /// 多选文件
        /// </summary>
        /// <param name="title"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<string> OpenFiles(string title, string filter)
        {
            List<string> path = new List<string>();
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.Filter = filter;
            dialog.AutoUpgradeEnabled = true;
            dialog.Title = title;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                path.AddRange(dialog.FileNames);
            }
            return path;
        }
        /// <summary>
        /// 选择文件夹
        /// </summary>
        /// <returns></returns>
        public static string OpenFolderBrowser()
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();

            folder.Description = "选择目录";
            if (folder.ShowDialog() == DialogResult.OK)
            {
                //文件夹路径
                return folder.SelectedPath;

            }
            return null;
        }
    }

}
