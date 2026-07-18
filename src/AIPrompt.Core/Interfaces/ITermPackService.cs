using AIPrompt.Core.Models;

namespace AIPrompt.Core.Interfaces;

public interface ITermPackService
{
    Task<TermPackImportResult> ImportPackAsync(string json, CancellationToken cancellationToken = default);

    Task<string> ExportPackAsync(int categoryId, string packName, CancellationToken cancellationToken = default);
}
