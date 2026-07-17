using CommunityToolkit.Mvvm.ComponentModel;

namespace AIPrompt.App.ViewModels;

public partial class PromptBlockItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _content;

    public PromptBlockItemViewModel(int? termPhraseId, string content)
    {
        TermPhraseId = termPhraseId;
        _content = content;
    }

    public int? TermPhraseId { get; }

    public bool IsFreeText => TermPhraseId is null;
}
