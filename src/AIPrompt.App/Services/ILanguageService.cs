namespace AIPrompt.App.Services;

public interface ILanguageService
{
    string GetString(string key);

    void ApplyLanguage(string cultureCode);
}
