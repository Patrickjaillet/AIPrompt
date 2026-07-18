using AIPrompt.App.ViewModels;
using AIPrompt.Core.Models;

namespace AIPrompt.App.Services;

public interface IDialogService
{
    bool? ShowTermEditor(TermEditorViewModel viewModel);

    void ShowCategoryManager(CategoryManagerViewModel viewModel);

    void ShowGenreManager(GenreManagerViewModel viewModel);

    bool? ShowSavedPromptEditor(SavedPromptEditorViewModel viewModel);

    bool ShowConfirmation(string title, string message);

    string? ShowSaveFileDialog(string defaultFileName, string filter, string? initialDirectory = null);

    string? ShowOpenFileDialog(string filter);

    string? ShowFolderPicker(string? initialFolder);

    ImportMode? ShowImportModeChoice();

    void ShowPresentationMode(string title, string markdownContent);
}
