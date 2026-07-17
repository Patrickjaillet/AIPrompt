using System.Windows.Controls;
using AIPrompt.App.ViewModels;

namespace AIPrompt.App.Views;

public partial class RoadmapGeneratorView : UserControl
{
    public RoadmapGeneratorView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is RoadmapGeneratorViewModel viewModel)
        {
            await viewModel.InitializeAsync();
        }
    }
}
