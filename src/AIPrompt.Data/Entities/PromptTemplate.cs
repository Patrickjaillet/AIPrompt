namespace AIPrompt.Data.Entities;

public class PromptTemplate
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public PromptCategory Category { get; set; } = null!;

    public List<PromptBlock> Blocks { get; set; } = new();

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
