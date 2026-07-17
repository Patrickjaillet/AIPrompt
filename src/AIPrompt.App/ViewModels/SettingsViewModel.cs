using AIPrompt.App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AIPrompt.App.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly ISettingsService _settingsService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private string _defaultExportFolder;

    public SettingsViewModel(ISettingsService settingsService, IDialogService dialogService)
    {
        _settingsService = settingsService;
        _dialogService = dialogService;
        _defaultExportFolder = settingsService.DefaultExportFolder ?? string.Empty;
    }

    [RelayCommand]
    private async Task BrowseExportFolderAsync()
    {
        var folder = _dialogService.ShowFolderPicker(DefaultExportFolder);
        if (folder is null)
        {
            return;
        }

        DefaultExportFolder = folder;
        await _settingsService.SetDefaultExportFolderAsync(folder);
    }
}
