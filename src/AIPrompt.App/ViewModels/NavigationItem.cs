using Material.Icons;

namespace AIPrompt.App.ViewModels;

public class NavigationItem
{
    public required string Title { get; init; }

    public required MaterialIconKind Icon { get; init; }

    public required ViewModelBase ViewModel { get; init; }
}
