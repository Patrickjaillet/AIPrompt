namespace AIPrompt.Core.Models;

public class TermPackModel
{
    public string PackName { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public string CategoryDescription { get; set; } = string.Empty;

    public string CategoryIconKey { get; set; } = string.Empty;

    public List<TermPackItemModel> Items { get; set; } = new();
}

public class TermPackItemModel
{
    public string Content { get; set; } = string.Empty;

    public string GenreName { get; set; } = string.Empty;

    public string GenreDescription { get; set; } = string.Empty;

    public string Tags { get; set; } = string.Empty;

    public string Language { get; set; } = "FR";
}
