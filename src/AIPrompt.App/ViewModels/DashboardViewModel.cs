using System.Collections.ObjectModel;
using AIPrompt.Core.Interfaces;
using AIPrompt.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AIPrompt.App.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly IUsageStatsService _usageStatsService;

    [ObservableProperty]
    private int _totalCategories;

    [ObservableProperty]
    private int _totalGenres;

    [ObservableProperty]
    private int _totalTerms;

    [ObservableProperty]
    private int _totalSavedPrompts;

    [ObservableProperty]
    private int _totalRoadmapProjects;

    public DashboardViewModel(IUsageStatsService usageStatsService)
    {
        _usageStatsService = usageStatsService;
    }

    public ObservableCollection<CategoryUsageModel> TopCategories { get; } = [];

    public ObservableCollection<TermUsageModel> MostUsedTerms { get; } = [];

    public async Task InitializeAsync()
    {
        var stats = await _usageStatsService.GetStatsAsync();

        TotalCategories = stats.TotalCategories;
        TotalGenres = stats.TotalGenres;
        TotalTerms = stats.TotalTerms;
        TotalSavedPrompts = stats.TotalSavedPrompts;
        TotalRoadmapProjects = stats.TotalRoadmapProjects;

        TopCategories.Clear();
        foreach (var category in stats.TopCategories)
        {
            TopCategories.Add(category);
        }

        MostUsedTerms.Clear();
        foreach (var term in stats.MostUsedTerms)
        {
            MostUsedTerms.Add(term);
        }
    }
}
