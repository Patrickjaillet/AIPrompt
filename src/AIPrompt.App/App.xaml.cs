using System.IO;
using System.Windows;
using AIPrompt.App.Services;
using AIPrompt.App.ViewModels;
using AIPrompt.Core.Interfaces;
using AIPrompt.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace AIPrompt.App;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

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

        var services = new ServiceCollection();
        ConfigureServices(services, databasePath);
        _serviceProvider = services.BuildServiceProvider();

        var databaseInitializerService = _serviceProvider.GetRequiredService<IDatabaseInitializerService>();
        await databaseInitializerService.InitializeAsync();

        Log.Information("Application started and database initialized at {DatabasePath}", databasePath);

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        Log.CloseAndFlush();
        base.OnExit(e);
    }

    private static void ConfigureServices(IServiceCollection services, string databasePath)
    {
        services.AddDbContext<AIPromptDbContext>(options => options.UseSqlite($"Data Source={databasePath}"));

        services.AddScoped<IDatabaseInitializerService, DatabaseInitializerService>();
        services.AddScoped<ITermPhraseRepository, TermPhraseRepository>();
        services.AddScoped<IPromptCategoryRepository, PromptCategoryRepository>();
        services.AddScoped<IPromptGenreRepository, PromptGenreRepository>();
        services.AddScoped<IPromptTemplateRepository, PromptTemplateRepository>();
        services.AddScoped<ISavedPromptRepository, SavedPromptRepository>();

        services.AddSingleton<ThemeService>();
        services.AddSingleton<IDialogService, DialogService>();

        services.AddSingleton<DashboardViewModel>();
        services.AddSingleton<TermLibraryViewModel>();
        services.AddSingleton<PromptBuilderViewModel>();
        services.AddSingleton<SavedPromptsViewModel>();
        services.AddSingleton<RoadmapGeneratorViewModel>();
        services.AddSingleton<ImportExportViewModel>();
        services.AddSingleton<SettingsViewModel>();
        services.AddSingleton<AboutViewModel>();
        services.AddSingleton<MainViewModel>();

        services.AddSingleton<MainWindow>();
    }
}
