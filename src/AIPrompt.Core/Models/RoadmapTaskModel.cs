namespace AIPrompt.Core.Models;

public class RoadmapTaskModel
{
    public int Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public bool IsChecked { get; set; }

    public int OrderIndex { get; set; }
}
