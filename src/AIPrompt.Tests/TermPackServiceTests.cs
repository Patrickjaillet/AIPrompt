using AIPrompt.Core.Models;
using AIPrompt.Data;
using AIPrompt.Data.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AIPrompt.Tests;

public class TermPackServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AIPromptDbContext _context;
    private readonly TermPackService _termPackService;

    public TermPackServiceTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AIPromptDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AIPromptDbContext(options);
        _context.Database.EnsureCreated();

        _termPackService = new TermPackService(_context);
    }

    [Fact]
    public async Task ImportPackAsync_CreatesCategoryGenreAndTerms_WhenNoneExist()
    {
        var json = System.Text.Json.JsonSerializer.Serialize(new TermPackModel
        {
            PackName = "Marketing Starter",
            CategoryName = "Marketing",
            CategoryDescription = "Marketing prompts",
            CategoryIconKey = "Bullhorn",
            Items =
            [
                new TermPackItemModel { Content = "Rédige un slogan percutant", GenreName = "Créatif", Tags = "slogan", Language = "FR" },
                new TermPackItemModel { Content = "Write a catchy tagline", GenreName = "Creative", Tags = "tagline", Language = "EN" }
            ]
        });

        var result = await _termPackService.ImportPackAsync(json);

        Assert.Equal("Marketing", result.CategoryName);
        Assert.Equal(2, result.ImportedCount);
        Assert.Equal(0, result.SkippedCount);
        Assert.Equal(1, await _context.PromptCategories.CountAsync());
        Assert.Equal(2, await _context.PromptGenres.CountAsync());
        Assert.Equal(2, await _context.TermPhrases.CountAsync());
    }

    [Fact]
    public async Task ImportPackAsync_SkipsDuplicateTermsWithinCategory()
    {
        var category = new PromptCategory { Name = "Code", Description = "Code prompts", IconKey = "CodeTags" };
        var genre = new PromptGenre { Name = "Technique", Description = "Technical genre" };
        _context.PromptCategories.Add(category);
        _context.PromptGenres.Add(genre);
        await _context.SaveChangesAsync();
        _context.TermPhrases.Add(new TermPhrase
        {
            Content = "Écris une fonction",
            CategoryId = category.Id,
            GenreId = genre.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        var json = System.Text.Json.JsonSerializer.Serialize(new TermPackModel
        {
            PackName = "Code Pack",
            CategoryName = "Code",
            Items =
            [
                new TermPackItemModel { Content = "Écris une fonction", GenreName = "Technique" },
                new TermPackItemModel { Content = "Corrige ce bug", GenreName = "Technique" }
            ]
        });

        var result = await _termPackService.ImportPackAsync(json);

        Assert.Equal(1, result.ImportedCount);
        Assert.Equal(1, result.SkippedCount);
        Assert.Equal(2, await _context.TermPhrases.CountAsync());
        Assert.Equal(1, await _context.PromptCategories.CountAsync());
    }

    [Fact]
    public async Task ExportPackAsync_ThenImportPackAsync_RoundTripsTerms()
    {
        var category = new PromptCategory { Name = "Support", Description = "Support prompts", IconKey = "LifeBuoy" };
        var genre = new PromptGenre { Name = "Direct", Description = "Direct genre" };
        _context.PromptCategories.Add(category);
        _context.PromptGenres.Add(genre);
        await _context.SaveChangesAsync();
        _context.TermPhrases.Add(new TermPhrase
        {
            Content = "Comment puis-je vous aider ?",
            CategoryId = category.Id,
            GenreId = genre.Id,
            Tags = "accueil",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        var exportedJson = await _termPackService.ExportPackAsync(category.Id, "Support Pack");

        await _context.TermPhrases.ExecuteDeleteAsync();
        await _context.PromptCategories.ExecuteDeleteAsync();
        await _context.PromptGenres.ExecuteDeleteAsync();

        var result = await _termPackService.ImportPackAsync(exportedJson);

        Assert.Equal("Support Pack", result.PackName);
        Assert.Equal(1, result.ImportedCount);
        Assert.Contains(await _context.TermPhrases.ToListAsync(), term => term.Content == "Comment puis-je vous aider ?");
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
