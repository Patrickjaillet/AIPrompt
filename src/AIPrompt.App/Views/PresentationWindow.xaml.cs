using System.Windows;
using System.Windows.Input;
using Markdig;

namespace AIPrompt.App.Views;

public partial class PresentationWindow : Window
{
    public PresentationWindow(string title, string markdownContent)
    {
        InitializeComponent();
        Title = title;

        var body = Markdown.ToHtml(markdownContent);
        const string style = """
            body { background-color: #1E1E1E; color: #E8E8E8; font-family: Segoe UI, sans-serif; font-size: 22px; line-height: 1.6; padding: 64px 96px; }
            h1, h2, h3 { color: #BB86FC; }
            code { background-color: #2D2D2D; padding: 2px 6px; border-radius: 4px; font-family: Consolas, monospace; }
            pre code { display: block; padding: 16px; overflow-x: auto; }
            a { color: #82AAFF; }
            """;
        var html = $"<html><head><meta charset=\"utf-8\" /><style>{style}</style></head><body>{body}</body></html>";

        Browser.NavigateToString(html);
    }

    private void OnCloseClick(object sender, RoutedEventArgs e) => Close();

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Close();
        }
    }
}
