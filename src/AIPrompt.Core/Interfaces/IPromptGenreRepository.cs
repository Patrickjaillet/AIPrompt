using AIPrompt.Core.Models;

namespace AIPrompt.Core.Interfaces;

public interface IPromptGenreRepository
{
    Task<PromptGenreModel> AddAsync(PromptGenreModel model, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PromptGenreModel>> GetAllAsync(CancellationToken cancellationToken = default);

    Task UpdateAsync(PromptGenreModel model, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
