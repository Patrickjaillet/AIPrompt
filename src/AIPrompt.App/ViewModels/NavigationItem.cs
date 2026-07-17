using System.ComponentModel;
using AIPrompt.App.Services;
using Material.Icons;

namespace AIPrompt.App.ViewModels;

public class NavigationItem : INotifyPropertyChanged
{
    private readonly string _titleKey;

    public NavigationItem(string titleKey, MaterialIconKind icon, ViewModelBase viewModel)
    {
        _titleKey = titleKey;
        Icon = icon;
        ViewModel = viewModel;
        Loc.Instance.PropertyChanged += (_, _) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
    }

    public string Title => Loc.Instance[_titleKey];

    public MaterialIconKind Icon { get; }

    public ViewModelBase ViewModel { get; }

    public event PropertyChangedEventHandler? PropertyChanged;
}
