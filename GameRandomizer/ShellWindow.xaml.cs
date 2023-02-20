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
using System.Printing;

namespace GameRandomizer
{
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
            HeadLineText.Text = Tools.GetHeadLineText("Заголовок");

            SaveFontSize.Text = Convert.ToString(Tools.GetFontSize());

            PhrasesTextBlock.FontSize += 5d;

          
            StartRandomButton.Content = Tools.GetHeadLineText("КнопкаНачала");
            ButtonSettings.Text = Tools.GetHeadLineText("КнопкаНачала");

            SlowRButton.Content = Tools.GetHeadLineText("МедленнаяКнопка");
            FastRButton.Content = Tools.GetHeadLineText("БыстраяКнопка");
            fastMode.Text = Tools.GetHeadLineText("БыстраяКнопка");
            slowMode.Text = Tools.GetHeadLineText("МедленнаяКнопка");

            FastRButton.Foreground = Tools.GetProgressBarFillingColor("BrushColor:");
            SlowRButton.Foreground = Tools.GetProgressBarFillingColor("BrushColor:");

            LimitInSeconds = Tools.GetTimeLimit();

            FillingStep = 100d / LimitInSeconds;

            RingProgressBar.Foreground = Tools.GetProgressBarFillingColor("ЦветШкалыПрогресса:");
            SimpleProgressBar.Foreground = Tools.GetProgressBarFillingColor("ЦветШкалыПрогресса:");
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
                RingProgressBar.Value = 0;
                SimpleProgressBar.Value = 0;
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
            UpdatePhrases();
        }
        private void UpdatePhrases()
        {
            Random rnd = new Random();

            string[] textforPhrases = File.ReadAllLines(Sources.Phrases());
            PhrasesTextBlock.Text = textforPhrases[rnd.NextInt64(0, textforPhrases.Length)];

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
        private void SaveHead_Click(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SaveText(SaveTextForHead.Text, "Заголовок:");
                HeadLineText.Text = SaveTextForHead.Text;
                SaveTextForHead.Text = "";
            }
        }

        private void SavePhrases_Click(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string textForPhrases = SaveTextForPhrases.Text;
                if (textForPhrases != "")
                    File.AppendAllText(Sources.Phrases(), "\n" + textForPhrases);

                SaveTextForPhrases.Text = "";
            }
                
        }
        private void SaveGame_Click(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter) 
            {

                Regex regex = new Regex(@"(.*)\{(.*)\}");

                string textForGame = SaveTextForGame.Text;

                if (textForGame != "")
                {
                    if (regex.IsMatch(textForGame))
                    {
                        File.AppendAllText(Sources.Games(), "\n" + textForGame);
                    }
                }

                SaveTextForGame.Text = "";

            }
        }
        private void SaveFontSize_Click(object sender, KeyEventArgs e) 
        {
            if (e.Key== Key.Enter && Int32.TryParse(SaveFontSize.Text,out int t))
            {
                string[] allText = File.ReadAllLines(Sources.Font());
                string textForFontSize = SaveFontSize.Text;

                Regex regex = new Regex(@"(.*)\:(.*)");
                int fontSize = int.Parse(regex.Match(allText.Single(x => x.StartsWith("Size"))).Groups[2].Value.Trim());

                if (t > 0 && t <= 64)
                {

                    File.WriteAllText(Sources.Font(), "");

                    for (int i = 0; i < allText.Length; ++i)
                    {
                        if (allText[i].StartsWith("Size:"))
                        {
                            allText[i] = "Size:" + textForFontSize;
                            break;
                        }
                    }

                    for (int i = 0; i < allText.Length; ++i)
                    {
                        File.AppendAllText(Sources.Font(), allText[i] + "\n");
                    }
                
                    if (fontSize > Convert.ToInt32(textForFontSize))
                    {
                        HeadLineText.FontSize -= 10d;
                        PhrasesTextBlock.FontSize -= 5d;
                    }
                    else if(fontSize < Convert.ToInt32(textForFontSize))
                    {
                        HeadLineText.FontSize += 10d;
                        PhrasesTextBlock.FontSize += 5d;
                    }

                    FontInfo.ApplyFont(MainTabItem, Tools.GetFont());
                }
            }
            
        }
        private void ChangeFontColor_Click(object? sender, EventArgs e)
        {
            string[] allText = File.ReadAllLines(Sources.Font());

            SlowRButton.Foreground =  new SolidColorBrush((Color)ColorConverter.ConvertFromString(ClrPicker.SelectedColor.ToString()));
            FastRButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ClrPicker.SelectedColor.ToString()));



            File.WriteAllText(Sources.Font(), "");

            for (int i = 0; i < allText.Length; ++i)
            {
                if (allText[i].StartsWith("BrushColor:"))
                {
                    allText[i] = "BrushColor:" + ClrPicker.SelectedColor.ToString();
                    break;
                }
            }

            for (int i = 0; i < allText.Length; ++i)
            {
                File.AppendAllText(Sources.Font(), allText[i] + "\n");
            }

            FontInfo.ApplyFont(MainTabItem, Tools.GetFont());   
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SaveText(fastMode.Text, "БыстраяКнопка:");
            SaveText(slowMode.Text, "МедленнаяКнопка:");

            FastRButton.Content = fastMode.Text;
            SlowRButton.Content = slowMode.Text;
        }

        private void ButtonSettings_TextChanged(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SaveText(ButtonSettings.Text, "КнопкаНачала:");
                StartRandomButton.Content = ButtonSettings.Text;
            }  
        }

        private void SaveText(string textForSave,string textforSearch)
        {
            string[] allText = File.ReadAllLines(Sources.ElementTexts());

            File.WriteAllText(Sources.ElementTexts(), "");

            for (int i = 0; i < allText.Length; ++i)
            {

                if (allText[i].StartsWith(textforSearch))
                {
                    allText[i] = textforSearch + textForSave;
                    break;
                }
            }

           
            for (int i = 0; i < allText.Length; ++i)
            {
                File.AppendAllText(Sources.ElementTexts(), allText[i] + "\n");
            }

        }
    }
}
