using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AsuAsopFilesProcessing
{
    internal class FileProcessing
    {

        //словарь перевозчиков (код маршрута - замена)
        Dictionary<string, string> perevoz = new Dictionary<string, string>()
            {
                { "20", "609"},
                { "42", "612"},
                { "51", "609"},
                { "55", "607"},
                { "58", "612"},
                { "59", "610"},
                { "72", "611"},
                { "73", "610"},
                { "89", "606"}
            };


        public void Process(string FileName) 
        {

            Logger log = new Logger();
            string current_date = "";
            string logstring = "";
            
            
            //===========================================================================================
            //1 - проверка наличия папки сохранения. Если папка не найдена - то создать папку
            //===========================================================================================
            string currdir = Directory.GetCurrentDirectory();
            //поиск директории и ее создание если не найдена
            string target = currdir + @"\ProcessedFiles";
            if (!Directory.Exists(target))
            {
                try
                {
                    Directory.CreateDirectory(target);
                }
                catch (Exception ex)
                {
                    current_date = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                    logstring = " ОШИБКА!!! Не удалось создать директорию сохранения выходных файлов. Сообщение о ошибке: "+ex.ToString();
                    log.SaveToLogFile(current_date+logstring);
                }
                
            }


            //===========================================================================================
            //2 - подготовка 15 и 16 колонки, которые одинаковы для всех строк
            //===========================================================================================

            List<string> oslines = new List<string>();

            //колонки константы 15 и 16

            //15 колонка имеет одинаковое значение всегда
            string col15 = @"0001;";
            string col16 = "";

            //16 колонка
            //извлекаем дату
            string pattern = @"\b\d{6}\b"; // Регулярное выражение для поиска последовательности из 6 цифр
            Match match = Regex.Match(FileName, pattern);

            if (match.Success)
            {

                string dateStr = match.Value;
                
                if (DateTime.TryParseExact(dateStr, "yyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime date))
                {
                    //Console.WriteLine("Извлеченная дата: " + date.ToString("dd.MM.yyyy"));
                    col16 = date.ToString("dd.MM.yyyy");
                }
                else
                {
                    current_date = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                    logstring = " ОШИБКА!!! Файл: " + FileName + " -  Невозможно извлечь дату из названия файла. ";
                    log.SaveToLogFile(current_date + logstring);
                    //Console.WriteLine("Невозможно извлечь дату из названия файла.");
                }


            }
            else
            {
                current_date = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                logstring = " ОШИБКА!!! Файл: " + FileName + " -  Не найдено совпадений с датой в названии файла.";
                log.SaveToLogFile(current_date + logstring);
                //Console.WriteLine("Не найдено совпадений с датой в названии файла.");
            }




            //===========================================================================================
            //3 - читаем входной файл и формируем все остальные колонки
            //===========================================================================================
            StreamReader reader = new StreamReader(FileName, Encoding.GetEncoding("windows-1252"));//GetEncoding("UTF-8")
            var encoding = reader.CurrentEncoding;
            while (!reader.EndOfStream)
            {
                
                string line = reader.ReadLine();
                var values = line.Split(';');
                int count_col = values.Count();


                //обработка пятой колонки. убираем нули и сохраняем целые числа
                string colString = values[5].Replace('"', ' ').Trim();
                colString = colString.Replace('.', ',');
                double newvalue = double.Parse(colString);
                values[5] = newvalue.ToString();


                //формируем строку из значений прочитаных из файла. форматируем строку под нужный формат
                string srow = "";
                for (int i = 0; i < count_col; i++)
                {
                    srow += @"" + values[i].Replace('"', ' ').Trim() + @";";
                }


                //получаем значение 14 колонки из таблицы соответсвий и вставляем в строку новую колонку
                string pvalue = "";
                if (perevoz.TryGetValue(values[3].Replace('"', ' ').Trim(), out pvalue))
                {
                    
                    //Console.WriteLine("For key = " + values[3].Replace('\"', ' ').Trim() + ", value = {0}.", pvalue);
                }
                else
                {

                    current_date = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                    logstring = " ОШИБКА!!! Файл: " + FileName + " -  Не найдено значению по ключу "+ values[3].Replace('\"', ' ').Trim() + " в таблице соответсвий перевозчиков. ";
                    log.SaveToLogFile(current_date + logstring);

                    //Console.WriteLine("Key = " + values[3].Replace('\"', ' ').Trim() + " is not found.");
                }
                srow += @"" + pvalue + @";";


                srow += col15 + col16;


                oslines.Add(srow);


            }

            //===========================================================================================
            //4 - сохраняем полученый результат в выходной файл
            //===========================================================================================

            //имя файла для сохранения           
            string namefile = Path.GetFileNameWithoutExtension(FileName);
            string save_filename = namefile + " cor " + @".csv";

            FileStream fs = null;
            fs = new FileStream(target + @"\" + save_filename, FileMode.Create);
            //FileInfo f = new FileInfo(target + @"\" + save_filename);

            //StreamWriter sw = f.AppendText();

            try
            {
                //StreamWriter sw = f.CreateText();
                StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("windows-1252"));

                foreach (string line in oslines)
                {
                    sw.WriteLine(line);
                }
                sw.Close();
            }
            catch (Exception ex)
            {
                current_date = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                logstring = " ОШИБКА!!! Не удалось сохранить выходной файл с именем "+ save_filename+". Сообщение о ошибке: " + ex.ToString();
                log.SaveToLogFile(current_date + logstring);
            }
            



        }//конец процедуры процессинга обработки



    }
}
