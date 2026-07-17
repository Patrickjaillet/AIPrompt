namespace AIPrompt.Data.Entities;

public class RoadmapProject
{
    public int Id { get; set; }

    public string ProjectName { get; set; } = string.Empty;

    public List<RoadmapPhase> Phases { get; set; } = new();

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
