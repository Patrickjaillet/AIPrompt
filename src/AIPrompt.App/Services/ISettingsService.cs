namespace AIPrompt.App.Services;

public interface ISettingsService
{
    string? DefaultExportFolder { get; }

    string BackupsFolder { get; }

    bool AutoBackupEnabled { get; }

    int AutoBackupIntervalMinutes { get; }

    Task SetDefaultExportFolderAsync(string folder, CancellationToken cancellationToken = default);

    Task SetAutoBackupAsync(bool enabled, int intervalMinutes, CancellationToken cancellationToken = default);
}
