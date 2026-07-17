using AIPrompt.Core.Models;
using AIPrompt.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AIPrompt.Tests;

public class RoadmapProjectRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AIPromptDbContext _context;
    private readonly RoadmapProjectRepository _repository;

    public RoadmapProjectRepositoryTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AIPromptDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AIPromptDbContext(options);
        _context.Database.EnsureCreated();

        _repository = new RoadmapProjectRepository(_context);
    }

    private static RoadmapProjectModel BuildSampleProject()
    {
        return new RoadmapProjectModel
        {
            ProjectName = "AIPrompt",
            Description = "Générateur de prompts",
            Phases =
            [
                new RoadmapPhaseModel
                {
                    Title = "Phase 0",
                    OrderIndex = 0,
                    Tasks =
                    [
                        new RoadmapTaskModel { Description = "Initialiser le dépôt", OrderIndex = 0 },
                        new RoadmapTaskModel { Description = "Ajouter la licence", OrderIndex = 1, IsChecked = true }
                    ]
                }
            ]
        };
    }

    [Fact]
    public async Task AddAsync_PersistsProjectWithPhasesAndTasks()
    {
        var created = await _repository.AddAsync(BuildSampleProject());

        Assert.NotEqual(0, created.Id);
        Assert.Equal(1, await _context.RoadmapProjects.CountAsync());
        Assert.Equal(1, await _context.RoadmapPhases.CountAsync());
        Assert.Equal(2, await _context.RoadmapTasks.CountAsync());
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsPhasesAndTasksInOrder()
    {
        var created = await _repository.AddAsync(BuildSampleProject());

        var loaded = await _repository.GetByIdAsync(created.Id);

        Assert.NotNull(loaded);
        Assert.Equal("AIPrompt", loaded!.ProjectName);
        Assert.Single(loaded.Phases);
        Assert.Equal(2, loaded.Phases[0].Tasks.Count);
        Assert.True(loaded.Phases[0].Tasks[1].IsChecked);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        var result = await _repository.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsProjectSummaries()
    {
        await _repository.AddAsync(new RoadmapProjectModel { ProjectName = "Zeta" });
        await _repository.AddAsync(new RoadmapProjectModel { ProjectName = "Alpha" });

        var results = await _repository.GetAllAsync();

        Assert.Equal(["Alpha", "Zeta"], results.Select(project => project.ProjectName));
    }

    [Fact]
    public async Task DeleteAsync_RemovesProject()
    {
        var created = await _repository.AddAsync(BuildSampleProject());

        await _repository.DeleteAsync(created.Id);

        Assert.Null(await _repository.GetByIdAsync(created.Id));
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
