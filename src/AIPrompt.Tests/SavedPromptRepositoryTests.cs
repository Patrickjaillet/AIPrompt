using AIPrompt.Core.Models;
using AIPrompt.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AIPrompt.Tests;

public class SavedPromptRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AIPromptDbContext _context;
    private readonly SavedPromptRepository _repository;

    public SavedPromptRepositoryTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AIPromptDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AIPromptDbContext(options);
        _context.Database.EnsureCreated();

        _repository = new SavedPromptRepository(_context);
    }

    [Fact]
    public async Task AddAsync_PersistsSavedPrompt()
    {
        var created = await _repository.AddAsync(new SavedPromptModel
        {
            Title = "Mon prompt",
            FinalContent = "Contenu assemblé",
            ExportFormat = PromptExportFormat.Markdown
        });

        Assert.NotEqual(0, created.Id);
        Assert.Equal(1, await _context.SavedPrompts.CountAsync());
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPromptsOrderedByMostRecentlyUpdated()
    {
        var first = await _repository.AddAsync(new SavedPromptModel { Title = "First", FinalContent = "A" });
        await Task.Delay(10);
        var second = await _repository.AddAsync(new SavedPromptModel { Title = "Second", FinalContent = "B" });

        var results = await _repository.GetAllAsync();

        Assert.Equal(["Second", "First"], results.Select(prompt => prompt.Title));
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
