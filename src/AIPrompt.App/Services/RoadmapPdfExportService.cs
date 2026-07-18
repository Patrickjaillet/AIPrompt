using AIPrompt.Core.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AIPrompt.App.Services;

public class RoadmapPdfExportService
{
    public void Export(RoadmapProjectModel project, string filePath)
    {
        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(36);
                page.DefaultTextStyle(style => style.FontSize(11));

                page.Header().Column(header =>
                {
                    header.Item().Text(project.ProjectName).FontSize(20).Bold();
                    if (!string.IsNullOrWhiteSpace(project.Description))
                    {
                        header.Item().PaddingTop(4).Text(project.Description).FontSize(11).Italic();
                    }
                });

                page.Content().PaddingTop(16).Column(column =>
                {
                    column.Spacing(10);

                    foreach (var phase in project.Phases.OrderBy(phase => phase.OrderIndex))
                    {
                        column.Item().Text(phase.Title).FontSize(14).Bold();

                        foreach (var task in phase.Tasks.OrderBy(task => task.OrderIndex))
                        {
                            var marker = task.IsChecked ? "[x]" : "[ ]";
                            column.Item().PaddingLeft(12).Text($"{marker} {task.Description}");
                        }
                    }
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.CurrentPageNumber();
                    text.Span(" / ");
                    text.TotalPages();
                });
            });
        }).GeneratePdf(filePath);
    }
}
