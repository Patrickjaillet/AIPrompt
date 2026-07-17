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
        return model;
    }

    public async Task<IReadOnlyList<SavedPromptModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.SavedPrompts
            .OrderByDescending(prompt => prompt.UpdatedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(entity => new SavedPromptModel
        {
            Id = entity.Id,
            Title = entity.Title,
            FinalContent = entity.FinalContent,
            SourceTemplateId = entity.SourceTemplateId,
            ExportFormat = (PromptExportFormat)entity.ExportFormat,
            FilePath = entity.FilePath
        }).ToList();
    }
}
