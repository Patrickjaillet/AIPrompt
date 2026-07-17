namespace AIPrompt.Data.Entities;

public class TermPhrase
{
    public int Id { get; set; }

    public string Content { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public PromptCategory Category { get; set; } = null!;

    public int GenreId { get; set; }

    public PromptGenre Genre { get; set; } = null!;

    public string Tags { get; set; } = string.Empty;

    public string Language { get; set; } = "FR";

    public int UsageCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
