namespace AIPrompt.Data.Entities;

public class PromptBlock
{
    public int Id { get; set; }

    public int TemplateId { get; set; }

    public PromptTemplate Template { get; set; } = null!;

    public int OrderIndex { get; set; }

    public int? TermPhraseId { get; set; }

    public TermPhrase? TermPhrase { get; set; }

    public string? FreeText { get; set; }
}
