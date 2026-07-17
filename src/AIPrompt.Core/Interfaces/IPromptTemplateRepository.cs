using AIPrompt.Core.Models;

namespace AIPrompt.Core.Interfaces;

public interface IPromptTemplateRepository
{
    Task<PromptTemplateModel> AddAsync(PromptTemplateModel model, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PromptTemplateModel>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<PromptTemplateModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
