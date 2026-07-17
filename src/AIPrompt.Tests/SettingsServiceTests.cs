using AIPrompt.App.Services;

namespace AIPrompt.Tests;

public class SettingsServiceTests : IDisposable
{
    private readonly string _tempDirectory;

    public SettingsServiceTests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDirectory);
    }

    [Fact]
    public void DefaultExportFolder_IsNull_WhenNoSettingsFileExists()
    {
        var service = new SettingsService(_tempDirectory);

        Assert.Null(service.DefaultExportFolder);
    }

    [Fact]
    public async Task SetDefaultExportFolderAsync_PersistsAcrossInstances()
    {
        var service = new SettingsService(_tempDirectory);

        await service.SetDefaultExportFolderAsync(@"C:\Exports");

        var reloaded = new SettingsService(_tempDirectory);
        Assert.Equal(@"C:\Exports", reloaded.DefaultExportFolder);
    }

    [Fact]
    public void ThemeMode_DefaultsToLight()
    {
        var service = new SettingsService(_tempDirectory);

        Assert.Equal(ThemeMode.Light, service.ThemeMode);
    }

    [Fact]
    public async Task SetThemeModeAsync_PersistsAcrossInstances()
    {
        var service = new SettingsService(_tempDirectory);

        await service.SetThemeModeAsync(ThemeMode.Dark);

        var reloaded = new SettingsService(_tempDirectory);
        Assert.Equal(ThemeMode.Dark, reloaded.ThemeMode);
    }

    [Fact]
    public async Task SetAccentColorAsync_PersistsAcrossInstances()
    {
        var service = new SettingsService(_tempDirectory);

        await service.SetAccentColorAsync("Teal");

        var reloaded = new SettingsService(_tempDirectory);
        Assert.Equal("Teal", reloaded.AccentColor);
    }

    [Fact]
    public async Task SetLanguageAsync_PersistsAcrossInstances()
    {
        var service = new SettingsService(_tempDirectory);

        await service.SetLanguageAsync("en");

        var reloaded = new SettingsService(_tempDirectory);
        Assert.Equal("en", reloaded.Language);
    }

    [Fact]
    public void BackupsFolder_DefaultsUnderAppDataDirectory()
    {
        var service = new SettingsService(_tempDirectory);

        Assert.StartsWith(_tempDirectory, service.BackupsFolder);
        Assert.True(Directory.Exists(service.BackupsFolder));
    }

    [Fact]
    public async Task SetDefaultBackupFolderAsync_OverridesBackupsFolder()
    {
        var service = new SettingsService(_tempDirectory);
        var customFolder = Path.Combine(_tempDirectory, "custom-backups");

        await service.SetDefaultBackupFolderAsync(customFolder);

        Assert.Equal(customFolder, service.BackupsFolder);
        Assert.True(Directory.Exists(customFolder));
    }

    public void Dispose()
    {
        Directory.Delete(_tempDirectory, recursive: true);
    }
}
