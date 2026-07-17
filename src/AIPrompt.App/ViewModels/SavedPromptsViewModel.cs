using System.Collections.ObjectModel;
using System.IO;
using AIPrompt.App.Services;
using AIPrompt.Core.Interfaces;
using AIPrompt.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AIPrompt.App.ViewModels;

public partial class SavedPromptsViewModel : ViewModelBase
{
    private readonly ISavedPromptRepository _savedPromptRepository;
    private readonly IDialogService _dialogService;
    private readonly PromptExportService _exportService;
    private readonly ISettingsService _settingsService;

    private List<SavedPromptModel> _allPrompts = [];

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _sortOption = SortByDate;

    private const string SortByDate = "Date";
    private const string SortByTitle = "Titre";
    private const string SortByCategory = "Catégorie";

    public SavedPromptsViewModel(
        ISavedPromptRepository savedPromptRepository,
        IDialogService dialogService,
        PromptExportService exportService,
        ISettingsService settingsService)
    {
        _savedPromptRepository = savedPromptRepository;
        _dialogService = dialogService;
        _exportService = exportService;
        _settingsService = settingsService;
    }

    public ObservableCollection<SavedPromptModel> Prompts { get; } = [];

    public ObservableCollection<string> SortOptions { get; } = [SortByDate, SortByTitle, SortByCategory];

    public ObservableCollection<SavedPromptModel> SelectedPrompts { get; } = [];

    public async Task InitializeAsync()
    {
        await ReloadAsync();
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    partial void OnSortOptionChanged(string value) => ApplyFilter();

    [RelayCommand]
    private async Task EditAsync(SavedPromptModel prompt)
    {
        var editorViewModel = new SavedPromptEditorViewModel(prompt);

        if (_dialogService.ShowSavedPromptEditor(editorViewModel) == true)
        {
            prompt.Title = editorViewModel.Title;
            prompt.FinalContent = editorViewModel.FinalContent;
            await _savedPromptRepository.UpdateAsync(prompt);
            await ReloadAsync();
        }
    }

    [RelayCommand]
    private async Task DuplicateAsync(SavedPromptModel prompt)
    {
        await _savedPromptRepository.AddAsync(new SavedPromptModel
        {
            Title = $"{prompt.Title} (copie)",
            FinalContent = prompt.FinalContent,
            SourceTemplateId = prompt.SourceTemplateId,
            ExportFormat = prompt.ExportFormat
        });

        await ReloadAsync();
    }

    [RelayCommand]
    private async Task DeleteAsync(SavedPromptModel prompt)
    {
        var confirmed = _dialogService.ShowConfirmation(
            "Supprimer le prompt",
            $"Voulez-vous vraiment supprimer « {prompt.Title} » ? Cette action est irréversible.");

        if (!confirmed)
        {
            return;
        }

        await _savedPromptRepository.DeleteAsync(prompt.Id);
        await ReloadAsync();
    }

    [RelayCommand]
    private async Task ExportMarkdownAsync(SavedPromptModel prompt)
    {
        var path = _dialogService.ShowSaveFileDialog($"{prompt.Title}.md", "Markdown (*.md)|*.md");
        if (path is null)
        {
            return;
        }

        await _exportService.ExportAsync(prompt, path, PromptExportFormat.Markdown);
    }

    [RelayCommand]
    private async Task ExportPlainTextAsync(SavedPromptModel prompt)
    {
        var path = _dialogService.ShowSaveFileDialog($"{prompt.Title}.txt", "Texte brut (*.txt)|*.txt");
        if (path is null)
        {
            return;
        }

        await _exportService.ExportAsync(prompt, path, PromptExportFormat.PlainText);
    }

    [RelayCommand]
    private async Task ExportSelectedAsync()
    {
        if (SelectedPrompts.Count == 0)
        {
            return;
        }

        var folder = _dialogService.ShowFolderPicker(_settingsService.DefaultExportFolder);
        if (folder is null)
        {
            return;
        }

        foreach (var prompt in SelectedPrompts)
        {
            var extension = prompt.ExportFormat == PromptExportFormat.Markdown ? "md" : "txt";
            var fileName = $"{SanitizeFileName(prompt.Title)}.{extension}";
            var filePath = Path.Combine(folder, fileName);
            await _exportService.ExportAsync(prompt, filePath, prompt.ExportFormat);
        }
    }

    private async Task ReloadAsync()
    {
        _allPrompts = (await _savedPromptRepository.GetAllAsync()).ToList();
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        IEnumerable<SavedPromptModel> filtered = _allPrompts.Where(prompt =>
            string.IsNullOrWhiteSpace(SearchText)
            || prompt.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
            || prompt.FinalContent.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        filtered = SortOption switch
        {
            SortByTitle => filtered.OrderBy(prompt => prompt.Title),
            SortByCategory => filtered.OrderBy(prompt => prompt.CategoryName),
            _ => filtered.OrderByDescending(prompt => prompt.UpdatedAt)
        };

        Prompts.Clear();
        foreach (var prompt in filtered)
        {
            Prompts.Add(prompt);
        }
    }

    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        return string.Concat(fileName.Select(character => invalidChars.Contains(character) ? '_' : character));
    }
}
