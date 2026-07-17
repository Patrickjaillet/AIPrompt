using System.Collections.ObjectModel;
using AIPrompt.App.Services;
using AIPrompt.Core.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AIPrompt.App.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly ISettingsService _settingsService;
    private readonly IDialogService _dialogService;
    private readonly ThemeService _themeService;
    private readonly ILanguageService _languageService;
    private readonly IBackupService _backupService;
    private bool _isInitializing = true;

    [ObservableProperty]
    private string _defaultExportFolder;

    [ObservableProperty]
    private string _defaultRoadmapExportFolder;

    [ObservableProperty]
    private string _defaultBackupFolder;

    [ObservableProperty]
    private ThemeMode _selectedThemeMode;

    [ObservableProperty]
    private string _selectedAccentColor;

    [ObservableProperty]
    private string _selectedLanguage;

    [ObservableProperty]
    private string _languageNotice = string.Empty;

    public SettingsViewModel(
        ISettingsService settingsService,
        IDialogService dialogService,
        ThemeService themeService,
        ILanguageService languageService,
        IBackupService backupService)
    {
        _settingsService = settingsService;
        _dialogService = dialogService;
        _themeService = themeService;
        _languageService = languageService;
        _backupService = backupService;

        _defaultExportFolder = settingsService.DefaultExportFolder ?? string.Empty;
        _defaultRoadmapExportFolder = settingsService.DefaultRoadmapExportFolder ?? string.Empty;
        _defaultBackupFolder = settingsService.BackupsFolder;
        _selectedThemeMode = settingsService.ThemeMode;
        _selectedAccentColor = settingsService.AccentColor;
        _selectedLanguage = settingsService.Language;

        _themeService.ApplyThemeMode(_selectedThemeMode);
        _themeService.ApplyAccentColor(_selectedAccentColor);
        _languageService.ApplyLanguage(_selectedLanguage);

        _isInitializing = false;
    }

    public IReadOnlyList<ThemeMode> ThemeModes { get; } = [ThemeMode.Light, ThemeMode.Dark, ThemeMode.System];

    public IReadOnlyList<string> AccentColors { get; } = ThemeService.AvailableAccentColors;

    public ObservableCollection<string> Languages { get; } = ["fr", "en"];

    partial void OnSelectedThemeModeChanged(ThemeMode value)
    {
        _themeService.ApplyThemeMode(value);
        if (!_isInitializing)
        {
            _ = _settingsService.SetThemeModeAsync(value);
        }
    }

    partial void OnSelectedAccentColorChanged(string value)
    {
        _themeService.ApplyAccentColor(value);
        if (!_isInitializing)
        {
            _ = _settingsService.SetAccentColorAsync(value);
        }
    }

    partial void OnSelectedLanguageChanged(string value)
    {
        _languageService.ApplyLanguage(value);

        if (_isInitializing)
        {
            return;
        }

        _ = _settingsService.SetLanguageAsync(value);
        LanguageNotice = _languageService.GetString("Settings_LanguageAppliedNotice");
    }

    [RelayCommand]
    private async Task BrowseExportFolderAsync()
    {
        var folder = _dialogService.ShowFolderPicker(DefaultExportFolder);
        if (folder is null)
        {
            return;
        }

        DefaultExportFolder = folder;
        await _settingsService.SetDefaultExportFolderAsync(folder);
    }

    [RelayCommand]
    private async Task BrowseRoadmapExportFolderAsync()
    {
        var folder = _dialogService.ShowFolderPicker(DefaultRoadmapExportFolder);
        if (folder is null)
        {
            return;
        }

        DefaultRoadmapExportFolder = folder;
        await _settingsService.SetDefaultRoadmapExportFolderAsync(folder);
    }

    [RelayCommand]
    private async Task BrowseBackupFolderAsync()
    {
        var folder = _dialogService.ShowFolderPicker(DefaultBackupFolder);
        if (folder is null)
        {
            return;
        }

        DefaultBackupFolder = folder;
        await _settingsService.SetDefaultBackupFolderAsync(folder);
    }

    [RelayCommand]
    private async Task ResetDatabaseAsync()
    {
        var firstConfirm = _dialogService.ShowConfirmation(
            _languageService.GetString("Dialog_ResetDbTitle"),
            _languageService.GetString("Dialog_ResetDbMessage"));

        if (!firstConfirm)
        {
            return;
        }

        var secondConfirm = _dialogService.ShowConfirmation(
            _languageService.GetString("Dialog_ResetDbFinalTitle"),
            _languageService.GetString("Dialog_ResetDbFinalMessage"));

        if (!secondConfirm)
        {
            return;
        }

        await _backupService.ResetDatabaseAsync();
    }
}
