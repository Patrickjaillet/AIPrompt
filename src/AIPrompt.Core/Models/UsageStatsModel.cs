namespace AIPrompt.Core.Models;

public class UsageStatsModel
{
    public int TotalCategories { get; set; }

    public int TotalGenres { get; set; }

    public int TotalTerms { get; set; }

    public int TotalSavedPrompts { get; set; }

    public int TotalRoadmapProjects { get; set; }

    public List<CategoryUsageModel> TopCategories { get; set; } = new();

    public List<TermUsageModel> MostUsedTerms { get; set; } = new();
}

public class CategoryUsageModel
{
    public string CategoryName { get; set; } = string.Empty;

    public int UsageCount { get; set; }
}

public class TermUsageModel
{
    public string Content { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public int UsageCount { get; set; }

    public DateTime UpdatedAt { get; set; }
}
