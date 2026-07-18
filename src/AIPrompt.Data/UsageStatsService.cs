using AIPrompt.Core.Interfaces;
using AIPrompt.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace AIPrompt.Data;

public class UsageStatsService : IUsageStatsService
{
    private readonly AIPromptDbContext _context;

    public UsageStatsService(AIPromptDbContext context)
    {
        _context = context;
    }

    public async Task<UsageStatsModel> GetStatsAsync(int topCount = 5, CancellationToken cancellationToken = default)
    {
        var terms = await _context.TermPhrases
            .Include(term => term.Category)
            .ToListAsync(cancellationToken);

        var topCategories = terms
            .GroupBy(term => term.Category.Name)
            .Select(group => new CategoryUsageModel { CategoryName = group.Key, UsageCount = group.Sum(term => term.UsageCount) })
            .Where(category => category.UsageCount > 0)
            .OrderByDescending(category => category.UsageCount)
            .Take(topCount)
            .ToList();

        var mostUsedTerms = terms
            .Where(term => term.UsageCount > 0)
            .OrderByDescending(term => term.UsageCount)
            .ThenByDescending(term => term.UpdatedAt)
            .Take(topCount)
            .Select(term => new TermUsageModel
            {
                Content = term.Content,
                CategoryName = term.Category.Name,
                UsageCount = term.UsageCount,
                UpdatedAt = term.UpdatedAt
            })
            .ToList();

        return new UsageStatsModel
        {
            TotalCategories = await _context.PromptCategories.CountAsync(cancellationToken),
            TotalGenres = await _context.PromptGenres.CountAsync(cancellationToken),
            TotalTerms = terms.Count,
            TotalSavedPrompts = await _context.SavedPrompts.CountAsync(cancellationToken),
            TotalRoadmapProjects = await _context.RoadmapProjects.CountAsync(cancellationToken),
            TopCategories = topCategories,
            MostUsedTerms = mostUsedTerms
        };
    }
}
