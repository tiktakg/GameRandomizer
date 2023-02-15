using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using WpfColorFontDialog;
using System.IO;
using ModernWpf.Controls;

namespace GameRandomizer
{
    /// <summary>
    /// Interaction logic for ShellWindow.xaml
    /// </summary>
    public partial class ShellWindow : Window
    {
        private DispatcherTimer Timer;
        private int Counter;
        private int LimitInSeconds;
        private double FillingStep;
        public ShellWindow()
        {
            InitializeComponent();
            SettingComponents();
        }

        private void SettingComponents()
        { 
            ToggleRolfButton.Checked += (s, e) => MainTabItem.Header = "Random Gay";
            ToggleRolfButton.Unchecked += (s, e) => MainTabItem.Header = "Random Game";

            Logo.Source = Tools.PathToImage(Sources.MainLogo());
            LogoSettings.Source = Tools.PathToImage(Sources.MainLogo());

            LogoInProgressBar.Source = Tools.PathToImage(Sources.ProgressBarImage());

            FontInfo.ApplyFont(MainTabItem, Tools.GetFont());
            HeadLineText.FontSize += 10d;
            HeadLineText.Text = Tools.GetHeadLineText();
            HeadLineSettings.Text = Tools.GetHeadLineText();

            PhrasesTextBlock.FontSize += 5d;

            StartRandomButton.Content = Tools.GetStartButtonText();
            ButtonSettings.Text = Tools.GetStartButtonText();

            SlowRButton.Content = Tools.GetSlowButtonText();
            FastRButton.Content = Tools.GetFastButtonText();

            ModeSettings1.Text = Tools.GetSlowButtonText();
            ModeSettings2.Text = Tools.GetFastButtonText();

            LimitInSeconds = Tools.GetTimeLimit();

            FillingStep = 100d / LimitInSeconds;

            RingProgressBar.Foreground = Tools.GetProgressBarFillingColor();
            SimpleProgressBar.Foreground = Tools.GetProgressBarFillingColor();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }

        private void StartRandomButton_Click(object sender, RoutedEventArgs e)
        {
            Timer = new DispatcherTimer(DispatcherPriority.Render);
            Timer.Tick += Timer_Tick;
            if ((bool)FastRButton.IsChecked)
                EndOfRandomization();
            else
            {
                StartOfRandomization();
                ProgressRing.IsActive = true;
                Timer.Interval = new TimeSpan(0, 0, 1);
                Timer.Start();
            }

        }
        private void ChangeVisibleOfRandomizationControls(Visibility visibility)
        {
            RingProgressBar.Visibility = visibility;
            SimpleProgressBar.Visibility = visibility;
            ProgressRing.Visibility = visibility;
            WaitTextBlock.Visibility = visibility;
            PhrasesTextBlock.Visibility = visibility;
            LogoInProgressBar.Visibility = visibility;
        }
        private void StartOfRandomization()
        {
            ChangeVisibleOfRandomizationControls(Visibility.Visible);

            TextBlockToGameOutput.Text = "";
        }
        private void EndOfRandomization()
        {
            ChangeVisibleOfRandomizationControls(Visibility.Collapsed);

            TextBlockToGameOutput.Text = Tools.GetGameWithMode();
        }
        private void UpdateWaitTextBlock()
        {
            string LastSymb = WaitTextBlock.Text.Substring(21);
            switch (LastSymb)
            {
                case "":
                    WaitTextBlock.Text += ".";
                    break;
                case ".":
                    WaitTextBlock.Text += ".";
                    break;
                case "..":
                    WaitTextBlock.Text += ".";
                    break;
                case "...":
                    WaitTextBlock.Text = WaitTextBlock.Text.Substring(0, 21);
                    break;
            }
        }
        private void UpdateProgressControls()
        {
            RingProgressBar.Value += FillingStep;
            SimpleProgressBar.Value += FillingStep;
        }
        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (Counter == LimitInSeconds)
            {
                Timer.Stop();
                EndOfRandomization();
            }
            Counter++;
            UpdateWaitTextBlock();
            UpdateProgressControls();
        }
        private void LogoClick1(object? sender, EventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Document"; // Default file name
            dialog.DefaultExt = ".png"; // Default file extension
            dialog.Filter = "PNG|*.png|JPEG|*.jpeg|BMP|*.bmp|TIFF|*.tiff|GIF|*.gif"; // Filter files by extension

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                
                
                File.Delete(Directory.GetCurrentDirectory() + @"\Sources\Images\MainLogo.png");
                File.Copy(dialog.FileName, Directory.GetCurrentDirectory() + @"\Sources\Images\MainLogo.png");
                InvalidateVisual();
                SettingComponents();

            }
            
        }
        private void ApplySettings(object? sender, EventArgs e)
        {
            InitializeComponent();
        }
    }
}
