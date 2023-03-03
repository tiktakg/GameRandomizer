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
using Microsoft.VisualBasic;

namespace GameRandomizer
{
    public partial class ShellWindow : Window
    {
        private ColorFontDialog dialog;
        private DispatcherTimer Timer;
        private FontInfo font;
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
            //SetButonFont();

            ToggleRolfButton.Checked += (s, e) => MainTabItem.Header = "Random Gay";
            ToggleRolfButton.Unchecked += (s, e) => MainTabItem.Header = "Random Game";

            Logo.Source = Tools.PathToImage(Sources.MainLogo());
            LogoSettings.Source = Tools.PathToImage(Sources.MainLogo());
            LogoSettings_2.Source = Tools.PathToImage(Sources.ProgressBarImage());
            
            LogoInProgressBar.Source = Tools.PathToImage(Sources.ProgressBarImage());

            FontInfo.ApplyFont(MainTabItem, Tools.GetFont());
           

            HeadLineText.FontSize += 10d;
            HeadLineText.Text = Tools.GetHeadLineText("Заголовок");

            PhrasesTextBlock.FontSize += 5d;

          
            StartRandomButton.Content = Tools.GetHeadLineText("КнопкаНачала");
            ButtonSettings.Text = Tools.GetHeadLineText("КнопкаНачала");

            SlowRButton.Content = Tools.GetHeadLineText("МедленнаяКнопка");
            FastRButton.Content = Tools.GetHeadLineText("БыстраяКнопка");
            fastMode.Text = Tools.GetHeadLineText("БыстраяКнопка");
            slowMode.Text = Tools.GetHeadLineText("МедленнаяКнопка");

            LimitInSeconds = Tools.GetTimeLimit();

            FillingStep = 100d / LimitInSeconds;

            RingProgressBar.Foreground = Tools.GetProgressBarFillingColor("ЦветШкалыПрогресса:",Sources.ElementTexts());
            SimpleProgressBar.Foreground = Tools.GetProgressBarFillingColor("ЦветШкалыПрогресса:", Sources.ElementTexts());

            InitGameScroll();
            InitPhrasesScroll();

        }
        private void InitGameScroll()
        {
            string[] games = File.ReadAllLines(Sources.Games());

            GameComboBox.Items.Clear();

            Canvas canvas= new Canvas();

            Label[] label = new Label[games.Length];
            Button[] buttons = new Button[games.Length];
            Grid[] grids= new Grid[games.Length];


            for(int i = 0; i < label.Length;i++) 
            {
                grids[i] = new Grid();
                label[i] = new Label();
                buttons[i] = new Button();
            }


            for(int i = 0; i < games.Length; i++) 
            {
                label[i].Content = games[i];
                buttons[i].Content = "❌";
                label[i].HorizontalAlignment= HorizontalAlignment.Left;
                buttons[i].HorizontalAlignment = HorizontalAlignment.Right;
                buttons[i].Name = $"_{i}";
                buttons[i].Click += DeleteGame;


                buttons[i].Margin = new Thickness(0,0,10,0);

                buttons[i].Width = 40;

                GameComboBox.Items.Add(grids[i]);



                grids[i].Children.Add(buttons[i]);
                grids[i].Margin = new Thickness(0, 2, 0, 0);
                grids[i].Children.Add(label[i]);

                Canvas.SetZIndex(buttons[i], 1);
                Canvas.SetZIndex(label[i], -1);

               
            }
            
            
        }
        private void DeleteGame(object sender, RoutedEventArgs e)
        {
            string[] games = File.ReadAllLines(Sources.Games());
            File.WriteAllText(Sources.Games(), $"");

            string name = (sender as Button).Name;

            

            StreamWriter file = new StreamWriter(Sources.Games());
            for(int i = 0;i < games.Length;i++) 
            {
                if (i == Convert.ToInt64(name.Replace("_", "")))
                    continue;
                file.WriteLine(games[i]);
            }

            file.Close();
            InitGameScroll();
        }

