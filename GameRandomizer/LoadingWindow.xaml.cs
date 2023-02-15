using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;

namespace GameRandomizer
{
    /// <summary>
    /// Interaction logic for LoadingWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {
        DispatcherTimer timer;
        int counter;
        public LoadingWindow()
        {
            InitializeComponent();
            SetImage();
            CheckForFile();
            if (ReadLaunch())
            {
                SettingUpFileds();
                ChangeLaunch();
                timer.Start();
            }
            else
            {
                ShellWindow mainWindow = new ShellWindow();
                App.Current.MainWindow = mainWindow;
                mainWindow.Show();
                Close();
            }

        }

        private void SetImage() => LogoImage.Source = Tools.PathToImage(Sources.MainLogo());

        private void CheckForFile()
        {
            if (!File.Exists(Sources.Launch()))
            {
                File.Create(Sources.Launch());
                File.WriteAllText(Sources.Launch(), "True");
            }
        }

        private bool ReadLaunch()
        {
            bool IsFirst;
            string TextFromFile = File.ReadAllText(Sources.Launch());
            if (string.IsNullOrEmpty(TextFromFile))
                return true;
            if (!bool.TryParse(TextFromFile, out IsFirst))
                return true;
            return IsFirst;
        }

        private void ChangeLaunch()
        {
            File.WriteAllTextAsync(Sources.Launch(), false.ToString());
        }

        private void SettingUpFileds()
        {
            timer = new DispatcherTimer(DispatcherPriority.Render);
            timer.Interval = new System.TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;
            counter = 0;
        }

        private void Timer_Tick(object? sender, System.EventArgs e)
        {
            if (counter == 3)
            { 
                timer.Stop();
                Close();
                new ShellWindow().Show();
            }
            counter++;
        }
    }
}
