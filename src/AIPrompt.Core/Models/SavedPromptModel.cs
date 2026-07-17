namespace AIPrompt.Core.Models;

public enum PromptExportFormat
{
    Markdown,
    PlainText
}

public class SavedPromptModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string FinalContent { get; set; } = string.Empty;

    public int? SourceTemplateId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public PromptExportFormat ExportFormat { get; set; }

    public string? FilePath { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
