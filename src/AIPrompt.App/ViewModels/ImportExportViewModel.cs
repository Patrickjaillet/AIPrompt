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
    private readonly IDialogService _dialogService;
    private readonly ISettingsService _settingsService;
    private readonly IAutoBackupService _autoBackupService;

    [ObservableProperty]
    private bool _autoBackupEnabled;

    [ObservableProperty]
    private int _autoBackupIntervalMinutes;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public ImportExportViewModel(
        IBackupService backupService,
        IDialogService dialogService,
        ISettingsService settingsService,
        IAutoBackupService autoBackupService)
    {
        _backupService = backupService;
        _dialogService = dialogService;
        _settingsService = settingsService;
        _autoBackupService = autoBackupService;

        _autoBackupEnabled = settingsService.AutoBackupEnabled;
        _autoBackupIntervalMinutes = settingsService.AutoBackupIntervalMinutes;
    }

    public ObservableCollection<string> BackupFiles { get; } = [];

    public void RefreshBackupFiles()
    {
        BackupFiles.Clear();
        foreach (var file in Directory.GetFiles(_settingsService.BackupsFolder, "*.json").OrderByDescending(f => f))
        {
            BackupFiles.Add(file);
        }
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
        StatusMessage = $"Export réalisé vers {path}";
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
            ? "Import fusionné avec succès."
            : "Bibliothèque remplacée avec succès.";
    }

    [RelayCommand]
    private async Task RestoreAsync(string backupFilePath)
    {
        var confirmed = _dialogService.ShowConfirmation(
            "Restaurer la sauvegarde",
            $"Voulez-vous restaurer « {Path.GetFileName(backupFilePath)} » ? La bibliothèque actuelle sera intégralement remplacée.");

        if (!confirmed)
        {
            return;
        }

        var json = await File.ReadAllTextAsync(backupFilePath);
        await _backupService.ImportFromJsonAsync(json, ImportMode.Overwrite);
        StatusMessage = $"Bibliothèque restaurée depuis {Path.GetFileName(backupFilePath)}";
    }
}
