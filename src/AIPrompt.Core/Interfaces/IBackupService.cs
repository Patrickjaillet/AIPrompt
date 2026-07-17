using AIPrompt.Core.Models;

namespace AIPrompt.Core.Interfaces;

public interface IBackupService
{
    Task<string> ExportToJsonAsync(CancellationToken cancellationToken = default);

    Task ImportFromJsonAsync(string json, ImportMode mode, CancellationToken cancellationToken = default);

    Task ResetDatabaseAsync(CancellationToken cancellationToken = default);
}
