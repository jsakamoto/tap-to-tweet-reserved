namespace TapToTweetReserved.Client;

public class SupportedLanguage
{
    public readonly string Caption;

    public readonly string LangCode;

    public SupportedLanguage(string caption, string langCode)
    {
        this.Caption = caption;
        this.LangCode = langCode;
    }

    public static readonly IReadOnlyList<SupportedLanguage> List = new[] {
        new SupportedLanguage("English", "en"),
        new SupportedLanguage("日本語", "ja"),
    };
}
