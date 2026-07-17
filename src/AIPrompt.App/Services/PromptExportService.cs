using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using AIPrompt.Core.Models;
using Markdig;

namespace AIPrompt.App.Services;

public partial class PromptExportService
{
    public string BuildMarkdown(SavedPromptModel prompt)
    {
        return $"# {prompt.Title}\n\n{prompt.FinalContent}\n";
    }

    public string BuildPlainText(SavedPromptModel prompt)
    {
        var html = Markdown.ToHtml(prompt.FinalContent);
        var withoutTags = HtmlTagRegex().Replace(html, string.Empty);
        var decoded = WebUtility.HtmlDecode(withoutTags);
        return $"{prompt.Title}\n\n{decoded.Trim()}\n";
    }

    public async Task ExportAsync(SavedPromptModel prompt, string filePath, PromptExportFormat format, CancellationToken cancellationToken = default)
    {
        var content = format == PromptExportFormat.Markdown ? BuildMarkdown(prompt) : BuildPlainText(prompt);
        await File.WriteAllTextAsync(filePath, content, cancellationToken);
    }

    [GeneratedRegex("<[^>]+>")]
    private static partial Regex HtmlTagRegex();
}
