using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using AIPrompt.App.Services;
using AIPrompt.Core.Interfaces;
using AIPrompt.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AIPrompt.App.ViewModels;

public partial class RoadmapGeneratorViewModel : ViewModelBase
{
    private readonly IRoadmapProjectRepository _roadmapProjectRepository;
    private readonly ITermPhraseRepository _termPhraseRepository;
    private readonly IDialogService _dialogService;
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private string _projectName = string.Empty;

    [ObservableProperty]
    private string _projectDescription = string.Empty;

    [ObservableProperty]
    private string _previewMarkdown = string.Empty;

    [ObservableProperty]
    private TermPhraseModel? _selectedLibraryTerm;

    [ObservableProperty]
    private RoadmapProjectModel? _selectedProjectToLoad;

    public RoadmapGeneratorViewModel(
        IRoadmapProjectRepository roadmapProjectRepository,
        ITermPhraseRepository termPhraseRepository,
        IDialogService dialogService,
        ISettingsService settingsService)
    {
        _roadmapProjectRepository = roadmapProjectRepository;
        _termPhraseRepository = termPhraseRepository;
        _dialogService = dialogService;
        _settingsService = settingsService;

        Phases.CollectionChanged += (_, _) => RefreshPreview();
    }

    public ObservableCollection<RoadmapPhaseItemViewModel> Phases { get; } = [];

    public ObservableCollection<TermPhraseModel> LibraryTerms { get; } = [];

    public ObservableCollection<RoadmapProjectModel> SavedProjects { get; } = [];

    public async Task InitializeAsync()
    {
        LibraryTerms.Clear();
        foreach (var term in await _termPhraseRepository.GetAllAsync())
        {
            LibraryTerms.Add(term);
        }

        SavedProjects.Clear();
        foreach (var project in await _roadmapProjectRepository.GetAllAsync())
        {
            SavedProjects.Add(project);
        }
    }

    partial void OnProjectNameChanged(string value) => RefreshPreview();

    partial void OnProjectDescriptionChanged(string value) => RefreshPreview();

    [RelayCommand]
    private void AddPhase()
    {
        AddPhaseItem(new RoadmapPhaseItemViewModel("Nouvelle phase"));
    }

    [RelayCommand]
    private void RemovePhase(RoadmapPhaseItemViewModel phase)
    {
        phase.Changed -= OnPhaseChanged;
        Phases.Remove(phase);
    }

    [RelayCommand]
    private void MovePhaseUp(RoadmapPhaseItemViewModel phase)
    {
        var index = Phases.IndexOf(phase);
        if (index > 0)
        {
            Phases.Move(index, index - 1);
        }
    }

    [RelayCommand]
    private void MovePhaseDown(RoadmapPhaseItemViewModel phase)
    {
        var index = Phases.IndexOf(phase);
        if (index >= 0 && index < Phases.Count - 1)
        {
            Phases.Move(index, index + 1);
        }
    }

    [RelayCommand]
    private void AddTaskFromLibrary(RoadmapPhaseItemViewModel phase)
    {
        if (SelectedLibraryTerm is null)
        {
            return;
        }

        phase.AddTaskFromLibrary(SelectedLibraryTerm.Content);
    }

    [RelayCommand]
    private async Task SaveAsTemplateAsync()
    {
        if (string.IsNullOrWhiteSpace(ProjectName) || Phases.Count == 0)
        {
            return;
        }

        var model = BuildModel();
        var created = await _roadmapProjectRepository.AddAsync(model);
        SavedProjects.Add(created);
    }

    [RelayCommand]
    private async Task LoadTemplateAsync()
    {
        if (SelectedProjectToLoad is null)
        {
            return;
        }

        var project = await _roadmapProjectRepository.GetByIdAsync(SelectedProjectToLoad.Id);
        if (project is null)
        {
            return;
        }

        foreach (var phase in Phases)
        {
            phase.Changed -= OnPhaseChanged;
        }

        Phases.Clear();
        ProjectName = project.ProjectName;
        ProjectDescription = project.Description;

        foreach (var phase in project.Phases)
        {
            var phaseViewModel = new RoadmapPhaseItemViewModel(phase.Title);
            foreach (var task in phase.Tasks)
            {
                phaseViewModel.AddTaskFromLibrary(task.Description);
            }

            AddPhaseItem(phaseViewModel);
        }
    }

    [RelayCommand]
    private void ExportRoadmap()
    {
        var path = _dialogService.ShowSaveFileDialog("ROADMAP.md", "Markdown (*.md)|*.md", _settingsService.DefaultRoadmapExportFolder);
        if (path is null)
        {
            return;
        }

        File.WriteAllText(path, PreviewMarkdown);
    }

    private void AddPhaseItem(RoadmapPhaseItemViewModel phase)
    {
        phase.Changed += OnPhaseChanged;
        Phases.Add(phase);
    }

    private void OnPhaseChanged(object? sender, EventArgs e) => RefreshPreview();

    private RoadmapProjectModel BuildModel()
    {
        return new RoadmapProjectModel
        {
            ProjectName = ProjectName,
            Description = ProjectDescription,
            Phases = Phases.Select((phase, phaseIndex) => new RoadmapPhaseModel
            {
                Title = phase.Title,
                OrderIndex = phaseIndex,
                Tasks = phase.Tasks.Select((task, taskIndex) => new RoadmapTaskModel
                {
                    Description = task.Description,
                    IsChecked = task.IsChecked,
                    OrderIndex = taskIndex
                }).ToList()
            }).ToList()
        };
    }

    private void RefreshPreview()
    {
        var builder = new StringBuilder();
        builder.AppendLine($"# {ProjectName}");
        builder.AppendLine();

        if (!string.IsNullOrWhiteSpace(ProjectDescription))
        {
            builder.AppendLine($"> {ProjectDescription}");
            builder.AppendLine();
        }

        foreach (var phase in Phases)
        {
            builder.AppendLine($"## {phase.Title}");
            builder.AppendLine();

            foreach (var task in phase.Tasks)
            {
                var checkbox = task.IsChecked ? "[x]" : "[ ]";
                builder.AppendLine($"- {checkbox} {task.Description}");
            }

            builder.AppendLine();
        }

        PreviewMarkdown = builder.ToString();
    }
}
