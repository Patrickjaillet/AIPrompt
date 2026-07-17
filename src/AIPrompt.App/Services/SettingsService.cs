using System.IO;
using System.Text.Json;

namespace AIPrompt.App.Services;

public class SettingsService : ISettingsService
{
    private readonly string _settingsFilePath;
    private readonly string _defaultBackupsFolder;
    private AppSettings _settings;

    public SettingsService(string appDataDirectory)
    {
        _settingsFilePath = Path.Combine(appDataDirectory, "settings.json");
        _settings = Load(_settingsFilePath);
        _defaultBackupsFolder = Path.Combine(appDataDirectory, "backups");
        Directory.CreateDirectory(BackupsFolder);
    }

    public string? DefaultExportFolder => _settings.DefaultExportFolder;

    public string? DefaultRoadmapExportFolder => _settings.DefaultRoadmapExportFolder;

    public string BackupsFolder => string.IsNullOrWhiteSpace(_settings.DefaultBackupFolder)
        ? _defaultBackupsFolder
        : _settings.DefaultBackupFolder;

    public bool AutoBackupEnabled => _settings.AutoBackupEnabled;

    public int AutoBackupIntervalMinutes => _settings.AutoBackupIntervalMinutes;

    public ThemeMode ThemeMode => Enum.TryParse<ThemeMode>(_settings.ThemeMode, out var mode) ? mode : ThemeMode.Light;

    public string AccentColor => _settings.AccentColor;

    public string Language => _settings.Language;

    public async Task SetDefaultExportFolderAsync(string folder, CancellationToken cancellationToken = default)
    {
        _settings.DefaultExportFolder = folder;
        await SaveAsync(cancellationToken);
    }

    public async Task SetDefaultRoadmapExportFolderAsync(string folder, CancellationToken cancellationToken = default)
    {
        _settings.DefaultRoadmapExportFolder = folder;
        await SaveAsync(cancellationToken);
    }

    public async Task SetDefaultBackupFolderAsync(string folder, CancellationToken cancellationToken = default)
    {
        _settings.DefaultBackupFolder = folder;
        Directory.CreateDirectory(BackupsFolder);
        await SaveAsync(cancellationToken);
    }

    public async Task SetAutoBackupAsync(bool enabled, int intervalMinutes, CancellationToken cancellationToken = default)
    {
        _settings.AutoBackupEnabled = enabled;
        _settings.AutoBackupIntervalMinutes = intervalMinutes;
        await SaveAsync(cancellationToken);
    }

    public async Task SetThemeModeAsync(ThemeMode mode, CancellationToken cancellationToken = default)
    {
        _settings.ThemeMode = mode.ToString();
        await SaveAsync(cancellationToken);
    }

    public async Task SetAccentColorAsync(string accentColor, CancellationToken cancellationToken = default)
    {
        _settings.AccentColor = accentColor;
        await SaveAsync(cancellationToken);
    }

    public async Task SetLanguageAsync(string language, CancellationToken cancellationToken = default)
    {
        _settings.Language = language;
        await SaveAsync(cancellationToken);
    }

    private async Task SaveAsync(CancellationToken cancellationToken)
    {
        await using var stream = File.Create(_settingsFilePath);
        await JsonSerializer.SerializeAsync(stream, _settings, cancellationToken: cancellationToken);
    }

    private static AppSettings Load(string path)
    {
        if (!File.Exists(path))
        {
            return new AppSettings();
        }

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
    }
}
