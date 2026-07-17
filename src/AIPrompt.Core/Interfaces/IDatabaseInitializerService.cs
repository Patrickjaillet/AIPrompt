namespace AIPrompt.Core.Interfaces;

public interface IDatabaseInitializerService
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}
