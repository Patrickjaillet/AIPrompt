using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using AIPrompt.App.Services;
using AIPrompt.Core.Interfaces;
using AIPrompt.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GongSolutions.Wpf.DragDrop;
using Markdig;

namespace AIPrompt.App.ViewModels;

public partial class PromptBuilderViewModel : ViewModelBase, IDropTarget
{
    private const string AllFilterValue = "Toutes";

    private readonly ITermPhraseRepository _termPhraseRepository;
    private readonly IPromptCategoryRepository _categoryRepository;
    private readonly IPromptTemplateRepository _templateRepository;
    private readonly ISavedPromptRepository _savedPromptRepository;
    private readonly IDialogService _dialogService;

    private List<TermPhraseModel> _allLibraryTerms = [];
    private List<PromptCategoryModel> _categories = [];

    [ObservableProperty]
    private string _librarySearchText = string.Empty;

    [ObservableProperty]
    private string _selectedCategoryFilter = AllFilterValue;

    [ObservableProperty]
    private string _previewMarkdown = string.Empty;

    [ObservableProperty]
    private string _templateTitle = string.Empty;

    [ObservableProperty]
    private PromptCategoryModel? _selectedCategory;

    [ObservableProperty]
    private PromptTemplateModel? _selectedTemplateToLoad;

    public PromptBuilderViewModel(
        ITermPhraseRepository termPhraseRepository,
        IPromptCategoryRepository categoryRepository,
        IPromptTemplateRepository templateRepository,
        ISavedPromptRepository savedPromptRepository,
        IDialogService dialogService)
    {
        _termPhraseRepository = termPhraseRepository;
        _categoryRepository = categoryRepository;
        _templateRepository = templateRepository;
        _savedPromptRepository = savedPromptRepository;
        _dialogService = dialogService;

        Blocks.CollectionChanged += (_, _) => RefreshPreview();
    }

    public ObservableCollection<TermPhraseModel> LibraryTerms { get; } = [];

    public ObservableCollection<string> CategoryFilters { get; } = [AllFilterValue];

    public ObservableCollection<PromptBlockItemViewModel> Blocks { get; } = [];

    public ObservableCollection<PromptTemplateModel> Templates { get; } = [];

    public async Task InitializeAsync()
    {
        _categories = (await _categoryRepository.GetAllAsync()).ToList();

        CategoryFilters.Clear();
        CategoryFilters.Add(AllFilterValue);
        foreach (var category in _categories)
        {
            CategoryFilters.Add(category.Name);
        }

        SelectedCategoryFilter = AllFilterValue;
        SelectedCategory = _categories.FirstOrDefault();

        Templates.Clear();
        foreach (var template in await _templateRepository.GetAllAsync())
        {
            Templates.Add(template);
        }

        _allLibraryTerms = (await _termPhraseRepository.GetAllAsync()).ToList();
        ApplyLibraryFilter();
    }

    partial void OnLibrarySearchTextChanged(string value) => ApplyLibraryFilter();

    partial void OnSelectedCategoryFilterChanged(string value) => ApplyLibraryFilter();

    [RelayCommand]
    private void AddLibraryTerm(TermPhraseModel term)
    {
        AddBlock(new PromptBlockItemViewModel(term.Id, term.Content));
    }

    [RelayCommand]
    private void AddFreeTextBlock()
    {
        AddBlock(new PromptBlockItemViewModel(termPhraseId: null, content: string.Empty));
    }

    [RelayCommand]
    private void RemoveBlock(PromptBlockItemViewModel block)
    {
        block.PropertyChanged -= OnBlockPropertyChanged;
        Blocks.Remove(block);
    }

    [RelayCommand]
    private void MoveBlockUp(PromptBlockItemViewModel block)
    {
        var index = Blocks.IndexOf(block);
        if (index > 0)
        {
            Blocks.Move(index, index - 1);
        }
    }

    [RelayCommand]
    private void MoveBlockDown(PromptBlockItemViewModel block)
    {
        var index = Blocks.IndexOf(block);
        if (index >= 0 && index < Blocks.Count - 1)
        {
            Blocks.Move(index, index + 1);
        }
    }

