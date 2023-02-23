using System.IO;

namespace GameRandomizer
{
    internal static class Sources
    {
        private static readonly string Path;
        static Sources()
        {
            Path = Directory.GetCurrentDirectory() + @"\Sources\";
        }
        internal static string Font() => Path + @"Data\Font.txt";
        internal static string Games() => Path + @"Data\Games.txt";
        internal static string Launch() => Path + @"Data\Launch.txt";
        internal static string Phrases() => Path + @"Data\Phrases.txt";
        internal static string ElementTexts() => Path + @"Data\ElementTexts.txt";
        internal static string MainLogo() => Path + @"\Images\MainLogo.png";
        internal static string MainLogoSettings() => Path + @"\Images\settingImg1.png";
        internal static string ProgressBarImage() => Path + @"\Images\ProgressBarImage.png";
    }
}
