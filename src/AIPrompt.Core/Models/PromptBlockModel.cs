namespace AIPrompt.Core.Models;

public class PromptBlockModel
{
    public int Id { get; set; }

    public int OrderIndex { get; set; }

    public int? TermPhraseId { get; set; }

    public string? FreeText { get; set; }

    public string Content { get; set; } = string.Empty;
}
