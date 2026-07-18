using System.Collections.ObjectModel;
using AIPrompt.App.Services;
using AIPrompt.Core.Interfaces;
using AIPrompt.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AIPrompt.App.ViewModels;

public partial class TermLibraryViewModel : ViewModelBase
{
    private const string AllFilterValue = "Toutes";

    private readonly ITermPhraseRepository _termPhraseRepository;
    private readonly IPromptCategoryRepository _categoryRepository;
    private readonly IPromptGenreRepository _genreRepository;
    private readonly IDialogService _dialogService;

    private List<TermPhraseModel> _allTerms = [];
    private List<PromptCategoryModel> _categories = [];
    private List<PromptGenreModel> _genres = [];

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _selectedCategoryFilter = AllFilterValue;

    [ObservableProperty]
    private string _selectedGenreFilter = AllFilterValue;

    [ObservableProperty]
    private string _selectedLanguageFilter = AllFilterValue;

    public TermLibraryViewModel(
        ITermPhraseRepository termPhraseRepository,
        IPromptCategoryRepository categoryRepository,
        IPromptGenreRepository genreRepository,
        IDialogService dialogService)
    {
        _termPhraseRepository = termPhraseRepository;
        _categoryRepository = categoryRepository;
        _genreRepository = genreRepository;
        _dialogService = dialogService;
    }

    public ObservableCollection<TermRowViewModel> Terms { get; } = [];

    public ObservableCollection<string> CategoryFilters { get; } = [AllFilterValue];

    public ObservableCollection<string> GenreFilters { get; } = [AllFilterValue];

    public ObservableCollection<string> LanguageFilters { get; } = [AllFilterValue];

    public async Task InitializeAsync()
    {
        await ReloadReferenceDataAsync();
        await ReloadTermsAsync();
    }

    partial void OnSearchTextChanged(string value) => _ = ApplyFiltersAsync();

    partial void OnSelectedCategoryFilterChanged(string value) => _ = ApplyFiltersAsync();

    partial void OnSelectedGenreFilterChanged(string value) => _ = ApplyFiltersAsync();

    partial void OnSelectedLanguageFilterChanged(string value) => _ = ApplyFiltersAsync();

    [RelayCommand]
    private async Task AddTermAsync()
    {
        if (_categories.Count == 0 || _genres.Count == 0)
        {
            return;
        }

        var editorViewModel = new TermEditorViewModel(_categories, _genres, GetAvailableTags(), existingTerm: null);

        if (_dialogService.ShowTermEditor(editorViewModel) == true)
        {
            await _termPhraseRepository.AddAsync(editorViewModel.ToModel());
            await ReloadTermsAsync();
        }
    }

    [RelayCommand]
    private async Task EditTermAsync(TermRowViewModel row)
    {
        var editorViewModel = new TermEditorViewModel(_categories, _genres, GetAvailableTags(), row.Term);

        if (_dialogService.ShowTermEditor(editorViewModel) == true)
        {
            await _termPhraseRepository.UpdateAsync(editorViewModel.ToModel());
            await ReloadTermsAsync();
        }
    }

    [RelayCommand]
    private async Task DeleteTermAsync(TermRowViewModel row)
    {
        await _termPhraseRepository.DeleteAsync(row.Id);
        await ReloadTermsAsync();
    }

    [RelayCommand]
    private async Task MarkUsedAsync(TermRowViewModel row)
    {
        await _termPhraseRepository.IncrementUsageAsync(row.Id);
        await ReloadTermsAsync();
    }

    [RelayCommand]
    private async Task ManageCategoriesAsync()
    {
        var managerViewModel = new CategoryManagerViewModel(_categoryRepository, _categories);
        _dialogService.ShowCategoryManager(managerViewModel);
        await ReloadReferenceDataAsync();
        await ReloadTermsAsync();
    }

    [RelayCommand]
    private async Task ManageGenresAsync()
    {
        var managerViewModel = new GenreManagerViewModel(_genreRepository, _genres);
        _dialogService.ShowGenreManager(managerViewModel);
        await ReloadReferenceDataAsync();
        await ReloadTermsAsync();
    }

    private async Task ReloadReferenceDataAsync()
    {
        _categories = (await _categoryRepository.GetAllAsync()).ToList();
        _genres = (await _genreRepository.GetAllAsync()).ToList();

        CategoryFilters.Clear();
        CategoryFilters.Add(AllFilterValue);
        foreach (var category in _categories)
        {
            CategoryFilters.Add(category.Name);
        }

        GenreFilters.Clear();
        GenreFilters.Add(AllFilterValue);
        foreach (var genre in _genres)
        {
            GenreFilters.Add(genre.Name);
        }

        LanguageFilters.Clear();
        LanguageFilters.Add(AllFilterValue);
        LanguageFilters.Add("FR");
        LanguageFilters.Add("EN");

        SelectedCategoryFilter = CategoryFilters.Contains(SelectedCategoryFilter) ? SelectedCategoryFilter : AllFilterValue;
        SelectedGenreFilter = GenreFilters.Contains(SelectedGenreFilter) ? SelectedGenreFilter : AllFilterValue;
        SelectedLanguageFilter = LanguageFilters.Contains(SelectedLanguageFilter) ? SelectedLanguageFilter : AllFilterValue;
    }

    private async Task ReloadTermsAsync()
    {
        _allTerms = (await _termPhraseRepository.GetAllAsync()).ToList();
        await ApplyFiltersAsync();
    }

    private async Task ApplyFiltersAsync()
    {
        IEnumerable<TermPhraseModel> candidates = _allTerms;

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var matches = await _termPhraseRepository.SearchAsync(SearchText);
            var matchedIds = matches.Select(term => term.Id).ToHashSet();
            candidates = _allTerms.Where(term => matchedIds.Contains(term.Id));
        }

        var filtered = candidates.Where(term =>
        {
            var matchesCategory = SelectedCategoryFilter == AllFilterValue
                || _categories.FirstOrDefault(category => category.Id == term.CategoryId)?.Name == SelectedCategoryFilter;

            var matchesGenre = SelectedGenreFilter == AllFilterValue
                || _genres.FirstOrDefault(genre => genre.Id == term.GenreId)?.Name == SelectedGenreFilter;

            var matchesLanguage = SelectedLanguageFilter == AllFilterValue
                || term.Language == SelectedLanguageFilter;

            return matchesCategory && matchesGenre && matchesLanguage;
        });

        Terms.Clear();
        foreach (var term in filtered)
        {
            Terms.Add(new TermRowViewModel
            {
                Term = term,
                CategoryName = _categories.FirstOrDefault(category => category.Id == term.CategoryId)?.Name ?? string.Empty,
                GenreName = _genres.FirstOrDefault(genre => genre.Id == term.GenreId)?.Name ?? string.Empty
            });
        }
    }

    private List<string> GetAvailableTags()
    {
        return _allTerms
            .SelectMany(term => term.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(tag => tag)
            .ToList();
    }
}
