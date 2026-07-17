namespace AIPrompt.Data.Entities;

public enum ExportFormat
{
    Markdown,
    PlainText
}

public class SavedPrompt
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string FinalContent { get; set; } = string.Empty;

    public int? SourceTemplateId { get; set; }

    public PromptTemplate? SourceTemplate { get; set; }

    public ExportFormat ExportFormat { get; set; }

    public string? FilePath { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
