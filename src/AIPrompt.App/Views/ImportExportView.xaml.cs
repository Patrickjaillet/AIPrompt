using System.Windows.Controls;
using AIPrompt.App.ViewModels;

namespace AIPrompt.App.Views;

public partial class ImportExportView : UserControl
{
    public ImportExportView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is ImportExportViewModel viewModel)
        {
            viewModel.RefreshBackupFiles();
        }
    }
}
