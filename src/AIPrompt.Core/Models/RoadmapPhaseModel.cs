namespace AIPrompt.Core.Models;

public class RoadmapPhaseModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public int OrderIndex { get; set; }

    public List<RoadmapTaskModel> Tasks { get; set; } = [];
}
