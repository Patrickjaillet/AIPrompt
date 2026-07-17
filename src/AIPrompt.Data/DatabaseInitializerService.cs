using AIPrompt.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AIPrompt.Data;

public class DatabaseInitializerService : IDatabaseInitializerService
{
    private readonly AIPromptDbContext _context;

    public DatabaseInitializerService(AIPromptDbContext context)
    {
        _context = context;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await _context.Database.MigrateAsync(cancellationToken);
        SeedData.Apply(_context);
    }
}
