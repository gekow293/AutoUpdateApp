using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Запуск обновления...");

            string appDomain = AppDomain.CurrentDomain.BaseDirectory;

            string[] parts = appDomain.Split(new string[] { "\\" }, StringSplitOptions.None);

            var root = String.Join("\\", parts.Take(parts.Length - 5));

            var proc = new ProcessStartInfo();
            proc.WorkingDirectory = root + @"\WpfApp\WpfApp\bin\Release";
            proc.FileName = "WpfApp.exe";

            while (true)
            {
                Console.WriteLine("Ожидание завершения процесса WpfApp.exe");

                Thread.Sleep(5000);

                if (!Process.GetProcessesByName("WpfApp.exe").Any())
                {
                    Console.WriteLine("процесс WpfApp.exe завершен");
                    
                    if (Directory.Exists(root + @"\WpfApp"))
                        Directory.Delete(root + @"\WpfApp", true);
                    if (Directory.Exists(root + @"\UpdateWpfApp"))
                        Directory.Move(root + @"\UpdateWpfApp", root + @"\WpfApp");

                    Console.WriteLine("Запускается обновленная версия программы");

                    Process proc1 = new Process();
                    proc1.StartInfo.WorkingDirectory = root + @"\WpfApp\WpfApp\bin\Release";
                    proc1.StartInfo.FileName = "WpfApp.exe";
                    proc1.Start();

                    break;
                }
                else
                {
                    Console.WriteLine("Процесс WpfApp.exe не завершен");
                }
            }

            Console.WriteLine("Нажмите любую клавишу для выхода");
            Console.ReadKey();
        }
    }
}
