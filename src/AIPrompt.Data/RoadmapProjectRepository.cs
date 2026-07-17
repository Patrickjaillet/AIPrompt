using AIPrompt.Core.Interfaces;
using AIPrompt.Core.Models;
using AIPrompt.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIPrompt.Data;

public class RoadmapProjectRepository : IRoadmapProjectRepository
{
    private readonly AIPromptDbContext _context;

    public RoadmapProjectRepository(AIPromptDbContext context)
    {
        _context = context;
    }

    public async Task<RoadmapProjectModel> AddAsync(RoadmapProjectModel model, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var entity = new RoadmapProject
        {
            ProjectName = model.ProjectName,
            Description = model.Description,
            CreatedAt = now,
            UpdatedAt = now,
            Phases = model.Phases.Select(phase => new RoadmapPhase
            {
                Title = phase.Title,
                OrderIndex = phase.OrderIndex,
                Tasks = phase.Tasks.Select(task => new RoadmapTask
                {
                    Description = task.Description,
                    IsChecked = task.IsChecked,
                    OrderIndex = task.OrderIndex
                }).ToList()
            }).ToList()
        };

        _context.RoadmapProjects.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        model.Id = entity.Id;
        return model;
    }

    public async Task<IReadOnlyList<RoadmapProjectModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.RoadmapProjects
            .OrderBy(project => project.ProjectName)
            .ToListAsync(cancellationToken);

        return entities.Select(entity => new RoadmapProjectModel
        {
            Id = entity.Id,
            ProjectName = entity.ProjectName,
            Description = entity.Description
        }).ToList();
    }

    public async Task<RoadmapProjectModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.RoadmapProjects
            .Include(project => project.Phases)
            .ThenInclude(phase => phase.Tasks)
            .FirstOrDefaultAsync(project => project.Id == id, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        return new RoadmapProjectModel
        {
            Id = entity.Id,
            ProjectName = entity.ProjectName,
            Description = entity.Description,
            Phases = entity.Phases
                .OrderBy(phase => phase.OrderIndex)
                .Select(phase => new RoadmapPhaseModel
                {
                    Id = phase.Id,
                    Title = phase.Title,
                    OrderIndex = phase.OrderIndex,
                    Tasks = phase.Tasks
                        .OrderBy(task => task.OrderIndex)
                        .Select(task => new RoadmapTaskModel
                        {
                            Id = task.Id,
                            Description = task.Description,
                            IsChecked = task.IsChecked,
                            OrderIndex = task.OrderIndex
                        }).ToList()
                }).ToList()
        };
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.RoadmapProjects.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return;
        }

        _context.RoadmapProjects.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
