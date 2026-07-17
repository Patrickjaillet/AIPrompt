using AIPrompt.Core.Models;

namespace AIPrompt.Core.Interfaces;

public interface IPromptCategoryRepository
{
    Task<PromptCategoryModel> AddAsync(PromptCategoryModel model, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PromptCategoryModel>> GetAllAsync(CancellationToken cancellationToken = default);

    Task UpdateAsync(PromptCategoryModel model, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
