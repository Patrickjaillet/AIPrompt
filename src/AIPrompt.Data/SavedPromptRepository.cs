using AIPrompt.Core.Interfaces;
using AIPrompt.Core.Models;
using AIPrompt.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIPrompt.Data;

public class SavedPromptRepository : ISavedPromptRepository
{
    private readonly AIPromptDbContext _context;

    public SavedPromptRepository(AIPromptDbContext context)
    {
        _context = context;
    }

    public async Task<SavedPromptModel> AddAsync(SavedPromptModel model, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var entity = new SavedPrompt
        {
            Title = model.Title,
            FinalContent = model.FinalContent,
            SourceTemplateId = model.SourceTemplateId,
            ExportFormat = (ExportFormat)model.ExportFormat,
            FilePath = model.FilePath,
            CreatedAt = now,
            UpdatedAt = now
        };

        _context.SavedPrompts.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        model.Id = entity.Id;
        model.CreatedAt = now;
        model.UpdatedAt = now;
        return model;
    }

    public async Task<IReadOnlyList<SavedPromptModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.SavedPrompts
            .Include(prompt => prompt.SourceTemplate)
            .ThenInclude(template => template!.Category)
            .OrderByDescending(prompt => prompt.UpdatedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(ToModel).ToList();
    }

    public async Task<SavedPromptModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.SavedPrompts
            .Include(prompt => prompt.SourceTemplate)
            .ThenInclude(template => template!.Category)
            .FirstOrDefaultAsync(prompt => prompt.Id == id, cancellationToken);

        return entity is null ? null : ToModel(entity);
    }

    public async Task UpdateAsync(SavedPromptModel model, CancellationToken cancellationToken = default)
    {
        var entity = await _context.SavedPrompts.FindAsync([model.Id], cancellationToken)
            ?? throw new InvalidOperationException($"SavedPrompt with id {model.Id} was not found.");

        entity.Title = model.Title;
        entity.FinalContent = model.FinalContent;
        entity.ExportFormat = (ExportFormat)model.ExportFormat;
        entity.FilePath = model.FilePath;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.SavedPrompts.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return;
        }

        _context.SavedPrompts.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static SavedPromptModel ToModel(SavedPrompt entity)
    {
        return new SavedPromptModel
        {
            Id = entity.Id,
            Title = entity.Title,
            FinalContent = entity.FinalContent,
            SourceTemplateId = entity.SourceTemplateId,
            CategoryName = entity.SourceTemplate?.Category?.Name ?? string.Empty,
            ExportFormat = (PromptExportFormat)entity.ExportFormat,
            FilePath = entity.FilePath,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
