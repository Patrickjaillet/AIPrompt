using System.Collections.ObjectModel;
using System.ComponentModel;
using AIPrompt.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AIPrompt.App.ViewModels;

public partial class RoadmapPhaseItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _title;

    public RoadmapPhaseItemViewModel(string title)
    {
        _title = title;
        Tasks.CollectionChanged += (_, _) => Changed?.Invoke(this, EventArgs.Empty);
        PropertyChanged += OnPropertyChanged;
    }

    public ObservableCollection<RoadmapTaskItemViewModel> Tasks { get; } = [];

    public event EventHandler? Changed;

    [RelayCommand]
    private void AddTask()
    {
        AddTaskItem(new RoadmapTaskItemViewModel(string.Empty));
    }

    public void AddTaskFromLibrary(string content)
    {
        AddTaskItem(new RoadmapTaskItemViewModel(content));
    }

    [RelayCommand]
    private void RemoveTask(RoadmapTaskItemViewModel task)
    {
        task.PropertyChanged -= OnTaskPropertyChanged;
        Tasks.Remove(task);
    }

    [RelayCommand]
    private void MoveTaskUp(RoadmapTaskItemViewModel task)
    {
        var index = Tasks.IndexOf(task);
        if (index > 0)
        {
            Tasks.Move(index, index - 1);
        }
    }

    [RelayCommand]
    private void MoveTaskDown(RoadmapTaskItemViewModel task)
    {
        var index = Tasks.IndexOf(task);
        if (index >= 0 && index < Tasks.Count - 1)
        {
            Tasks.Move(index, index + 1);
        }
    }

    private void AddTaskItem(RoadmapTaskItemViewModel task)
    {
        task.PropertyChanged += OnTaskPropertyChanged;
        Tasks.Add(task);
    }

    private void OnTaskPropertyChanged(object? sender, PropertyChangedEventArgs e) => Changed?.Invoke(this, EventArgs.Empty);

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) => Changed?.Invoke(this, EventArgs.Empty);
}
