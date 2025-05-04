using YG;

namespace Utils 
{
    public static class Translator
    {
        public static string Translate(string ru, string en, string tr)
        {
            if (ru == null || en == null || tr == null)
                return string.Empty;

            return YandexGame.EnvironmentData.language switch
            {
                "ru" => ru.Trim(),
                "en" => en.Trim(),
                "tr" => tr.Trim(),
                _ => en.Trim(),
            };
        }
    }
}