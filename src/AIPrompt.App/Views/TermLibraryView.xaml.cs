using System.Windows.Controls;
using AIPrompt.App.ViewModels;

namespace AIPrompt.App.Views;

public partial class TermLibraryView : UserControl
{
    public TermLibraryView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is TermLibraryViewModel viewModel)
        {
            await viewModel.InitializeAsync();
        }
    }
}
