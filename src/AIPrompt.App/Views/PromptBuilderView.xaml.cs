using System.Windows.Controls;
using AIPrompt.App.ViewModels;

namespace AIPrompt.App.Views;

public partial class PromptBuilderView : UserControl
{
    public PromptBuilderView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is PromptBuilderViewModel viewModel)
        {
            await viewModel.InitializeAsync();
        }
    }
}
