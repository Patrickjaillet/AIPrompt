using AIPrompt.Core.Interfaces;
using AIPrompt.Core.Models;
using AIPrompt.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIPrompt.Data;

public class PromptGenreRepository : IPromptGenreRepository
{
    private readonly AIPromptDbContext _context;

    public PromptGenreRepository(AIPromptDbContext context)
    {
        _context = context;
    }

    public async Task<PromptGenreModel> AddAsync(PromptGenreModel model, CancellationToken cancellationToken = default)
    {
        var entity = new PromptGenre
        {
            Name = model.Name,
            Description = model.Description
        };

        _context.PromptGenres.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        model.Id = entity.Id;
        return model;
    }

    public async Task<IReadOnlyList<PromptGenreModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.PromptGenres.OrderBy(genre => genre.Name).ToListAsync(cancellationToken);
        return entities.Select(ToModel).ToList();
    }

    public async Task UpdateAsync(PromptGenreModel model, CancellationToken cancellationToken = default)
    {
        var entity = await _context.PromptGenres.FindAsync([model.Id], cancellationToken)
            ?? throw new InvalidOperationException($"PromptGenre with id {model.Id} was not found.");

        entity.Name = model.Name;
        entity.Description = model.Description;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.PromptGenres.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return;
        }

        _context.PromptGenres.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static PromptGenreModel ToModel(PromptGenre entity)
    {
        return new PromptGenreModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description
        };
    }
}
