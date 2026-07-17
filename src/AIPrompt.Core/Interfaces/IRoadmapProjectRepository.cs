using AIPrompt.Core.Models;

namespace AIPrompt.Core.Interfaces;

public interface IRoadmapProjectRepository
{
    Task<RoadmapProjectModel> AddAsync(RoadmapProjectModel model, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RoadmapProjectModel>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<RoadmapProjectModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
