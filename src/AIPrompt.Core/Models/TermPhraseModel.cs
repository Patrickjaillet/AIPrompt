namespace AIPrompt.Core.Models;

public class TermPhraseModel
{
    public int Id { get; set; }

    public string Content { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public int GenreId { get; set; }

    public string Tags { get; set; } = string.Empty;

    public string Language { get; set; } = "FR";

    public int UsageCount { get; set; }
}
