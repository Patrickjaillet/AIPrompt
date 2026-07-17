using System.IO;
using System.Windows;
using AIPrompt.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace AIPrompt.App;

public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var appDataDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "AIPrompt");

        Directory.CreateDirectory(appDataDirectory);

        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(
                Path.Combine(appDataDirectory, "logs", "aiprompt-.log"),
                rollingInterval: RollingInterval.Day)
            .CreateLogger();

        var databasePath = Path.Combine(appDataDirectory, "aiprompt.db");

        var optionsBuilder = new DbContextOptionsBuilder<AIPromptDbContext>();
        optionsBuilder.UseSqlite($"Data Source={databasePath}");

        await using var context = new AIPromptDbContext(optionsBuilder.Options);
        var databaseInitializerService = new DatabaseInitializerService(context);
        await databaseInitializerService.InitializeAsync();

        Log.Information("Application started and database initialized at {DatabasePath}", databasePath);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.CloseAndFlush();
        base.OnExit(e);
    }
}
