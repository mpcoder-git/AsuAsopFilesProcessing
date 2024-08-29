using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AsuAsopFilesProcessing
{
    internal class FilesScanner
    {
        string scandir = Settings.Default.ScanDir;

        public List<string> GetFilesToLoad()
        {

            List<string> list_filesinscandir = new List<string>();
            
            DirectoryInfo dir = new DirectoryInfo(scandir);
            foreach (var d in dir.GetFiles("*.csv"))
            {
                
                //список нужен, чтобы сравнивать с другими списками при необходимости
                list_filesinscandir.Add((d.Name).ToString());
                
            }

            return list_filesinscandir;

        }
    
    
    
    }
}
