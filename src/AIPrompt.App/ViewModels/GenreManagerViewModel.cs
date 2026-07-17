using System.Collections.ObjectModel;
using AIPrompt.Core.Interfaces;
using AIPrompt.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AIPrompt.App.ViewModels;

public partial class GenreManagerViewModel : ViewModelBase
{
    private readonly IPromptGenreRepository _genreRepository;

    [ObservableProperty]
    private PromptGenreModel? _selectedGenre;

    [ObservableProperty]
    private string _editingName = string.Empty;

    [ObservableProperty]
    private string _editingDescription = string.Empty;

    public GenreManagerViewModel(IPromptGenreRepository genreRepository, IReadOnlyList<PromptGenreModel> genres)
    {
        _genreRepository = genreRepository;
        Genres = new ObservableCollection<PromptGenreModel>(genres);
    }

    public ObservableCollection<PromptGenreModel> Genres { get; }

    partial void OnSelectedGenreChanged(PromptGenreModel? value)
    {
        EditingName = value?.Name ?? string.Empty;
        EditingDescription = value?.Description ?? string.Empty;
    }

    [RelayCommand]
    private async Task AddAsync()
    {
        if (string.IsNullOrWhiteSpace(EditingName))
        {
            return;
        }

        var created = await _genreRepository.AddAsync(new PromptGenreModel
        {
            Name = EditingName,
            Description = EditingDescription
        });

        Genres.Add(created);
        SelectedGenre = created;
    }

    [RelayCommand]
    private async Task UpdateAsync()
    {
        if (SelectedGenre is null || string.IsNullOrWhiteSpace(EditingName))
        {
            return;
        }

        SelectedGenre.Name = EditingName;
        SelectedGenre.Description = EditingDescription;

        await _genreRepository.UpdateAsync(SelectedGenre);
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (SelectedGenre is null)
        {
            return;
        }

        await _genreRepository.DeleteAsync(SelectedGenre.Id);
        Genres.Remove(SelectedGenre);
        SelectedGenre = null;
    }
}
