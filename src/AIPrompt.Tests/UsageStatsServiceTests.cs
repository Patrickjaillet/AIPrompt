using AIPrompt.Data;
using AIPrompt.Data.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AIPrompt.Tests;

public class UsageStatsServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AIPromptDbContext _context;
    private readonly UsageStatsService _usageStatsService;

    public UsageStatsServiceTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AIPromptDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AIPromptDbContext(options);
        _context.Database.EnsureCreated();

        _usageStatsService = new UsageStatsService(_context);
    }

    [Fact]
    public async Task GetStatsAsync_RanksCategoriesAndTermsByUsageCount()
    {
        var codeCategory = new PromptCategory { Name = "Code", Description = "Code prompts", IconKey = "CodeTags" };
        var marketingCategory = new PromptCategory { Name = "Marketing", Description = "Marketing prompts", IconKey = "Bullhorn" };
        var genre = new PromptGenre { Name = "Technique", Description = "Technical genre" };
        _context.PromptCategories.AddRange(codeCategory, marketingCategory);
        _context.PromptGenres.Add(genre);
        await _context.SaveChangesAsync();

        var now = DateTime.UtcNow;
        _context.TermPhrases.AddRange(
            new TermPhrase { Content = "Écris une fonction", CategoryId = codeCategory.Id, GenreId = genre.Id, UsageCount = 10, CreatedAt = now, UpdatedAt = now },
            new TermPhrase { Content = "Corrige ce bug", CategoryId = codeCategory.Id, GenreId = genre.Id, UsageCount = 5, CreatedAt = now, UpdatedAt = now },
            new TermPhrase { Content = "Rédige un slogan", CategoryId = marketingCategory.Id, GenreId = genre.Id, UsageCount = 1, CreatedAt = now, UpdatedAt = now },
            new TermPhrase { Content = "Jamais utilisé", CategoryId = marketingCategory.Id, GenreId = genre.Id, UsageCount = 0, CreatedAt = now, UpdatedAt = now });
        await _context.SaveChangesAsync();

        var stats = await _usageStatsService.GetStatsAsync(topCount: 2);

        Assert.Equal(2, stats.TotalCategories);
        Assert.Equal(4, stats.TotalTerms);
        Assert.Equal(2, stats.TopCategories.Count);
        Assert.Equal("Code", stats.TopCategories[0].CategoryName);
        Assert.Equal(15, stats.TopCategories[0].UsageCount);
        Assert.Equal(2, stats.MostUsedTerms.Count);
        Assert.Equal("Écris une fonction", stats.MostUsedTerms[0].Content);
        Assert.DoesNotContain(stats.MostUsedTerms, term => term.Content == "Jamais utilisé");
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