        private void InitPhrasesScroll()
        {
            string[] games = File.ReadAllLines(Sources.Phrases());

            PhrasesComboBox.Items.Clear();

            Canvas canvas = new Canvas();

            Label[] label = new Label[games.Length];
            Button[] buttons = new Button[games.Length];
            Grid[] grids = new Grid[games.Length];

            


            for (int i = 0; i < label.Length; i++)
            {
                grids[i] = new Grid();
                label[i] = new Label();
                buttons[i] = new Button();
            }


            for (int i = 0; i < games.Length; i++)
            {
                label[i].Content = games[i];
                buttons[i].Content = "❌";
                label[i].HorizontalAlignment = HorizontalAlignment.Left;
                buttons[i].HorizontalAlignment = HorizontalAlignment.Right;
                buttons[i].Name = $"__{i}";
                buttons[i].Click += DeletePhrase;


                buttons[i].Margin = new Thickness(0, 0, 10, 0);

                buttons[i].Width = 40;


                PhrasesComboBox.Items.Add(grids[i]);

                grids[i].Children.Add(buttons[i]);
                grids[i].Margin = new Thickness(0, 2, 0, 0);
                grids[i].Children.Add(label[i]);

                Canvas.SetZIndex(buttons[i], 1);
                Canvas.SetZIndex(label[i], -1);
            }


        }

        private void DeletePhrase(object sender, RoutedEventArgs e)
        {
            string[] games = File.ReadAllLines(Sources.Phrases());
            File.WriteAllText(Sources.Games(), $"");

            string name = (sender as Button).Name;



            StreamWriter file = new StreamWriter(Sources.Phrases());
            for (int i = 0; i < games.Length; i++)
            {
                if (i == Convert.ToInt64(name.Replace("__", "")))
                    continue;
                file.WriteLine(games[i]);
            }

            file.Close();
            InitPhrasesScroll();
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
                Counter = 0;
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
            try
            {
                PhrasesTextBlock.Text = textforPhrases[rnd.NextInt64(0, textforPhrases.Length)];
            }
            catch
            {

            }
            

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
        private void LogoClick2(object? sender, EventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Document"; // Default file name
            dialog.DefaultExt = ".png"; // Default file extension
            dialog.Filter = "PNG|*.png|JPEG|*.jpeg|BMP|*.bmp|TIFF|*.tiff|GIF|*.gif"; // Filter files by extension

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                File.Delete(Directory.GetCurrentDirectory() + @"\Sources\Images\ProgressBarImage.png");
                File.Copy(dialog.FileName, Directory.GetCurrentDirectory() + @"\Sources\Images\ProgressBarImage.png");
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
                Tools.SaveText(SaveTextForHead.Text, "Заголовок:", Sources.ElementTexts());
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
                    File.AppendAllText(Sources.Phrases(), textForPhrases + "\n");

                SaveTextForPhrases.Text = "";

                InitPhrasesScroll();
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
                        File.AppendAllText(Sources.Games(),  textForGame + "\n");
                    }
                }

