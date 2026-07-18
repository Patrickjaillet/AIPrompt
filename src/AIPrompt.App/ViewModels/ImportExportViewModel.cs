using System.Collections.ObjectModel;
using System.IO;
using AIPrompt.App.Services;
using AIPrompt.Core.Interfaces;
using AIPrompt.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AIPrompt.App.ViewModels;

public partial class ImportExportViewModel : ViewModelBase
{
    private readonly IBackupService _backupService;
    private readonly ITermPackService _termPackService;
    private readonly IPromptCategoryRepository _categoryRepository;
    private readonly IDialogService _dialogService;
    private readonly ISettingsService _settingsService;
    private readonly IAutoBackupService _autoBackupService;
    private readonly ILanguageService _languageService;

    [ObservableProperty]
    private bool _autoBackupEnabled;

    [ObservableProperty]
    private int _autoBackupIntervalMinutes;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private PromptCategoryModel? _selectedExportPackCategory;

    [ObservableProperty]
    private string _exportPackName = string.Empty;

    public ImportExportViewModel(
        IBackupService backupService,
        ITermPackService termPackService,
        IPromptCategoryRepository categoryRepository,
        IDialogService dialogService,
        ISettingsService settingsService,
        IAutoBackupService autoBackupService,
        ILanguageService languageService)
    {
        _backupService = backupService;
        _termPackService = termPackService;
        _categoryRepository = categoryRepository;
        _dialogService = dialogService;
        _settingsService = settingsService;
        _autoBackupService = autoBackupService;
        _languageService = languageService;

        _autoBackupEnabled = settingsService.AutoBackupEnabled;
        _autoBackupIntervalMinutes = settingsService.AutoBackupIntervalMinutes;
    }

    public ObservableCollection<string> BackupFiles { get; } = [];

    public ObservableCollection<PromptCategoryModel> ExportPackCategories { get; } = [];

    public void RefreshBackupFiles()
    {
        BackupFiles.Clear();
        foreach (var file in Directory.GetFiles(_settingsService.BackupsFolder, "*.json").OrderByDescending(f => f))
        {
            BackupFiles.Add(file);
        }
    }

    public async Task RefreshExportPackCategoriesAsync()
    {
        ExportPackCategories.Clear();
        foreach (var category in await _categoryRepository.GetAllAsync())
        {
            ExportPackCategories.Add(category);
        }
        SelectedExportPackCategory ??= ExportPackCategories.FirstOrDefault();
    }

    partial void OnAutoBackupEnabledChanged(bool value) => ApplyAutoBackupSettings(value, AutoBackupIntervalMinutes);

    partial void OnAutoBackupIntervalMinutesChanged(int value) => ApplyAutoBackupSettings(AutoBackupEnabled, value);

    private void ApplyAutoBackupSettings(bool enabled, int intervalMinutes)
    {
        _ = _settingsService.SetAutoBackupAsync(enabled, intervalMinutes).ContinueWith(_ => _autoBackupService.Reconfigure());
    }

    [RelayCommand]
    private async Task ExportAsync()
    {
        var path = _dialogService.ShowSaveFileDialog("aiprompt-backup.json", "JSON (*.json)|*.json");
        if (path is null)
        {
            return;
        }

        var json = await _backupService.ExportToJsonAsync();
        await File.WriteAllTextAsync(path, json);
        StatusMessage = string.Format(_languageService.GetString("ImportExport_ExportedTo"), path);
    }

    [RelayCommand]
    private async Task ImportAsync()
    {
        var path = _dialogService.ShowOpenFileDialog("JSON (*.json)|*.json");
        if (path is null)
        {
            return;
        }

        var mode = _dialogService.ShowImportModeChoice();
        if (mode is null)
        {
            return;
        }

        var json = await File.ReadAllTextAsync(path);
        await _backupService.ImportFromJsonAsync(json, mode.Value);
        StatusMessage = mode == ImportMode.Merge
            ? _languageService.GetString("ImportExport_MergeSuccess")
            : _languageService.GetString("ImportExport_OverwriteSuccess");
    }

    [RelayCommand]
    private async Task RestoreAsync(string backupFilePath)
    {
        var confirmed = _dialogService.ShowConfirmation(
            _languageService.GetString("Dialog_RestoreBackupTitle"),
            string.Format(_languageService.GetString("Dialog_RestoreBackupMessage"), Path.GetFileName(backupFilePath)));

        if (!confirmed)
        {
            return;
        }

        var json = await File.ReadAllTextAsync(backupFilePath);
        await _backupService.ImportFromJsonAsync(json, ImportMode.Overwrite);
        StatusMessage = string.Format(_languageService.GetString("ImportExport_RestoredFrom"), Path.GetFileName(backupFilePath));
    }

    [RelayCommand]
    private async Task ImportTermPackAsync()
    {
        var path = _dialogService.ShowOpenFileDialog("JSON (*.json)|*.json");
        if (path is null)
        {
            return;
        }

        var json = await File.ReadAllTextAsync(path);
        var result = await _termPackService.ImportPackAsync(json);
        StatusMessage = string.Format(
            _languageService.GetString("ImportExport_PackImported"),
            result.PackName,
            result.CategoryName,
            result.ImportedCount,
            result.SkippedCount);
    }

    [RelayCommand]
    private async Task ExportTermPackAsync()
    {
        if (SelectedExportPackCategory is null)
        {
            return;
        }

        var packName = string.IsNullOrWhiteSpace(ExportPackName) ? SelectedExportPackCategory.Name : ExportPackName;
        var path = _dialogService.ShowSaveFileDialog($"{packName}.json", "JSON (*.json)|*.json");
        if (path is null)
        {
            return;
        }

        var json = await _termPackService.ExportPackAsync(SelectedExportPackCategory.Id, packName);
        await File.WriteAllTextAsync(path, json);
        StatusMessage = string.Format(_languageService.GetString("ImportExport_PackExportedTo"), path);
    }
}
