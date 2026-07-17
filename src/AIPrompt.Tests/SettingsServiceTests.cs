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

    public void Dispose()
    {
        Directory.Delete(_tempDirectory, recursive: true);
    }
}