                SaveTextForGame.Text = "";
                InitGameScroll();
            }
        }
        private void VinTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SaveTextForGame.Text == "Игра {Режим_1, Режим_2, ...}")
                SaveTextForGame.Text = "";
            SaveTextForGame.Foreground = new SolidColorBrush(Colors.White);
        }

        private void VinTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SaveTextForGame.Text))
            {
                SaveTextForGame.Text = "Игра {Режим_1, Режим_2, ...}";
                SaveTextForGame.Foreground = new SolidColorBrush(Colors.White);
            }
        }

       
        private void ChangeFontColor_Click(object? sender, EventArgs e)
        {
            string[] allText = File.ReadAllLines(Sources.ElementTexts());

            if(ClrPicker.SelectedColor.ToString() != "")
            {
                SimpleProgressBar.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ClrPicker.SelectedColor.ToString()));
                RingProgressBar.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ClrPicker.SelectedColor.ToString()));


                Tools.SaveText(ClrPicker.SelectedColor.ToString(), "ЦветШкалыПрогресса:", Sources.ElementTexts());

                File.WriteAllText(Sources.ElementTexts(), "");

                for (int i = 0; i < allText.Length; ++i)
                {
                    if (allText[i].StartsWith("ЦветШкалыПрогресса:"))
                    {
                        allText[i] = "ЦветШкалыПрогресса:" + ClrPicker.SelectedColor.ToString();
                        break;
                    }
                }

                for (int i = 0; i < allText.Length; ++i)
                {
                    File.AppendAllText(Sources.ElementTexts(), allText[i] + "\n");
                }

                FontInfo.ApplyFont(MainTabItem, Tools.GetFont());
            }
           
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Tools.SaveText(fastMode.Text, "БыстраяКнопка:", Sources.ElementTexts());
            Tools.SaveText(slowMode.Text, "МедленнаяКнопка:", Sources.ElementTexts());

            FastRButton.Content = fastMode.Text;
            SlowRButton.Content = slowMode.Text;
        }

        private void ButtonSettings_TextChanged(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Tools.SaveText(ButtonSettings.Text, "КнопкаНачала:", Sources.ElementTexts());
                StartRandomButton.Content = ButtonSettings.Text;
            }  
        }
        
        private void LogoSettings_MouseEnter(object sender, MouseEventArgs e)
        {
            LogoSettingsLabel.Content = "Выбрать лого";
        }
        private void LogoSettings_MouseLeave(object sender, MouseEventArgs e)
        {
            LogoSettingsLabel.Content = "";
        }

        private void LogoSettings2_MouseEnter(object sender, MouseEventArgs e)
        {
            LogoSettingsLabel2.Content = "Выбрать лого";
        }
        private void LogoSettings2_MouseLeave(object sender, MouseEventArgs e)
        {
            LogoSettingsLabel2.Content = "";
        }

        private void ChangeFont_Click(object sender, RoutedEventArgs e)
        {
            dialog = new();
            dialog.Font = FontInfo.GetControlFont(FontButton);
            dialog.FontSizes = new int[] { 10, 12, 14, 16, 18, 20, 22 };
            if (dialog.ShowDialog() == true)
            {
                font = dialog.Font;
                if (font != null)
                {
                    

                    File.WriteAllText(Sources.Font(), $"");

                    StreamWriter file = new StreamWriter(Sources.Font());
                    file.WriteLine($"BrushColor:{font.Color.Name}");
                    file.WriteLine($"Family:{font.Family}");
                    file.WriteLine($"Size:{font.Size}");
                    file.WriteLine($"Style:{font.Style}");
                    file.WriteLine($"Stretch:{font.Stretch}");
                    file.WriteLine($"Weight:{font.Weight}");
                    file.Close();


                    SetButonFont();
                    FontInfo.ApplyFont(MainTabItem, Tools.GetFont());

                }
            }
        }

        private void SaveTimeProgressbar_Click(object sender, RoutedEventArgs e)
        {
            if(Int32.TryParse(TextForSaveTimeProgressbar.Text,out int Time))
            {
                Tools.SaveText(TextForSaveTimeProgressbar.Text, "Время:", Sources.ElementTexts());

                LimitInSeconds = Time;
                FillingStep = 100d / Time;

                TextForSaveTimeProgressbar.Text = "";
            }
           
        }

        private void SetButonFont()
        {
            FontInfo fi = Tools.GetFont();

            SlowRButton.Foreground = Tools.GetProgressBarFillingColor("BrushColor:", Sources.Font());
            SlowRButton.FontFamily = fi.Family;
            SlowRButton.FontSize = fi.Size;
            SlowRButton.FontStyle = fi.Style;
            SlowRButton.FontStretch = fi.Stretch;
            SlowRButton.Width = Tools.GetFontSize();

            FastRButton.Foreground = Tools.GetProgressBarFillingColor("BrushColor:", Sources.Font());
            FastRButton.FontFamily = fi.Family;
            FastRButton.FontSize = fi.Size;
            FastRButton.FontStyle = fi.Style;
            FastRButton.FontStretch = fi.Stretch;
            FastRButton.Width = Tools.GetFontSize();;
        }

    }
}
