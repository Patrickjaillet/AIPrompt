using CommunityToolkit.Mvvm.ComponentModel;
using Material.Icons;

namespace AIPrompt.App.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private NavigationItem _selectedNavigationItem;

    public MainViewModel(
        DashboardViewModel dashboardViewModel,
        TermLibraryViewModel termLibraryViewModel,
        PromptBuilderViewModel promptBuilderViewModel,
        SavedPromptsViewModel savedPromptsViewModel,
        RoadmapGeneratorViewModel roadmapGeneratorViewModel,
        ImportExportViewModel importExportViewModel,
        SettingsViewModel settingsViewModel,
        AboutViewModel aboutViewModel)
    {
        NavigationItems =
        [
            new NavigationItem("Nav_Dashboard", MaterialIconKind.ViewDashboard, dashboardViewModel),
            new NavigationItem("Nav_TermLibrary", MaterialIconKind.BookOpenVariant, termLibraryViewModel),
            new NavigationItem("Nav_PromptBuilder", MaterialIconKind.PuzzleOutline, promptBuilderViewModel),
            new NavigationItem("Nav_SavedPrompts", MaterialIconKind.ContentSave, savedPromptsViewModel),
            new NavigationItem("Nav_RoadmapGenerator", MaterialIconKind.MapMarkerPath, roadmapGeneratorViewModel),
            new NavigationItem("Nav_ImportExport", MaterialIconKind.SwapHorizontal, importExportViewModel),
            new NavigationItem("Nav_Settings", MaterialIconKind.Cog, settingsViewModel),
            new NavigationItem("Nav_About", MaterialIconKind.InformationOutline, aboutViewModel)
        ];

        _selectedNavigationItem = NavigationItems[0];
    }

    public List<NavigationItem> NavigationItems { get; }
}
