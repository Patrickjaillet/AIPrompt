using MaterialDesignThemes.Wpf;

namespace AIPrompt.App.Services;

public class ThemeService
{
    private readonly PaletteHelper _paletteHelper = new();

    public bool IsDarkTheme => _paletteHelper.GetTheme().GetBaseTheme() == BaseTheme.Dark;

    public void ToggleBaseTheme()
    {
        var theme = _paletteHelper.GetTheme();
        theme.SetBaseTheme(IsDarkTheme ? BaseTheme.Light : BaseTheme.Dark);
        _paletteHelper.SetTheme(theme);
    }

    public void SetPrimaryColor(System.Windows.Media.Color color)
    {
        var theme = _paletteHelper.GetTheme();
        theme.SetPrimaryColor(color);
        _paletteHelper.SetTheme(theme);
    }
}
