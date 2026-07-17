using System.Windows;
using AIPrompt.App.ViewModels;
using AIPrompt.App.Views;
using AIPrompt.Core.Models;
using Microsoft.Win32;

namespace AIPrompt.App.Services;

public class DialogService : IDialogService
{
    public bool? ShowTermEditor(TermEditorViewModel viewModel)
    {
        var window = new TermEditorWindow(viewModel)
        {
            Owner = Application.Current.MainWindow
        };

        return window.ShowDialog();
    }

    public void ShowCategoryManager(CategoryManagerViewModel viewModel)
    {
        var window = new CategoryManagerWindow(viewModel)
        {
            Owner = Application.Current.MainWindow
        };

        window.ShowDialog();
    }

    public void ShowGenreManager(GenreManagerViewModel viewModel)
    {
        var window = new GenreManagerWindow(viewModel)
        {
            Owner = Application.Current.MainWindow
        };

        window.ShowDialog();
    }

    public bool? ShowSavedPromptEditor(SavedPromptEditorViewModel viewModel)
    {
        var window = new SavedPromptEditorWindow(viewModel)
        {
            Owner = Application.Current.MainWindow
        };

        return window.ShowDialog();
    }

    public bool ShowConfirmation(string title, string message)
    {
        var window = new ConfirmationWindow(title, message)
        {
            Owner = Application.Current.MainWindow
        };

        return window.ShowDialog() == true;
    }

    public string? ShowSaveFileDialog(string defaultFileName, string filter)
    {
        var dialog = new SaveFileDialog
        {
            FileName = defaultFileName,
            Filter = filter
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    public string? ShowFolderPicker(string? initialFolder)
    {
        var dialog = new OpenFolderDialog
        {
            InitialDirectory = initialFolder
        };

        return dialog.ShowDialog() == true ? dialog.FolderName : null;
    }

    public string? ShowOpenFileDialog(string filter)
    {
        var dialog = new OpenFileDialog
        {
            Filter = filter
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    public ImportMode? ShowImportModeChoice()
    {
        var window = new ImportModeWindow
        {
            Owner = Application.Current.MainWindow
        };

        return window.ShowDialog() == true ? window.SelectedMode : null;
    }
}
