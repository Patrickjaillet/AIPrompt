namespace AIPrompt.Data.Entities;

public class PromptCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string IconKey { get; set; } = string.Empty;

    public List<TermPhrase> TermPhrases { get; set; } = new();

    public List<PromptTemplate> PromptTemplates { get; set; } = new();
}
