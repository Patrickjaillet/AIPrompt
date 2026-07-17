using AIPrompt.Core.Models;
using AIPrompt.Data;
using AIPrompt.Data.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AIPrompt.Tests;

public class BackupServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AIPromptDbContext _context;
    private readonly BackupService _backupService;

    public BackupServiceTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AIPromptDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AIPromptDbContext(options);
        _context.Database.EnsureCreated();

        _backupService = new BackupService(_context);
    }

    private async Task SeedBaseDataAsync()
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
    }

    [Fact]
    public async Task ExportToJsonAsync_IncludesAllSeededData()
    {
        await SeedBaseDataAsync();

        var json = await _backupService.ExportToJsonAsync();
        var backup = System.Text.Json.JsonSerializer.Deserialize<DataBackupModel>(json);

        Assert.NotNull(backup);
        Assert.Contains(backup!.Terms, term => term.Content == "Écris une fonction");
        Assert.Contains(backup.Categories, category => category.Name == "Code");
        Assert.Contains(backup.Genres, genre => genre.Name == "Technique");
    }

    [Fact]
    public async Task ImportFromJsonAsync_Overwrite_ReplacesExistingData()
    {
        await SeedBaseDataAsync();
        var json = await _backupService.ExportToJsonAsync();

        _context.TermPhrases.Add(new TermPhrase
        {
            Content = "Terme temporaire",
            CategoryId = _context.PromptCategories.First().Id,
            GenreId = _context.PromptGenres.First().Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        await _backupService.ImportFromJsonAsync(json, ImportMode.Overwrite);

        var terms = await _context.TermPhrases.ToListAsync();
        Assert.Single(terms);
        Assert.Equal("Écris une fonction", terms[0].Content);
    }

    [Fact]
    public async Task ImportFromJsonAsync_Merge_SkipsExistingCategoriesByName()
    {
        await SeedBaseDataAsync();
        var json = await _backupService.ExportToJsonAsync();

        await _backupService.ImportFromJsonAsync(json, ImportMode.Merge);

        Assert.Equal(1, await _context.PromptCategories.CountAsync());
        Assert.Equal(1, await _context.PromptGenres.CountAsync());
        Assert.Equal(1, await _context.TermPhrases.CountAsync());
    }

    [Fact]
    public async Task ImportFromJsonAsync_Merge_AddsNewTermsWithoutDuplicatingExisting()
    {
        await SeedBaseDataAsync();
        var json = await _backupService.ExportToJsonAsync();

        _context.TermPhrases.Add(new TermPhrase
        {
            Content = "Terme different",
            CategoryId = _context.PromptCategories.First().Id,
            GenreId = _context.PromptGenres.First().Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        await _backupService.ImportFromJsonAsync(json, ImportMode.Merge);

        Assert.Equal(2, await _context.TermPhrases.CountAsync());
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
