using System.Windows;
using AIPrompt.App.ViewModels;

namespace AIPrompt.App.Views;

public partial class CategoryManagerWindow : Window
{
    public CategoryManagerWindow(CategoryManagerViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
