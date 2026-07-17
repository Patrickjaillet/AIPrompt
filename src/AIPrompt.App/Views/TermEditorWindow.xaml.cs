using System.Windows;
using AIPrompt.App.ViewModels;

namespace AIPrompt.App.Views;

public partial class TermEditorWindow : Window
{
    public TermEditorWindow(TermEditorViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.CloseRequested += OnCloseRequested;
    }

    private TermEditorViewModel ViewModel => (TermEditorViewModel)DataContext;

    private void OnCloseRequested(object? sender, EventArgs e)
    {
        DialogResult = ViewModel.DialogResult;
        Close();
    }
}
