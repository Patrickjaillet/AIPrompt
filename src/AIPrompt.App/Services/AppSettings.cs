namespace AIPrompt.App.Services;

public class AppSettings
{
    public string? DefaultExportFolder { get; set; }

    public string? DefaultRoadmapExportFolder { get; set; }

    public string? DefaultBackupFolder { get; set; }

    public bool AutoBackupEnabled { get; set; }

    public int AutoBackupIntervalMinutes { get; set; } = 30;

    public string ThemeMode { get; set; } = "Light";

    public string AccentColor { get; set; } = "DeepPurple";

    public string Language { get; set; } = "fr";
}