    [RelayCommand]
    private async Task SaveTemplateAsync()
    {
        if (string.IsNullOrWhiteSpace(TemplateTitle) || SelectedCategory is null || Blocks.Count == 0)
        {
            return;
        }

        var model = new PromptTemplateModel
        {
            Title = TemplateTitle,
            CategoryId = SelectedCategory.Id,
            Blocks = Blocks.Select((block, index) => new PromptBlockModel
            {
                OrderIndex = index,
                TermPhraseId = block.TermPhraseId,
                FreeText = block.IsFreeText ? block.Content : null
            }).ToList()
        };

        var created = await _templateRepository.AddAsync(model);
        Templates.Add(created);
    }

    [RelayCommand]
    private async Task LoadTemplateAsync()
    {
        if (SelectedTemplateToLoad is null)
        {
            return;
        }

        var template = await _templateRepository.GetByIdAsync(SelectedTemplateToLoad.Id);
        if (template is null)
        {
            return;
        }

        foreach (var block in Blocks)
        {
            block.PropertyChanged -= OnBlockPropertyChanged;
        }

        Blocks.Clear();
        foreach (var block in template.Blocks)
        {
            AddBlock(new PromptBlockItemViewModel(block.TermPhraseId, block.Content));
        }

        TemplateTitle = template.Title;
        SelectedCategory = _categories.FirstOrDefault(category => category.Id == template.CategoryId);
    }

    [RelayCommand]
    private async Task GenerateFinalPromptAsync()
    {
        if (Blocks.Count == 0)
        {
            return;
        }

        var title = string.IsNullOrWhiteSpace(TemplateTitle) ? "Prompt assemblé" : TemplateTitle;

        await _savedPromptRepository.AddAsync(new SavedPromptModel
        {
            Title = title,
            FinalContent = PreviewMarkdown,
            ExportFormat = PromptExportFormat.Markdown
        });
    }

    [RelayCommand]
    private void EnterPresentationMode()
    {
        if (Blocks.Count == 0)
        {
            return;
        }

        var title = string.IsNullOrWhiteSpace(TemplateTitle) ? "Prompt" : TemplateTitle;
        _dialogService.ShowPresentationMode(title, PreviewMarkdown);
    }

    public void DragOver(IDropInfo dropInfo)
    {
        if (dropInfo.Data is TermPhraseModel or PromptBlockItemViewModel)
        {
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            dropInfo.Effects = DragDropEffects.Move;
        }
    }

    public void Drop(IDropInfo dropInfo)
    {
        var insertIndex = Math.Clamp(dropInfo.UnfilteredInsertIndex, 0, Blocks.Count);

        if (dropInfo.Data is TermPhraseModel term)
        {
            var block = new PromptBlockItemViewModel(term.Id, term.Content);
            block.PropertyChanged += OnBlockPropertyChanged;
            Blocks.Insert(insertIndex, block);
        }
        else if (dropInfo.Data is PromptBlockItemViewModel existingBlock)
        {
            var oldIndex = Blocks.IndexOf(existingBlock);
            if (oldIndex < 0)
            {
                return;
            }

            var newIndex = insertIndex > oldIndex ? insertIndex - 1 : insertIndex;
            Blocks.Move(oldIndex, Math.Clamp(newIndex, 0, Blocks.Count - 1));
        }
    }

    private void AddBlock(PromptBlockItemViewModel block)
    {
        block.PropertyChanged += OnBlockPropertyChanged;
        Blocks.Add(block);
    }

    private void OnBlockPropertyChanged(object? sender, PropertyChangedEventArgs e) => RefreshPreview();

    private void RefreshPreview()
    {
        var content = string.Join("\n\n", Blocks.Select(block => block.Content));
        Markdown.ToHtml(content);
        PreviewMarkdown = content;
    }

    private void ApplyLibraryFilter()
    {
        var filtered = _allLibraryTerms.Where(term =>
        {
            var matchesSearch = string.IsNullOrWhiteSpace(LibrarySearchText)
                || term.Content.Contains(LibrarySearchText, StringComparison.OrdinalIgnoreCase)
                || term.Tags.Contains(LibrarySearchText, StringComparison.OrdinalIgnoreCase);

            var matchesCategory = SelectedCategoryFilter == AllFilterValue
                || _categories.FirstOrDefault(category => category.Id == term.CategoryId)?.Name == SelectedCategoryFilter;

            return matchesSearch && matchesCategory;
        });

        LibraryTerms.Clear();
        foreach (var term in filtered)
        {
            LibraryTerms.Add(term);
        }
    }
}
