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
            if(e.Key ==Key.Enter) 
            {
                string[] allText = File.ReadAllLines(Sources.ElementTexts());
                string textForHead = SaveTextForHead.Text;

                if (textForHead != "")
                {
                    HeadLineText.Text = textForHead;

                    File.WriteAllText(Sources.ElementTexts(), "");

                    for (int i = 0; i < allText.Length; ++i)
                    {

                        if (allText[i].StartsWith("Заголовок:"))
                        {
                            allText[i] = "Заголовок:" + textForHead;
                            break;
                        }
                    }

                    for (int i = 0; i < allText.Length; ++i)
                    {
                        File.AppendAllText(Sources.ElementTexts(), allText[i] + "\n");
                    }
                }

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
                string textForHead = SaveFontSize.Text;

                if (t > 0 && t <= 64)
                {
                    File.WriteAllText(Sources.Font(), "");

                    for (int i = 0; i < allText.Length; ++i)
                    {
                        if (allText[i].StartsWith("Size:"))
                        {
                            allText[i] = "Size:" + textForHead;
                            break;
                        }
                    }

                    for (int i = 0; i < allText.Length; ++i)
                    {
                        File.AppendAllText(Sources.Font(), allText[i] + "\n");
                    }

                    InitializeComponent();
                    SettingComponents();
                }
            }
            
        }
        private void ChangeFontColor_Click(object? sender, EventArgs e)
        {
            string[] allText = File.ReadAllLines(Sources.Font());
            string textForHead = SaveFontSize.Text;

            MessageBox.Show(ClrPicker.SelectedColor.ToString());

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
                File.AppendAllText(Sources.Font(), allText[i] + "\n");
            

            InitializeComponent();
            SettingComponents();
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string[] allText = File.ReadAllLines(Sources.ElementTexts());

            string fastModeText = fastMode.Text;
            string slowModeText = slowMode.Text;

            if ((slowModeText != "") & (fastModeText != ""))
            {
                FastRButton.Content = fastModeText;
                SlowRButton.Content = slowModeText;

                File.WriteAllText(Sources.ElementTexts(), "");

                for (int i = 0; i < allText.Length; ++i)
                {
                    if (allText[i].StartsWith("БыстраяКнопка:"))
                    {
                        allText[i] = "БыстраяКнопка:" + fastModeText;
                    }


                    if (allText[i].StartsWith("МедленнаяКнопка:"))
                    {
                        allText[i] = "МедленнаяКнопка:" + slowModeText;
                    }    
                        
                }

                File.WriteAllText(Sources.ElementTexts(), "");

                for (int i = 0; i < allText.Length; ++i)
                {
                    File.AppendAllText(Sources.ElementTexts(), allText[i] + "\n");
                }
            }

        }

     
    }
}
