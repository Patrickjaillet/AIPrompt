namespace AIPrompt.Data.Entities;

public class PromptGenre
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<TermPhrase> TermPhrases { get; set; } = new();
}
