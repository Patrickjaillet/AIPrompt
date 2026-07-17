using AIPrompt.Core.Models;
using AIPrompt.Data;
using AIPrompt.Data.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AIPrompt.Tests;

public class TermPhraseRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AIPromptDbContext _context;
    private readonly TermPhraseRepository _repository;

    public TermPhraseRepositoryTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AIPromptDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AIPromptDbContext(options);
        _context.Database.EnsureCreated();

        var category = new PromptCategory { Name = "Code", Description = "Code prompts", IconKey = "CodeTags" };
        var genre = new PromptGenre { Name = "Technique", Description = "Technical genre" };
        _context.PromptCategories.Add(category);
        _context.PromptGenres.Add(genre);
        _context.SaveChanges();

        CategoryId = category.Id;
        GenreId = genre.Id;

        _repository = new TermPhraseRepository(_context);
    }

    private int CategoryId { get; }

    private int GenreId { get; }

    [Fact]
    public async Task AddAsync_PersistsTermPhrase()
    {
        var model = new TermPhraseModel { Content = "Écris une fonction", CategoryId = CategoryId, GenreId = GenreId, Tags = "code", Language = "FR" };

        var added = await _repository.AddAsync(model);

        Assert.NotEqual(0, added.Id);
        Assert.Equal(1, await _context.TermPhrases.CountAsync());
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsMatchingTermPhrase()
    {
        var added = await _repository.AddAsync(new TermPhraseModel { Content = "Content", CategoryId = CategoryId, GenreId = GenreId });

        var result = await _repository.GetByIdAsync(added.Id);

        Assert.NotNull(result);
        Assert.Equal("Content", result!.Content);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        var result = await _repository.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllTermPhrases()
    {
        await _repository.AddAsync(new TermPhraseModel { Content = "First", CategoryId = CategoryId, GenreId = GenreId });
        await _repository.AddAsync(new TermPhraseModel { Content = "Second", CategoryId = CategoryId, GenreId = GenreId });

        var results = await _repository.GetAllAsync();

        Assert.Equal(2, results.Count);
    }

    [Fact]
    public async Task UpdateAsync_ModifiesExistingTermPhrase()
    {
        var added = await _repository.AddAsync(new TermPhraseModel { Content = "Original", CategoryId = CategoryId, GenreId = GenreId });
        added.Content = "Updated";

        await _repository.UpdateAsync(added);

        var updated = await _repository.GetByIdAsync(added.Id);
        Assert.Equal("Updated", updated!.Content);
    }

    [Fact]
    public async Task UpdateAsync_Throws_WhenTermPhraseDoesNotExist()
    {
        var missing = new TermPhraseModel { Id = 999, Content = "Missing", CategoryId = CategoryId, GenreId = GenreId };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _repository.UpdateAsync(missing));
    }

    [Fact]
    public async Task DeleteAsync_RemovesTermPhrase()
    {
        var added = await _repository.AddAsync(new TermPhraseModel { Content = "ToDelete", CategoryId = CategoryId, GenreId = GenreId });

        await _repository.DeleteAsync(added.Id);

        Assert.Null(await _repository.GetByIdAsync(added.Id));
    }

    [Fact]
    public async Task DeleteAsync_DoesNothing_WhenTermPhraseDoesNotExist()
    {
        await _repository.DeleteAsync(999);

        Assert.Equal(0, await _context.TermPhrases.CountAsync());
    }

    [Fact]
    public async Task IncrementUsageAsync_IncrementsUsageCount()
    {
        var added = await _repository.AddAsync(new TermPhraseModel { Content = "Content", CategoryId = CategoryId, GenreId = GenreId });

        await _repository.IncrementUsageAsync(added.Id);
        await _repository.IncrementUsageAsync(added.Id);

        var updated = await _repository.GetByIdAsync(added.Id);
        Assert.Equal(2, updated!.UsageCount);
    }

    [Fact]
    public async Task IncrementUsageAsync_DoesNothing_WhenTermPhraseDoesNotExist()
    {
        await _repository.IncrementUsageAsync(999);

        Assert.Equal(0, await _context.TermPhrases.CountAsync());
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
