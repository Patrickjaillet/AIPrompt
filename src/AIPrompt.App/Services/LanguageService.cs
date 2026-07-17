using System.Globalization;
using System.Resources;

namespace AIPrompt.App.Services;

public class LanguageService : ILanguageService
{
    private readonly ResourceManager _resourceManager = new("AIPrompt.App.Resources.Strings", typeof(LanguageService).Assembly);

    public string GetString(string key)
    {
        return _resourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key;
    }

    public void ApplyLanguage(string cultureCode)
    {
        var culture = new CultureInfo(cultureCode);
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }
}
