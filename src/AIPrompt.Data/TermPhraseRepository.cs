using AIPrompt.Core.Interfaces;
using AIPrompt.Core.Models;
using AIPrompt.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIPrompt.Data;

public class TermPhraseRepository : ITermPhraseRepository
{
    private readonly AIPromptDbContext _context;

    public TermPhraseRepository(AIPromptDbContext context)
    {
        _context = context;
    }

    public async Task<TermPhraseModel> AddAsync(TermPhraseModel model, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var entity = new TermPhrase
        {
            Content = model.Content,
            CategoryId = model.CategoryId,
            GenreId = model.GenreId,
            Tags = model.Tags,
            Language = model.Language,
            UsageCount = model.UsageCount,
            CreatedAt = now,
            UpdatedAt = now
        };

        _context.TermPhrases.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        model.Id = entity.Id;
        return model;
    }

    public async Task<TermPhraseModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.TermPhrases.FindAsync([id], cancellationToken);
        return entity is null ? null : ToModel(entity);
    }

    public async Task<IReadOnlyList<TermPhraseModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.TermPhrases.ToListAsync(cancellationToken);
        return entities.Select(ToModel).ToList();
    }

    public async Task UpdateAsync(TermPhraseModel model, CancellationToken cancellationToken = default)
    {
        var entity = await _context.TermPhrases.FindAsync([model.Id], cancellationToken)
            ?? throw new InvalidOperationException($"TermPhrase with id {model.Id} was not found.");

        entity.Content = model.Content;
        entity.CategoryId = model.CategoryId;
        entity.GenreId = model.GenreId;
        entity.Tags = model.Tags;
        entity.Language = model.Language;
        entity.UsageCount = model.UsageCount;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.TermPhrases.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return;
        }

        _context.TermPhrases.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task IncrementUsageAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.TermPhrases.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return;
        }

        entity.UsageCount++;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TermPhraseModel>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return await GetAllAsync(cancellationToken);
        }

        var tokens = query.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var matchQuery = string.Join(' ', tokens.Select(token => $"\"{token.Replace("\"", "\"\"")}\"*"));

        var matchedIds = new List<int>();
        var connection = _context.Database.GetDbConnection();
        var wasClosed = connection.State != System.Data.ConnectionState.Open;
        if (wasClosed)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT rowid FROM TermPhraseFts WHERE TermPhraseFts MATCH $query ORDER BY rank;";
            var parameter = command.CreateParameter();
            parameter.ParameterName = "$query";
            parameter.Value = matchQuery;
            command.Parameters.Add(parameter);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                matchedIds.Add(reader.GetInt32(0));
            }
        }
        finally
        {
            if (wasClosed)
            {
                await connection.CloseAsync();
            }
        }

        var entities = await _context.TermPhrases
            .Where(term => matchedIds.Contains(term.Id))
            .ToListAsync(cancellationToken);

        return matchedIds
            .Select(id => entities.FirstOrDefault(entity => entity.Id == id))
            .Where(entity => entity is not null)
            .Select(entity => ToModel(entity!))
            .ToList();
    }

    private static TermPhraseModel ToModel(TermPhrase entity)
    {
        return new TermPhraseModel
        {
            Id = entity.Id,
            Content = entity.Content,
            CategoryId = entity.CategoryId,
            GenreId = entity.GenreId,
            Tags = entity.Tags,
            Language = entity.Language,
            UsageCount = entity.UsageCount
        };
    }
}
