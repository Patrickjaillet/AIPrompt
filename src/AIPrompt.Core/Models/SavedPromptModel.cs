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

    public PromptExportFormat ExportFormat { get; set; }

    public string? FilePath { get; set; }
}
