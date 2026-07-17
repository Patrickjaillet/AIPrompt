namespace AIPrompt.Core.Models;

public class RoadmapProjectModel
{
    public int Id { get; set; }

    public string ProjectName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<RoadmapPhaseModel> Phases { get; set; } = [];
}
