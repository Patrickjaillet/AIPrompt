using System.Windows;
using AIPrompt.App.ViewModels;

namespace AIPrompt.App.Views;

public partial class SavedPromptEditorWindow : Window
{
    public SavedPromptEditorWindow(SavedPromptEditorViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.CloseRequested += OnCloseRequested;
    }

    private SavedPromptEditorViewModel ViewModel => (SavedPromptEditorViewModel)DataContext;

    private void OnCloseRequested(object? sender, EventArgs e)
    {
        DialogResult = ViewModel.DialogResult;
        Close();
    }
}
