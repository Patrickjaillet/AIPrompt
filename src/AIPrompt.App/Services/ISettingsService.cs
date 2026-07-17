namespace AIPrompt.App.Services;

public interface ISettingsService
{
    string? DefaultExportFolder { get; }

    string? DefaultRoadmapExportFolder { get; }

    string BackupsFolder { get; }

    bool AutoBackupEnabled { get; }

    int AutoBackupIntervalMinutes { get; }

    ThemeMode ThemeMode { get; }

    string AccentColor { get; }

    string Language { get; }

    Task SetDefaultExportFolderAsync(string folder, CancellationToken cancellationToken = default);

    Task SetDefaultRoadmapExportFolderAsync(string folder, CancellationToken cancellationToken = default);

    Task SetDefaultBackupFolderAsync(string folder, CancellationToken cancellationToken = default);

    Task SetAutoBackupAsync(bool enabled, int intervalMinutes, CancellationToken cancellationToken = default);

    Task SetThemeModeAsync(ThemeMode mode, CancellationToken cancellationToken = default);

    Task SetAccentColorAsync(string accentColor, CancellationToken cancellationToken = default);

    Task SetLanguageAsync(string language, CancellationToken cancellationToken = default);
}
