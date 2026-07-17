using System.IO;
using System.Text.Json;

namespace AIPrompt.App.Services;

public class SettingsService : ISettingsService
{
    private readonly string _settingsFilePath;
    private AppSettings _settings;

    public SettingsService(string appDataDirectory)
    {
        _settingsFilePath = Path.Combine(appDataDirectory, "settings.json");
        _settings = Load(_settingsFilePath);
        BackupsFolder = Path.Combine(appDataDirectory, "backups");
        Directory.CreateDirectory(BackupsFolder);
    }

    public string? DefaultExportFolder => _settings.DefaultExportFolder;

    public string BackupsFolder { get; }

    public bool AutoBackupEnabled => _settings.AutoBackupEnabled;

    public int AutoBackupIntervalMinutes => _settings.AutoBackupIntervalMinutes;

    public async Task SetDefaultExportFolderAsync(string folder, CancellationToken cancellationToken = default)
    {
        _settings.DefaultExportFolder = folder;
        await SaveAsync(cancellationToken);
    }

    public async Task SetAutoBackupAsync(bool enabled, int intervalMinutes, CancellationToken cancellationToken = default)
    {
        _settings.AutoBackupEnabled = enabled;
        _settings.AutoBackupIntervalMinutes = intervalMinutes;
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
