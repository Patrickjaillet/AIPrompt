using AIPrompt.Core.Models;
using AIPrompt.Data;
using AIPrompt.Data.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AIPrompt.Tests;

public class PromptTemplateRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AIPromptDbContext _context;
    private readonly PromptTemplateRepository _repository;
    private readonly int _categoryId;
    private readonly int _termPhraseId;

    public PromptTemplateRepositoryTests()
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

        var termPhrase = new TermPhrase { Content = "Explique", CategoryId = category.Id, GenreId = genre.Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        _context.TermPhrases.Add(termPhrase);
        _context.SaveChanges();

        _categoryId = category.Id;
        _termPhraseId = termPhrase.Id;

        _repository = new PromptTemplateRepository(_context);
    }

    [Fact]
    public async Task AddAsync_PersistsTemplateWithBlocks()
    {
        var model = new PromptTemplateModel
        {
            Title = "Template 1",
            CategoryId = _categoryId,
            Blocks =
            [
                new PromptBlockModel { OrderIndex = 0, TermPhraseId = _termPhraseId },
                new PromptBlockModel { OrderIndex = 1, FreeText = "Texte libre" }
            ]
        };

        var created = await _repository.AddAsync(model);

        Assert.NotEqual(0, created.Id);
        Assert.Equal(2, await _context.PromptBlocks.CountAsync());
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsBlocksWithResolvedContent()
    {
        var model = new PromptTemplateModel
        {
            Title = "Template 1",
            CategoryId = _categoryId,
            Blocks =
            [
                new PromptBlockModel { OrderIndex = 0, TermPhraseId = _termPhraseId },
                new PromptBlockModel { OrderIndex = 1, FreeText = "Texte libre" }
            ]
        };
        var created = await _repository.AddAsync(model);

        var loaded = await _repository.GetByIdAsync(created.Id);

        Assert.NotNull(loaded);
        Assert.Equal(2, loaded!.Blocks.Count);
        Assert.Equal("Explique", loaded.Blocks[0].Content);
        Assert.Equal("Texte libre", loaded.Blocks[1].Content);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsTemplateSummaries()
    {
        await _repository.AddAsync(new PromptTemplateModel { Title = "Zeta", CategoryId = _categoryId });
        await _repository.AddAsync(new PromptTemplateModel { Title = "Alpha", CategoryId = _categoryId });

        var results = await _repository.GetAllAsync();

        Assert.Equal(["Alpha", "Zeta"], results.Select(template => template.Title));
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        var result = await _repository.GetByIdAsync(999);

        Assert.Null(result);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
