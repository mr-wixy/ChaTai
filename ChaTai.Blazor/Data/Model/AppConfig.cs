using OpenAI.GPT3;

namespace ChaTai.Blazor.Data
{
    public class AppConfig
    {
        public bool TightBorder { get; set; } = false;

        public string Secret { get; set; }

        public string ApiKey { get; set; }

        public string ProxyUrl { get; set; }

        public ShortcutKey ShortcutKey { get; set; } = ShortcutKey.Enter;

        public Theme Theme { get; set; } = Theme.Auto;

        public int HistoryMessageCount { get; set; } = 4;

        public string Avatar { get; set; } = "/icons/my.svg";

    }
}
