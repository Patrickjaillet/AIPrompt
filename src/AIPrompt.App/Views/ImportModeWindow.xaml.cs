using System.Windows;
using AIPrompt.Core.Models;

namespace AIPrompt.App.Views;

public partial class ImportModeWindow : Window
{
    public ImportModeWindow()
    {
        InitializeComponent();
    }

    public ImportMode? SelectedMode { get; private set; }

    private void OnMergeClick(object sender, RoutedEventArgs e)
    {
        SelectedMode = ImportMode.Merge;
        DialogResult = true;
        Close();
    }

    private void OnOverwriteClick(object sender, RoutedEventArgs e)
    {
        SelectedMode = ImportMode.Overwrite;
        DialogResult = true;
        Close();
    }

    private void OnCancelClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
