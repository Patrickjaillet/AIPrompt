namespace AIPrompt.App.Services;

public interface ISettingsService
{
    string? DefaultExportFolder { get; }

    Task SetDefaultExportFolderAsync(string folder, CancellationToken cancellationToken = default);
}
