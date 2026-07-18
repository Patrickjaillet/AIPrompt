using AIPrompt.Core.Models;

namespace AIPrompt.Core.Interfaces;

public interface ITermPhraseRepository
{
    Task<TermPhraseModel> AddAsync(TermPhraseModel model, CancellationToken cancellationToken = default);

    Task<TermPhraseModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TermPhraseModel>> GetAllAsync(CancellationToken cancellationToken = default);

    Task UpdateAsync(TermPhraseModel model, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task IncrementUsageAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TermPhraseModel>> SearchAsync(string query, CancellationToken cancellationToken = default);
}
