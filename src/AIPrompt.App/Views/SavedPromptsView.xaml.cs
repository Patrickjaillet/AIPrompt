using System.Windows.Controls;
using AIPrompt.App.ViewModels;
using AIPrompt.Core.Models;

namespace AIPrompt.App.Views;

public partial class SavedPromptsView : UserControl
{
    public SavedPromptsView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is SavedPromptsViewModel viewModel)
        {
            await viewModel.InitializeAsync();
        }
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is not SavedPromptsViewModel viewModel)
        {
            return;
        }

        viewModel.SelectedPrompts.Clear();
        foreach (var item in PromptsDataGrid.SelectedItems)
        {
            if (item is SavedPromptModel prompt)
            {
                viewModel.SelectedPrompts.Add(prompt);
            }
        }
    }
}
