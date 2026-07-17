using System.Windows;
using AIPrompt.App.ViewModels;
using AIPrompt.App.Views;

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
}
