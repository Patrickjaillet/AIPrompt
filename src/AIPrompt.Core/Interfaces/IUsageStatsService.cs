using AIPrompt.Core.Models;

namespace AIPrompt.Core.Interfaces;

public interface IUsageStatsService
{
    Task<UsageStatsModel> GetStatsAsync(int topCount = 5, CancellationToken cancellationToken = default);
}
