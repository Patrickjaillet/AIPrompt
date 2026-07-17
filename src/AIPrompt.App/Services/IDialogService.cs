using AIPrompt.App.ViewModels;

namespace AIPrompt.App.Services;

public interface IDialogService
{
    bool? ShowTermEditor(TermEditorViewModel viewModel);

    void ShowCategoryManager(CategoryManagerViewModel viewModel);

    void ShowGenreManager(GenreManagerViewModel viewModel);
}
