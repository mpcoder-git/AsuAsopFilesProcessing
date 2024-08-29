using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsuAsopFilesProcessing
{
    internal class Logger
    {

        public void SaveToLogFile(string logtext)
        {

            string currdir = Directory.GetCurrentDirectory();

            //поиск директории и ее создание если не найдена
            string target = currdir + @"\Logs";
            if (!Directory.Exists(target))
            {
                Directory.CreateDirectory(target);
            }

            string filename_logstr = DateTime.Now.ToString("dd_MM_yyyy");
            string filename_log = filename_logstr + @".txt";


            FileInfo f = new FileInfo(target + @"\" + filename_log);
            StreamWriter sw = f.AppendText(); // Для порождения
                                              // объекта StreamWriter
            sw.WriteLine(logtext);

            sw.Close();


        }




        public void SaveListToFile(List<string> list)
        {
            if (list.Count > 0)
            {

                SaveToLogFile("=======================================");
                SaveToLogFile("Дополнительная информация по файлу: ");
                SaveToLogFile("=======================================");
                foreach (var item in list)
                {
                    SaveToLogFile(item);
                }
                SaveToLogFile("=======================================");
            }
        }




    }
}
