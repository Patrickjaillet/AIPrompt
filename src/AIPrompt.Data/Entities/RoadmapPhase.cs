namespace AIPrompt.Data.Entities;

public class RoadmapPhase
{
    public int Id { get; set; }

    public int RoadmapProjectId { get; set; }

    public RoadmapProject RoadmapProject { get; set; } = null!;

    public string Title { get; set; } = string.Empty;

    public int OrderIndex { get; set; }

    public List<RoadmapTask> Tasks { get; set; } = new();
}
