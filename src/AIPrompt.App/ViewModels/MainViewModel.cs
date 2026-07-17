using AIPrompt.App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;

namespace AIPrompt.App.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly ThemeService _themeService;

    [ObservableProperty]
    private NavigationItem _selectedNavigationItem;

    public MainViewModel(
        ThemeService themeService,
        DashboardViewModel dashboardViewModel,
        TermLibraryViewModel termLibraryViewModel,
        PromptBuilderViewModel promptBuilderViewModel,
        SavedPromptsViewModel savedPromptsViewModel,
        RoadmapGeneratorViewModel roadmapGeneratorViewModel,
        ImportExportViewModel importExportViewModel,
        SettingsViewModel settingsViewModel,
        AboutViewModel aboutViewModel)
    {
        _themeService = themeService;

        NavigationItems =
        [
            new NavigationItem { Title = "Accueil", Icon = MaterialIconKind.ViewDashboard, ViewModel = dashboardViewModel },
            new NavigationItem { Title = "Bibliothèque de termes", Icon = MaterialIconKind.BookOpenVariant, ViewModel = termLibraryViewModel },
            new NavigationItem { Title = "Assembleur de prompt", Icon = MaterialIconKind.PuzzleOutline, ViewModel = promptBuilderViewModel },
            new NavigationItem { Title = "Prompts sauvegardés", Icon = MaterialIconKind.ContentSave, ViewModel = savedPromptsViewModel },
            new NavigationItem { Title = "Générateur de ROADMAP", Icon = MaterialIconKind.MapMarkerPath, ViewModel = roadmapGeneratorViewModel },
            new NavigationItem { Title = "Import / Export", Icon = MaterialIconKind.SwapHorizontal, ViewModel = importExportViewModel },
            new NavigationItem { Title = "Paramètres", Icon = MaterialIconKind.Cog, ViewModel = settingsViewModel },
            new NavigationItem { Title = "À propos", Icon = MaterialIconKind.InformationOutline, ViewModel = aboutViewModel }
        ];

        _selectedNavigationItem = NavigationItems[0];
    }

    public List<NavigationItem> NavigationItems { get; }

    [RelayCommand]
    private void ToggleTheme()
    {
        _themeService.ToggleBaseTheme();
    }
}
