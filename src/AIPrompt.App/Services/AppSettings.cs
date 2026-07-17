namespace AIPrompt.App.Services;

public class AppSettings
{
    public string? DefaultExportFolder { get; set; }

    public bool AutoBackupEnabled { get; set; }

    public int AutoBackupIntervalMinutes { get; set; } = 30;
}
