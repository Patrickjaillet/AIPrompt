using AIPrompt.Core.Models;

namespace AIPrompt.Core.Interfaces;

public interface ISavedPromptRepository
{
    Task<SavedPromptModel> AddAsync(SavedPromptModel model, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SavedPromptModel>> GetAllAsync(CancellationToken cancellationToken = default);
}
