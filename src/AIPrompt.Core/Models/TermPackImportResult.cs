namespace AIPrompt.Core.Models;

public class TermPackImportResult
{
    public string PackName { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public int ImportedCount { get; set; }

    public int SkippedCount { get; set; }
}
