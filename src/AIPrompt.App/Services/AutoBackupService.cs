using System.IO;
using AIPrompt.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace AIPrompt.App.Services;

public class AutoBackupService : IAutoBackupService, IDisposable
{
    private const int MaxBackupFiles = 10;

    private readonly IServiceProvider _serviceProvider;
    private readonly ISettingsService _settingsService;
    private Timer? _timer;

    public AutoBackupService(IServiceProvider serviceProvider, ISettingsService settingsService)
    {
        _serviceProvider = serviceProvider;
        _settingsService = settingsService;
    }

    public void Start() => Reconfigure();

    public void Reconfigure()
    {
        _timer?.Dispose();
        _timer = null;

        if (!_settingsService.AutoBackupEnabled)
        {
            return;
        }

        var interval = TimeSpan.FromMinutes(Math.Max(1, _settingsService.AutoBackupIntervalMinutes));
        _timer = new Timer(_ => RunBackup(), null, interval, interval);
    }

    private void RunBackup()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var backupService = scope.ServiceProvider.GetRequiredService<IBackupService>();
            var json = backupService.ExportToJsonAsync().GetAwaiter().GetResult();

            var fileName = $"backup-{DateTime.Now:yyyyMMdd-HHmmss}.json";
            var filePath = Path.Combine(_settingsService.BackupsFolder, fileName);
            File.WriteAllText(filePath, json);

            PruneOldBackups();

            Log.Information("Automatic backup created at {FilePath}", filePath);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Automatic backup failed");
        }
    }

    private void PruneOldBackups()
    {
        var files = Directory.GetFiles(_settingsService.BackupsFolder, "*.json")
            .OrderByDescending(file => file)
            .ToList();

        foreach (var file in files.Skip(MaxBackupFiles))
        {
            File.Delete(file);
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
