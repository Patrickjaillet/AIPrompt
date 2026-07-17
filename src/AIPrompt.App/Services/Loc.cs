using System.ComponentModel;

namespace AIPrompt.App.Services;

public class Loc : INotifyPropertyChanged
{
    public static Loc Instance { get; } = new();

    private ILanguageService? _languageService;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string this[string key] => _languageService?.GetString(key) ?? key;

    public void Initialize(ILanguageService languageService)
    {
        _languageService = languageService;
    }

    public void Refresh()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
    }
}
