using System.Windows.Controls;
using AIPrompt.App.ViewModels;

namespace AIPrompt.App.Views;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is DashboardViewModel viewModel)
        {
            await viewModel.InitializeAsync();
        }
    }
}
