using AIPrompt.Core.Interfaces;
using AIPrompt.Core.Models;
using AIPrompt.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIPrompt.Data;

public class PromptCategoryRepository : IPromptCategoryRepository
{
    private readonly AIPromptDbContext _context;

    public PromptCategoryRepository(AIPromptDbContext context)
    {
        _context = context;
    }

    public async Task<PromptCategoryModel> AddAsync(PromptCategoryModel model, CancellationToken cancellationToken = default)
    {
        var entity = new PromptCategory
        {
            Name = model.Name,
            Description = model.Description,
            IconKey = model.IconKey
        };

        _context.PromptCategories.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        model.Id = entity.Id;
        return model;
    }

    public async Task<IReadOnlyList<PromptCategoryModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.PromptCategories.OrderBy(category => category.Name).ToListAsync(cancellationToken);
        return entities.Select(ToModel).ToList();
    }

    public async Task UpdateAsync(PromptCategoryModel model, CancellationToken cancellationToken = default)
    {
        var entity = await _context.PromptCategories.FindAsync([model.Id], cancellationToken)
            ?? throw new InvalidOperationException($"PromptCategory with id {model.Id} was not found.");

        entity.Name = model.Name;
        entity.Description = model.Description;
        entity.IconKey = model.IconKey;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.PromptCategories.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return;
        }

        _context.PromptCategories.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static PromptCategoryModel ToModel(PromptCategory entity)
    {
        return new PromptCategoryModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            IconKey = entity.IconKey
        };
    }
}
