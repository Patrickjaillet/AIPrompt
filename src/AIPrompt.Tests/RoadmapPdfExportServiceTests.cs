using AIPrompt.App.Services;
using AIPrompt.Core.Models;

namespace AIPrompt.Tests;

public class RoadmapPdfExportServiceTests : IDisposable
{
    private readonly RoadmapPdfExportService _service = new();
    private readonly string _filePath = Path.Combine(Path.GetTempPath(), $"aiprompt-roadmap-{Guid.NewGuid()}.pdf");

    public RoadmapPdfExportServiceTests()
    {
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
    }

    [Fact]
    public void Export_WritesNonEmptyPdfFile()
    {
        var project = new RoadmapProjectModel
        {
            ProjectName = "AIPrompt",
            Description = "Roadmap de test",
            Phases =
            [
                new RoadmapPhaseModel
                {
                    Title = "Phase 0",
                    OrderIndex = 0,
                    Tasks =
                    [
                        new RoadmapTaskModel { Description = "Créer le dépôt", IsChecked = true, OrderIndex = 0 },
                        new RoadmapTaskModel { Description = "Rédiger le README", IsChecked = false, OrderIndex = 1 }
                    ]
                }
            ]
        };

        _service.Export(project, _filePath);

        Assert.True(File.Exists(_filePath));
        Assert.True(new FileInfo(_filePath).Length > 0);

        var header = new byte[5];
        using (var stream = File.OpenRead(_filePath))
        {
            stream.ReadExactly(header);
        }
        Assert.Equal("%PDF-", System.Text.Encoding.ASCII.GetString(header));
    }

    public void Dispose()
    {
        if (File.Exists(_filePath))
        {
            File.Delete(_filePath);
        }
    }
}
