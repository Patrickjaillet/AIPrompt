using AIPrompt.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AIPrompt.App.ViewModels;

public partial class SavedPromptEditorViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string _finalContent;

    public SavedPromptEditorViewModel(SavedPromptModel existingPrompt)
    {
        ExistingId = existingPrompt.Id;
        _title = existingPrompt.Title;
        _finalContent = existingPrompt.FinalContent;
    }

    public int ExistingId { get; }

    public bool? DialogResult { get; private set; }

    public event EventHandler? CloseRequested;

    [RelayCommand]
    private void Save()
    {
        if (string.IsNullOrWhiteSpace(Title))
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
}
