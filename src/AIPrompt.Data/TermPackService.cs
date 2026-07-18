using System.Text.Json;
using AIPrompt.Core.Interfaces;
using AIPrompt.Core.Models;
using AIPrompt.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIPrompt.Data;

public class TermPackService : ITermPackService
{
    private readonly AIPromptDbContext _context;

    public TermPackService(AIPromptDbContext context)
    {
        _context = context;
    }

    public async Task<TermPackImportResult> ImportPackAsync(string json, CancellationToken cancellationToken = default)
    {
        var pack = JsonSerializer.Deserialize<TermPackModel>(json)
            ?? throw new InvalidOperationException("Le fichier de pack est invalide.");

        var allCategories = await _context.PromptCategories.ToListAsync(cancellationToken);
        var category = allCategories.FirstOrDefault(existing => existing.Name.Equals(pack.CategoryName, StringComparison.OrdinalIgnoreCase));

        if (category is null)
        {
            category = new PromptCategory { Name = pack.CategoryName, Description = pack.CategoryDescription, IconKey = pack.CategoryIconKey };
            _context.PromptCategories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);
        }

        var existingGenres = await _context.PromptGenres.ToListAsync(cancellationToken);
        var existingTerms = await _context.TermPhrases
            .Where(term => term.CategoryId == category.Id)
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;
        var importedCount = 0;
        var skippedCount = 0;

        foreach (var item in pack.Items)
        {
            var genre = existingGenres.FirstOrDefault(existing => existing.Name.Equals(item.GenreName, StringComparison.OrdinalIgnoreCase));
            if (genre is null)
            {
                genre = new PromptGenre { Name = item.GenreName, Description = item.GenreDescription };
                _context.PromptGenres.Add(genre);
                await _context.SaveChangesAsync(cancellationToken);
                existingGenres.Add(genre);
            }

            if (existingTerms.Any(existing => existing.Content.Equals(item.Content, StringComparison.OrdinalIgnoreCase)))
            {
                skippedCount++;
                continue;
            }

            var term = new TermPhrase
            {
                Content = item.Content,
                CategoryId = category.Id,
                GenreId = genre.Id,
                Tags = item.Tags,
                Language = string.IsNullOrWhiteSpace(item.Language) ? "FR" : item.Language,
                CreatedAt = now,
                UpdatedAt = now
            };
            _context.TermPhrases.Add(term);
            existingTerms.Add(term);
            importedCount++;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new TermPackImportResult
        {
            PackName = pack.PackName,
            CategoryName = category.Name,
            ImportedCount = importedCount,
            SkippedCount = skippedCount
        };
    }

    public async Task<string> ExportPackAsync(int categoryId, string packName, CancellationToken cancellationToken = default)
    {
        var category = await _context.PromptCategories
            .FirstOrDefaultAsync(existing => existing.Id == categoryId, cancellationToken)
            ?? throw new InvalidOperationException("Catégorie introuvable.");

        var terms = await _context.TermPhrases
            .Include(term => term.Genre)
            .Where(term => term.CategoryId == categoryId)
            .ToListAsync(cancellationToken);

        var pack = new TermPackModel
        {
            PackName = packName,
            CategoryName = category.Name,
            CategoryDescription = category.Description,
            CategoryIconKey = category.IconKey,
            Items = terms.Select(term => new TermPackItemModel
            {
                Content = term.Content,
                GenreName = term.Genre.Name,
                GenreDescription = term.Genre.Description,
                Tags = term.Tags,
                Language = term.Language
            }).ToList()
        };

        return JsonSerializer.Serialize(pack, new JsonSerializerOptions { WriteIndented = true });
    }
}
