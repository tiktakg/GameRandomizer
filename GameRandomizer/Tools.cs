using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfColorFontDialog;

namespace GameRandomizer
{
    internal static class Tools
    {
        public static ImageSource PathToImage(string Path)
        {
            using (MemoryStream memoryStream =
                new MemoryStream(File.ReadAllBytes(Path)))
            {
                return BitmapFrame.Create(memoryStream,
                    BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
        }
        public static FontInfo GetFont()
        {
            string[] Settings = File.ReadAllLines(Sources.Font());
            Regex regex = new Regex(@"(.*)\:(.*)");
            FontInfo fi = new FontInfo();

            #region FontFamily

            string FontName = regex.Match(Settings.Single(x => x.StartsWith("Family"))).Groups[2].Value.Trim();
            fi.Family = (FontFamily)new FontFamilyConverter().ConvertFromString(FontName);

            #endregion

            #region Color

            string ColorName = regex.Match(Settings.Single(x => x.StartsWith("BrushColor"))).Groups[2].Value.Trim();
            PropertyInfo ColorPropInfo = typeof(Colors).GetProperty(ColorName);
            Color ColorFromColors = (Color)ColorPropInfo?.GetValue(null);
            fi.BrushColor = new SolidColorBrush(new Color()
            {
                A = ColorFromColors.A,
                R = ColorFromColors.R,
                G = ColorFromColors.G,
                B = ColorFromColors.B
            });

            #endregion

            #region Size

            fi.Size = double.Parse(regex.Match(Settings.Single(x => x.StartsWith("Size"))).Groups[2].Value.Trim());

            #endregion

            #region Style

            string StyleName = regex.Match(Settings.Single(x => x.StartsWith("Style"))).Groups[2].Value.Trim();
            PropertyInfo StylePropInfo = typeof(FontStyles).GetProperty(StyleName);
            FontStyle StyleFromStyles = (FontStyle)StylePropInfo?.GetValue(null);
            fi.Style = StyleFromStyles;

            #endregion

            #region Stretch

            string StretchName = regex.Match(Settings.Single(x => x.StartsWith("Stretch"))).Groups[2].Value.Trim();
            PropertyInfo StretchPropInfo = typeof(FontStretches).GetProperty(StretchName);
            FontStretch StretchFromStretchs = (FontStretch)StretchPropInfo?.GetValue(null);
            fi.Stretch = StretchFromStretchs;

            #endregion

            #region Weight

            string WeightName = regex.Match(Settings.Single(x => x.StartsWith("Weight"))).Groups[2].Value.Trim();
            PropertyInfo WeightPropInfo = typeof(FontWeights).GetProperty(WeightName);
            FontWeight WeightFromWeights = (FontWeight)WeightPropInfo?.GetValue(null);
            fi.Weight = WeightFromWeights;

            #endregion

            /*
            fi.Weight
            fi.Family
            fi.Stretch
            fi.Style
            fi.Size
            fi.Color
            */
            return fi;
        }
        public static string GetHeadLineText(string text)
        {
            return new Regex(@"(.*)\:(.*)")
                .Match(File.ReadAllLines(Sources.ElementTexts())
                .Single(x => x.StartsWith(text)))
                .Groups[2]
                .Value
                .Trim();
        }
        public static int GetTimeLimit()
        {
            return int
                .Parse(new Regex(@"(.*)\:(.*)")
                .Match(File.ReadAllLines(Sources.ElementTexts()).Single(x => x.StartsWith("Время")))
                .Groups[2]
                .Value
                .Trim());
        }
        public static string GetGameWithMode()
        {
            string[] AllGames = File.ReadAllLines(Sources.Games());
            Random rnd = new Random();
            string RandomedGame = AllGames[rnd.NextInt64(0, AllGames.Length)];
            Regex regex = new Regex(@"(.*)\{(.*)\}");
            string GameName = regex
                .Match(RandomedGame)
                .Groups[1]
                .Value
                .Trim();
            string[] GameModes = regex
                .Match(RandomedGame)
                .Groups[2]
                .Value
                .Split(',')
                .ToList()
                .Select(x => x.Trim())
                .ToArray();
            string GameMode = GameModes[rnd.NextInt64(0, GameModes.Length)];
            return $"{GameName} : {GameMode}";
        }
        public static SolidColorBrush GetProgressBarFillingColor()
        {
            string ColorInHex = new Regex(@"(.*)\:(.*)")
                .Match(File.ReadAllLines(Sources.ElementTexts()).Single(x => x.StartsWith("ЦветШкалыПрогресса")))
                .Groups[2]
                .Value
                .Trim();
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorInHex));
        }
    }
}
