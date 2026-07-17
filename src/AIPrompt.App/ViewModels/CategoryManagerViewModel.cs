using System.Collections.ObjectModel;
using AIPrompt.Core.Interfaces;
using AIPrompt.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AIPrompt.App.ViewModels;

public partial class CategoryManagerViewModel : ViewModelBase
{
    private readonly IPromptCategoryRepository _categoryRepository;

    [ObservableProperty]
    private PromptCategoryModel? _selectedCategory;

    [ObservableProperty]
    private string _editingName = string.Empty;

    [ObservableProperty]
    private string _editingDescription = string.Empty;

    [ObservableProperty]
    private string _editingIconKey = string.Empty;

    public CategoryManagerViewModel(IPromptCategoryRepository categoryRepository, IReadOnlyList<PromptCategoryModel> categories)
    {
        _categoryRepository = categoryRepository;
        Categories = new ObservableCollection<PromptCategoryModel>(categories);
    }

    public ObservableCollection<PromptCategoryModel> Categories { get; }

    partial void OnSelectedCategoryChanged(PromptCategoryModel? value)
    {
        EditingName = value?.Name ?? string.Empty;
        EditingDescription = value?.Description ?? string.Empty;
        EditingIconKey = value?.IconKey ?? string.Empty;
    }

    [RelayCommand]
    private async Task AddAsync()
    {
        if (string.IsNullOrWhiteSpace(EditingName))
        {
            return;
        }

        var created = await _categoryRepository.AddAsync(new PromptCategoryModel
        {
            Name = EditingName,
            Description = EditingDescription,
            IconKey = EditingIconKey
        });

        Categories.Add(created);
        SelectedCategory = created;
    }

    [RelayCommand]
    private async Task UpdateAsync()
    {
        if (SelectedCategory is null || string.IsNullOrWhiteSpace(EditingName))
        {
            return;
        }

        SelectedCategory.Name = EditingName;
        SelectedCategory.Description = EditingDescription;
        SelectedCategory.IconKey = EditingIconKey;

        await _categoryRepository.UpdateAsync(SelectedCategory);
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (SelectedCategory is null)
        {
            return;
        }

        await _categoryRepository.DeleteAsync(SelectedCategory.Id);
        Categories.Remove(SelectedCategory);
        SelectedCategory = null;
    }
}
