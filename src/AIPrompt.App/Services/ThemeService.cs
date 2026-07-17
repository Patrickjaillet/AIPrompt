using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace AIPrompt.App.Services;

public class ThemeService
{
    private readonly PaletteHelper _paletteHelper = new();
    private readonly SwatchesProvider _swatchesProvider = new();

    public static IReadOnlyList<string> AvailableAccentColors { get; } =
    [
        "DeepPurple", "Indigo", "Blue", "Teal", "Green", "Orange", "Red", "Pink", "Brown", "BlueGrey"
    ];

    public bool IsDarkTheme => _paletteHelper.GetTheme().GetBaseTheme() == BaseTheme.Dark;

    public void ToggleBaseTheme()
    {
        ApplyThemeMode(IsDarkTheme ? ThemeMode.Light : ThemeMode.Dark);
    }

    public void ApplyThemeMode(ThemeMode mode)
    {
        var baseTheme = mode switch
        {
            ThemeMode.Dark => BaseTheme.Dark,
            ThemeMode.System => Theme.GetSystemTheme() ?? BaseTheme.Light,
            _ => BaseTheme.Light
        };

        var theme = _paletteHelper.GetTheme();
        theme.SetBaseTheme(baseTheme);
        _paletteHelper.SetTheme(theme);
    }

    public void ApplyAccentColor(string accentColorName)
    {
        var swatch = _swatchesProvider.Swatches
            .FirstOrDefault(candidate => string.Equals(candidate.Name, accentColorName, StringComparison.OrdinalIgnoreCase));

        if (swatch is null)
        {
            return;
        }

        var theme = _paletteHelper.GetTheme();
        theme.SetPrimaryColor(swatch.ExemplarHue.Color);
        _paletteHelper.SetTheme(theme);
    }
}
