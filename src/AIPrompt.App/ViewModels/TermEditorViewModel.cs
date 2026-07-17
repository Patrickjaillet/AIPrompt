using AIPrompt.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AIPrompt.App.ViewModels;

public partial class TermEditorViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _content;

    [ObservableProperty]
    private PromptCategoryModel? _selectedCategory;

    [ObservableProperty]
    private PromptGenreModel? _selectedGenre;

    [ObservableProperty]
    private string _tags;

    [ObservableProperty]
    private string _language;

    public TermEditorViewModel(
        IReadOnlyList<PromptCategoryModel> categories,
        IReadOnlyList<PromptGenreModel> genres,
        IReadOnlyList<string> availableTags,
        TermPhraseModel? existingTerm)
    {
        Categories = categories;
        Genres = genres;
        AvailableTags = availableTags;
        Languages = ["FR", "EN"];

        IsEditMode = existingTerm is not null;
        ExistingId = existingTerm?.Id ?? 0;

        _content = existingTerm?.Content ?? string.Empty;
        _tags = existingTerm?.Tags ?? string.Empty;
        _language = existingTerm?.Language ?? "FR";
        _selectedCategory = categories.FirstOrDefault(category => category.Id == existingTerm?.CategoryId) ?? categories.FirstOrDefault();
        _selectedGenre = genres.FirstOrDefault(genre => genre.Id == existingTerm?.GenreId) ?? genres.FirstOrDefault();
    }

    public IReadOnlyList<PromptCategoryModel> Categories { get; }

    public IReadOnlyList<PromptGenreModel> Genres { get; }

    public IReadOnlyList<string> AvailableTags { get; }

    public IReadOnlyList<string> Languages { get; }

    public bool IsEditMode { get; }

    public string DialogTitle => IsEditMode ? "Modifier le terme" : "Nouveau terme";

    public int ExistingId { get; }

    public bool? DialogResult { get; private set; }

    public event EventHandler? CloseRequested;

    [RelayCommand]
    private void Save()
    {
        if (string.IsNullOrWhiteSpace(Content) || SelectedCategory is null || SelectedGenre is null)
        {
            return;
        }

        DialogResult = true;
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void Cancel()
    {
        DialogResult = false;
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    public TermPhraseModel ToModel()
    {
        return new TermPhraseModel
        {
            Id = ExistingId,
            Content = Content,
            CategoryId = SelectedCategory!.Id,
            GenreId = SelectedGenre!.Id,
            Tags = Tags,
            Language = Language
        };
    }
}
