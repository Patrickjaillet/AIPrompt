using AIPrompt.App.Services;
using AIPrompt.Core.Models;

namespace AIPrompt.Tests;

public class PromptExportServiceTests
{
    private readonly PromptExportService _service = new();

    [Fact]
    public void BuildMarkdown_IncludesTitleAsHeadingAndContent()
    {
        var prompt = new SavedPromptModel { Title = "Mon titre", FinalContent = "Contenu **important**" };

        var markdown = _service.BuildMarkdown(prompt);

        Assert.Contains("# Mon titre", markdown);
        Assert.Contains("Contenu **important**", markdown);
    }

    [Fact]
    public void BuildPlainText_StripsMarkdownFormatting()
    {
        var prompt = new SavedPromptModel { Title = "Mon titre", FinalContent = "Contenu **important** et *souligné*" };

        var plainText = _service.BuildPlainText(prompt);

        Assert.DoesNotContain("**", plainText);
        Assert.DoesNotContain("<", plainText);
        Assert.Contains("Mon titre", plainText);
        Assert.Contains("important", plainText);
    }
}
