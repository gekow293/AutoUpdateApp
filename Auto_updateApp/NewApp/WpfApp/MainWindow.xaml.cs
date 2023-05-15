using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using File = System.IO.File;

namespace WpfApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string appDomain = AppDomain.CurrentDomain.BaseDirectory;

        string rootPathNewApp;

        string rootPathUpdateApp;

        string root;

        // Текущая версия проекта, доступная для всего проекта
        string currVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public MainWindow()
        {
            string[] parts = appDomain.Split(new string[] { "\\" }, StringSplitOptions.None);

            root = String.Join("\\", parts.Take(parts.Length - 5));

            rootPathNewApp = root + @"\NewApp";

            rootPathUpdateApp = root + @"\UpdateWpfApp";

            InitializeComponent();

            version.Text = "Версия приложения " + currVersion;


            new Thread(CheckForUpdate).Start();
            //CheckForUpdate();
        }

        // запуск обновляющего приложения
        private void StartUpdater()
        {
            try
            {
                // Использование системных методов для запуска программы
                Process proc = new Process();
                proc.StartInfo.WorkingDirectory = root + @"\ConsoleUpdater\ConsoleUpdater\bin\Release";
                proc.StartInfo.FileName = "ConsoleUpdater.exe";
                proc.Start(); // Запускаем!
            }
            catch (Exception ex)
            {
            }
        }

        // проверить наличие новой версии приложения в локальной папке
        private void CheckForUpdate()
        {
            var up_version = GetOtherVersion();
            var cur_version = new Version(currVersion);

            if (cur_version.CompareTo(up_version) < 0) // Если нужно обновление
            {
                Thread.Sleep(2000);

                MessageBoxResult result = MessageBox.Show("Обновить приложение?", "Обнаружена новая версия приложения", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Dispatcher.Invoke(new Action(() => { label_status.Text = $"Обновление..."; }), DispatcherPriority.Render);

                    Thread.Sleep(3000);

                    UpdateFileAndRerun();
                    Process.GetCurrentProcess().Kill();
                }
                else
                {
                    Dispatcher.Invoke(new Action(() => { label_status.Text = ""; }), DispatcherPriority.Render);
                }
            }
            else
            {
                Dispatcher.Invoke(new Action(() => { label_status.Text = $"Запущена актуальная версия приложения ({currVersion})"; }), DispatcherPriority.Render);
            }
        }

        // проверка версии образцовой программы
        private Version GetOtherVersion()
        {
            try
            {
                string text = File.ReadAllText(rootPathNewApp + @"\WpfApp\Properties\AssemblyInfo.cs");
                Match match = new Regex("AssemblyVersion\\(\"(.*?)\"\\)").Match(text);
                return new Version(match.Groups[1].Value);
            }
            catch
            {
                return new Version(currVersion);  // Если номер версии не можем получить вернем текущую вверсию
            }
        }

        // копирование программы и запуск обновления
        private void UpdateFileAndRerun()
        {
            if (!Directory.Exists(rootPathUpdateApp))
                Directory.CreateDirectory(rootPathUpdateApp);
            CopyingProgramFiles();
            StartUpdater();
        }

        private void CopyingProgramFiles()
        {
            try
            {
                //Создать идентичное дерево каталогов
                foreach (string dirPath in Directory.GetDirectories(rootPathNewApp, "*", SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(rootPathNewApp, rootPathUpdateApp));

                //Скопировать все файлы. И перезаписать(если такие существуют)
                foreach (string newPath in Directory.GetFiles(rootPathNewApp, "*.*", SearchOption.AllDirectories))
                    File.Copy(newPath, newPath.Replace(rootPathNewApp, rootPathUpdateApp), true);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
