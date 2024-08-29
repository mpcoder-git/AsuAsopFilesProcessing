using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsuAsopFilesProcessing
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Программа преобразования необработанных файлов csv в формат, подходящий для загрузки программой AsuAsopLoader");
            
            //===========================================================================================
            //1 - загрузка всех файлов из папки при первом запуске
            //===========================================================================================
            string scandir = Settings.Default.ScanDir;
            List<string> listfiles = new List<string>();
            FilesScanner fs = new FilesScanner();           
            listfiles = fs.GetFilesToLoad();

            FileProcessing fp = new FileProcessing();


            //перебор файлов и отправка по одному на вставку
            foreach (string fname in listfiles)
            {

                fp.Process(scandir+@"\"+fname);
                //Console.WriteLine(scandir+@"\"+fname);

            }


        }
    }
}
