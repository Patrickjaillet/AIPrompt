using AIPrompt.Data;
using AIPrompt.Data.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AIPrompt.Tests;

public class TermPhraseFtsSearchTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AIPromptDbContext _context;
    private readonly TermPhraseRepository _repository;

    public TermPhraseFtsSearchTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AIPromptDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AIPromptDbContext(options);
        _context.Database.Migrate();

        _repository = new TermPhraseRepository(_context);
    }

    [Fact]
    public async Task SearchAsync_FindsTermsByContentPrefix()
    {
        var category = new PromptCategory { Name = "Code", Description = "Code prompts", IconKey = "CodeTags" };
        var genre = new PromptGenre { Name = "Technique", Description = "Technical genre" };
        _context.PromptCategories.Add(category);
        _context.PromptGenres.Add(genre);
        await _context.SaveChangesAsync();

        var now = DateTime.UtcNow;
        _context.TermPhrases.AddRange(
            new TermPhrase { Content = "Écris une fonction récursive", CategoryId = category.Id, GenreId = genre.Id, Tags = "code", CreatedAt = now, UpdatedAt = now },
            new TermPhrase { Content = "Corrige ce bug critique", CategoryId = category.Id, GenreId = genre.Id, Tags = "debug", CreatedAt = now, UpdatedAt = now });
        await _context.SaveChangesAsync();

        var results = await _repository.SearchAsync("fonction");

        Assert.Single(results);
        Assert.Equal("Écris une fonction récursive", results[0].Content);
    }

    [Fact]
    public async Task SearchAsync_FindsTermsByTag()
    {
        var category = new PromptCategory { Name = "Code", Description = "Code prompts", IconKey = "CodeTags" };
        var genre = new PromptGenre { Name = "Technique", Description = "Technical genre" };
        _context.PromptCategories.Add(category);
        _context.PromptGenres.Add(genre);
        await _context.SaveChangesAsync();

        var now = DateTime.UtcNow;
        _context.TermPhrases.Add(new TermPhrase { Content = "Rédige un slogan", CategoryId = category.Id, GenreId = genre.Id, Tags = "marketing,slogan", CreatedAt = now, UpdatedAt = now });
        await _context.SaveChangesAsync();

        var results = await _repository.SearchAsync("marketing");

        Assert.Single(results);
    }

    [Fact]
    public async Task SearchAsync_TracksUpdatesAndDeletes()
    {
        var category = new PromptCategory { Name = "Code", Description = "Code prompts", IconKey = "CodeTags" };
        var genre = new PromptGenre { Name = "Technique", Description = "Technical genre" };
        _context.PromptCategories.Add(category);
        _context.PromptGenres.Add(genre);
        await _context.SaveChangesAsync();

        var now = DateTime.UtcNow;
        var entity = new TermPhrase { Content = "Contenu original", CategoryId = category.Id, GenreId = genre.Id, CreatedAt = now, UpdatedAt = now };
        _context.TermPhrases.Add(entity);
        await _context.SaveChangesAsync();

        entity.Content = "Contenu modifié";
        await _context.SaveChangesAsync();

        var afterUpdate = await _repository.SearchAsync("modifié");
        Assert.Single(afterUpdate);

        var stale = await _repository.SearchAsync("original");
        Assert.Empty(stale);

        _context.TermPhrases.Remove(entity);
        await _context.SaveChangesAsync();

        var afterDelete = await _repository.SearchAsync("modifié");
        Assert.Empty(afterDelete);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
