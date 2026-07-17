using System.Windows;
using AIPrompt.App.ViewModels;

namespace AIPrompt.App.Views;

public partial class GenreManagerWindow : Window
{
    public GenreManagerWindow(GenreManagerViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
