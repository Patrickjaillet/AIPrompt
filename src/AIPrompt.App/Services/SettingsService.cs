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
    }

    public string? DefaultExportFolder => _settings.DefaultExportFolder;

    public async Task SetDefaultExportFolderAsync(string folder, CancellationToken cancellationToken = default)
    {
        _settings.DefaultExportFolder = folder;
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
