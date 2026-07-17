using AIPrompt.Core.Models;
using AIPrompt.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AIPrompt.Tests;

public class PromptCategoryRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AIPromptDbContext _context;
    private readonly PromptCategoryRepository _repository;

    public PromptCategoryRepositoryTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AIPromptDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AIPromptDbContext(options);
        _context.Database.EnsureCreated();

        _repository = new PromptCategoryRepository(_context);
    }

    [Fact]
    public async Task AddAsync_PersistsCategory()
    {
        var added = await _repository.AddAsync(new PromptCategoryModel { Name = "Code", Description = "Code prompts", IconKey = "CodeTags" });

        Assert.NotEqual(0, added.Id);
        Assert.Equal(1, await _context.PromptCategories.CountAsync());
    }

    [Fact]
    public async Task GetAllAsync_ReturnsCategoriesOrderedByName()
    {
        await _repository.AddAsync(new PromptCategoryModel { Name = "Roadmap" });
        await _repository.AddAsync(new PromptCategoryModel { Name = "Code" });

        var results = await _repository.GetAllAsync();

        Assert.Equal(["Code", "Roadmap"], results.Select(category => category.Name));
    }

    [Fact]
    public async Task UpdateAsync_ModifiesExistingCategory()
    {
        var added = await _repository.AddAsync(new PromptCategoryModel { Name = "Original" });
        added.Name = "Updated";

        await _repository.UpdateAsync(added);

        var all = await _repository.GetAllAsync();
        Assert.Equal("Updated", all.Single().Name);
    }

    [Fact]
    public async Task DeleteAsync_RemovesCategory()
    {
        var added = await _repository.AddAsync(new PromptCategoryModel { Name = "ToDelete" });

        await _repository.DeleteAsync(added.Id);

        Assert.Empty(await _repository.GetAllAsync());
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
