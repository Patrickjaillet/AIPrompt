using System.Windows;
using AIPrompt.App.ViewModels;

namespace AIPrompt.App;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
