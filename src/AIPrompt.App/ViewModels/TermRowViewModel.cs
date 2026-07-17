using AIPrompt.Core.Models;

namespace AIPrompt.App.ViewModels;

public class TermRowViewModel
{
    public required TermPhraseModel Term { get; init; }

    public required string CategoryName { get; init; }

    public required string GenreName { get; init; }

    public int Id => Term.Id;

    public string Content => Term.Content;

    public string Tags => Term.Tags;

    public string Language => Term.Language;

    public int UsageCount => Term.UsageCount;
}
