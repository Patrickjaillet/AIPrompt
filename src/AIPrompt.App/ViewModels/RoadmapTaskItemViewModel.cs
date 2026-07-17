using CommunityToolkit.Mvvm.ComponentModel;

namespace AIPrompt.App.ViewModels;

public partial class RoadmapTaskItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _description;

    [ObservableProperty]
    private bool _isChecked;

    public RoadmapTaskItemViewModel(string description, bool isChecked = false)
    {
        _description = description;
        _isChecked = isChecked;
    }
}
