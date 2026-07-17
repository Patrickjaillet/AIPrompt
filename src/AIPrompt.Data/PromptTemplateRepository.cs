using AIPrompt.Core.Interfaces;
using AIPrompt.Core.Models;
using AIPrompt.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIPrompt.Data;

public class PromptTemplateRepository : IPromptTemplateRepository
{
    private readonly AIPromptDbContext _context;

    public PromptTemplateRepository(AIPromptDbContext context)
    {
        _context = context;
    }

    public async Task<PromptTemplateModel> AddAsync(PromptTemplateModel model, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var entity = new PromptTemplate
        {
            Title = model.Title,
            CategoryId = model.CategoryId,
            CreatedAt = now,
            UpdatedAt = now,
            Blocks = model.Blocks.Select(block => new PromptBlock
            {
                OrderIndex = block.OrderIndex,
                TermPhraseId = block.TermPhraseId,
                FreeText = block.FreeText
            }).ToList()
        };

        _context.PromptTemplates.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        model.Id = entity.Id;
        return model;
    }

    public async Task<IReadOnlyList<PromptTemplateModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.PromptTemplates
            .OrderBy(template => template.Title)
            .ToListAsync(cancellationToken);

        return entities.Select(entity => new PromptTemplateModel
        {
            Id = entity.Id,
            Title = entity.Title,
            CategoryId = entity.CategoryId
        }).ToList();
    }

    public async Task<PromptTemplateModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.PromptTemplates
            .Include(template => template.Blocks)
            .ThenInclude(block => block.TermPhrase)
            .FirstOrDefaultAsync(template => template.Id == id, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        return new PromptTemplateModel
        {
            Id = entity.Id,
            Title = entity.Title,
            CategoryId = entity.CategoryId,
            Blocks = entity.Blocks
                .OrderBy(block => block.OrderIndex)
                .Select(block => new PromptBlockModel
                {
                    Id = block.Id,
                    OrderIndex = block.OrderIndex,
                    TermPhraseId = block.TermPhraseId,
                    FreeText = block.FreeText,
                    Content = block.FreeText ?? block.TermPhrase?.Content ?? string.Empty
                }).ToList()
        };
    }
}
