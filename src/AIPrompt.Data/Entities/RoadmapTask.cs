namespace AIPrompt.Data.Entities;

public class RoadmapTask
{
    public int Id { get; set; }

    public int PhaseId { get; set; }

    public RoadmapPhase Phase { get; set; } = null!;

    public string Description { get; set; } = string.Empty;

    public bool IsChecked { get; set; }

    public int OrderIndex { get; set; }
}
