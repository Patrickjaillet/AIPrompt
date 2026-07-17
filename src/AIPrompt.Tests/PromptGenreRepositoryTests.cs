using AIPrompt.Core.Models;
using AIPrompt.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AIPrompt.Tests;

public class PromptGenreRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AIPromptDbContext _context;
    private readonly PromptGenreRepository _repository;

    public PromptGenreRepositoryTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AIPromptDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AIPromptDbContext(options);
        _context.Database.EnsureCreated();

        _repository = new PromptGenreRepository(_context);
    }

    [Fact]
    public async Task AddAsync_PersistsGenre()
    {
        var added = await _repository.AddAsync(new PromptGenreModel { Name = "Technique", Description = "Technical genre" });

        Assert.NotEqual(0, added.Id);
        Assert.Equal(1, await _context.PromptGenres.CountAsync());
    }

    [Fact]
    public async Task UpdateAsync_ModifiesExistingGenre()
    {
        var added = await _repository.AddAsync(new PromptGenreModel { Name = "Original" });
        added.Name = "Updated";

        await _repository.UpdateAsync(added);

        var all = await _repository.GetAllAsync();
        Assert.Equal("Updated", all.Single().Name);
    }

    [Fact]
    public async Task DeleteAsync_RemovesGenre()
    {
        var added = await _repository.AddAsync(new PromptGenreModel { Name = "ToDelete" });

        await _repository.DeleteAsync(added.Id);

        Assert.Empty(await _repository.GetAllAsync());
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
