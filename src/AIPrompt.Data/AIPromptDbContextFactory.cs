using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AIPrompt.Data;

public class AIPromptDbContextFactory : IDesignTimeDbContextFactory<AIPromptDbContext>
{
    public AIPromptDbContext CreateDbContext(string[] args)
    {
        var dataDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "AIPrompt");

        Directory.CreateDirectory(dataDirectory);

        var databasePath = Path.Combine(dataDirectory, "aiprompt.db");

        var optionsBuilder = new DbContextOptionsBuilder<AIPromptDbContext>();
        optionsBuilder.UseSqlite($"Data Source={databasePath}");

        return new AIPromptDbContext(optionsBuilder.Options);
    }
}
