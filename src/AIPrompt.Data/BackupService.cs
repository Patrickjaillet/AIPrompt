using System.Text.Json;
using AIPrompt.Core.Interfaces;
using AIPrompt.Core.Models;
using AIPrompt.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIPrompt.Data;

public class BackupService : IBackupService
{
    private readonly AIPromptDbContext _context;

    public BackupService(AIPromptDbContext context)
    {
        _context = context;
    }

    public async Task<string> ExportToJsonAsync(CancellationToken cancellationToken = default)
    {
        var backup = new DataBackupModel
        {
            ExportedAt = DateTime.UtcNow,
            Categories = await _context.PromptCategories
                .Select(category => new PromptCategoryModel { Id = category.Id, Name = category.Name, Description = category.Description, IconKey = category.IconKey })
                .ToListAsync(cancellationToken),
            Genres = await _context.PromptGenres
                .Select(genre => new PromptGenreModel { Id = genre.Id, Name = genre.Name, Description = genre.Description })
                .ToListAsync(cancellationToken),
            Terms = await _context.TermPhrases
                .Select(term => new TermPhraseModel
                {
                    Id = term.Id,
                    Content = term.Content,
                    CategoryId = term.CategoryId,
                    GenreId = term.GenreId,
                    Tags = term.Tags,
                    Language = term.Language,
                    UsageCount = term.UsageCount
                })
                .ToListAsync(cancellationToken),
            SavedPrompts = await _context.SavedPrompts
                .Select(prompt => new SavedPromptModel
                {
                    Id = prompt.Id,
                    Title = prompt.Title,
                    FinalContent = prompt.FinalContent,
                    SourceTemplateId = prompt.SourceTemplateId,
                    ExportFormat = (PromptExportFormat)prompt.ExportFormat,
                    FilePath = prompt.FilePath
                })
                .ToListAsync(cancellationToken)
        };

        var templates = await _context.PromptTemplates
            .Include(template => template.Blocks)
            .ToListAsync(cancellationToken);
        backup.Templates = templates.Select(template => new PromptTemplateModel
        {
            Id = template.Id,
            Title = template.Title,
            CategoryId = template.CategoryId,
            Blocks = template.Blocks.Select(block => new PromptBlockModel
            {
                Id = block.Id,
                OrderIndex = block.OrderIndex,
                TermPhraseId = block.TermPhraseId,
                FreeText = block.FreeText
            }).ToList()
        }).ToList();

        var projects = await _context.RoadmapProjects
            .Include(project => project.Phases)
            .ThenInclude(phase => phase.Tasks)
            .ToListAsync(cancellationToken);
        backup.RoadmapProjects = projects.Select(project => new RoadmapProjectModel
        {
            Id = project.Id,
            ProjectName = project.ProjectName,
            Description = project.Description,
            Phases = project.Phases.Select(phase => new RoadmapPhaseModel
            {
                Id = phase.Id,
                Title = phase.Title,
                OrderIndex = phase.OrderIndex,
                Tasks = phase.Tasks.Select(task => new RoadmapTaskModel
                {
                    Id = task.Id,
                    Description = task.Description,
                    IsChecked = task.IsChecked,
                    OrderIndex = task.OrderIndex
                }).ToList()
            }).ToList()
        }).ToList();

        return JsonSerializer.Serialize(backup, new JsonSerializerOptions { WriteIndented = true });
    }

    public async Task ImportFromJsonAsync(string json, ImportMode mode, CancellationToken cancellationToken = default)
    {
        var backup = JsonSerializer.Deserialize<DataBackupModel>(json)
            ?? throw new InvalidOperationException("Le fichier de sauvegarde est invalide.");

        if (mode == ImportMode.Overwrite)
        {
            await OverwriteImportAsync(backup, cancellationToken);
        }
        else
        {
            await MergeImportAsync(backup, cancellationToken);
        }
    }

    private async Task OverwriteImportAsync(DataBackupModel backup, CancellationToken cancellationToken)
    {
        _context.SavedPrompts.RemoveRange(_context.SavedPrompts);
        _context.PromptTemplates.RemoveRange(_context.PromptTemplates);
        _context.TermPhrases.RemoveRange(_context.TermPhrases);
        _context.RoadmapProjects.RemoveRange(_context.RoadmapProjects);
        _context.PromptGenres.RemoveRange(_context.PromptGenres);
        _context.PromptCategories.RemoveRange(_context.PromptCategories);
        await _context.SaveChangesAsync(cancellationToken);

        var categoryIdMap = new Dictionary<int, int>();
        foreach (var category in backup.Categories)
        {
            var entity = new PromptCategory { Name = category.Name, Description = category.Description, IconKey = category.IconKey };
            _context.PromptCategories.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            categoryIdMap[category.Id] = entity.Id;
        }

        var genreIdMap = new Dictionary<int, int>();
        foreach (var genre in backup.Genres)
        {
            var entity = new PromptGenre { Name = genre.Name, Description = genre.Description };
            _context.PromptGenres.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            genreIdMap[genre.Id] = entity.Id;
        }

        var termIdMap = new Dictionary<int, int>();
        var now = DateTime.UtcNow;
        foreach (var term in backup.Terms)
        {
            var entity = new TermPhrase
            {
                Content = term.Content,
                CategoryId = categoryIdMap.GetValueOrDefault(term.CategoryId, term.CategoryId),
                GenreId = genreIdMap.GetValueOrDefault(term.GenreId, term.GenreId),
                Tags = term.Tags,
                Language = term.Language,
                UsageCount = term.UsageCount,
                CreatedAt = now,
                UpdatedAt = now
            };
            _context.TermPhrases.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            termIdMap[term.Id] = entity.Id;
        }

        var templateIdMap = new Dictionary<int, int>();
        foreach (var template in backup.Templates)
        {
            var entity = new PromptTemplate
            {
                Title = template.Title,
                CategoryId = categoryIdMap.GetValueOrDefault(template.CategoryId, template.CategoryId),
                CreatedAt = now,
                UpdatedAt = now,
                Blocks = template.Blocks.Select(block => new PromptBlock
                {
                    OrderIndex = block.OrderIndex,
                    TermPhraseId = block.TermPhraseId.HasValue ? termIdMap.GetValueOrDefault(block.TermPhraseId.Value, block.TermPhraseId.Value) : null,
                    FreeText = block.FreeText
                }).ToList()
            };
            _context.PromptTemplates.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            templateIdMap[template.Id] = entity.Id;
        }

        foreach (var prompt in backup.SavedPrompts)
        {
            var entity = new SavedPrompt
            {
                Title = prompt.Title,
                FinalContent = prompt.FinalContent,
                SourceTemplateId = prompt.SourceTemplateId.HasValue ? templateIdMap.GetValueOrDefault(prompt.SourceTemplateId.Value) : null,
                ExportFormat = (ExportFormat)prompt.ExportFormat,
                FilePath = prompt.FilePath,
                CreatedAt = now,
                UpdatedAt = now
            };
            _context.SavedPrompts.Add(entity);
        }

        foreach (var project in backup.RoadmapProjects)
        {
            var entity = new RoadmapProject
            {
                ProjectName = project.ProjectName,
                Description = project.Description,
                CreatedAt = now,
                UpdatedAt = now,
                Phases = project.Phases.Select(phase => new RoadmapPhase
                {
                    Title = phase.Title,
                    OrderIndex = phase.OrderIndex,
                    Tasks = phase.Tasks.Select(task => new RoadmapTask
                    {
                        Description = task.Description,
                        IsChecked = task.IsChecked,
                        OrderIndex = task.OrderIndex
                    }).ToList()
                }).ToList()
            };
            _context.RoadmapProjects.Add(entity);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task MergeImportAsync(DataBackupModel backup, CancellationToken cancellationToken)
    {
        var existingCategories = await _context.PromptCategories.ToListAsync(cancellationToken);
        var categoryIdMap = new Dictionary<int, int>();
        foreach (var category in backup.Categories)
        {
            var match = existingCategories.FirstOrDefault(existing => existing.Name.Equals(category.Name, StringComparison.OrdinalIgnoreCase));
            if (match is not null)
            {
                categoryIdMap[category.Id] = match.Id;
                continue;
            }

            var entity = new PromptCategory { Name = category.Name, Description = category.Description, IconKey = category.IconKey };
            _context.PromptCategories.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            existingCategories.Add(entity);
            categoryIdMap[category.Id] = entity.Id;
        }

        var existingGenres = await _context.PromptGenres.ToListAsync(cancellationToken);
        var genreIdMap = new Dictionary<int, int>();
        foreach (var genre in backup.Genres)
        {
            var match = existingGenres.FirstOrDefault(existing => existing.Name.Equals(genre.Name, StringComparison.OrdinalIgnoreCase));
            if (match is not null)
            {
                genreIdMap[genre.Id] = match.Id;
                continue;
            }

            var entity = new PromptGenre { Name = genre.Name, Description = genre.Description };
            _context.PromptGenres.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            existingGenres.Add(entity);
            genreIdMap[genre.Id] = entity.Id;
        }

        var existingTerms = await _context.TermPhrases.ToListAsync(cancellationToken);
        var termIdMap = new Dictionary<int, int>();
        var now = DateTime.UtcNow;
        foreach (var term in backup.Terms)
        {
            var match = existingTerms.FirstOrDefault(existing => existing.Content.Equals(term.Content, StringComparison.OrdinalIgnoreCase));
            if (match is not null)
            {
                termIdMap[term.Id] = match.Id;
                continue;
            }

            var entity = new TermPhrase
            {
                Content = term.Content,
                CategoryId = categoryIdMap.GetValueOrDefault(term.CategoryId, term.CategoryId),
                GenreId = genreIdMap.GetValueOrDefault(term.GenreId, term.GenreId),
                Tags = term.Tags,
                Language = term.Language,
                UsageCount = term.UsageCount,
                CreatedAt = now,
                UpdatedAt = now
            };
            _context.TermPhrases.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            existingTerms.Add(entity);
            termIdMap[term.Id] = entity.Id;
        }

        var existingTemplates = await _context.PromptTemplates.ToListAsync(cancellationToken);
        foreach (var template in backup.Templates)
        {
            if (existingTemplates.Any(existing => existing.Title.Equals(template.Title, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            var entity = new PromptTemplate
            {
                Title = template.Title,
                CategoryId = categoryIdMap.GetValueOrDefault(template.CategoryId, template.CategoryId),
                CreatedAt = now,
                UpdatedAt = now,
                Blocks = template.Blocks.Select(block => new PromptBlock
                {
                    OrderIndex = block.OrderIndex,
                    TermPhraseId = block.TermPhraseId.HasValue ? termIdMap.GetValueOrDefault(block.TermPhraseId.Value, block.TermPhraseId.Value) : null,
                    FreeText = block.FreeText
                }).ToList()
            };
            _context.PromptTemplates.Add(entity);
        }

        var existingPrompts = await _context.SavedPrompts.ToListAsync(cancellationToken);
        foreach (var prompt in backup.SavedPrompts)
        {
            if (existingPrompts.Any(existing => existing.Title.Equals(prompt.Title, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            _context.SavedPrompts.Add(new SavedPrompt
            {
                Title = prompt.Title,
                FinalContent = prompt.FinalContent,
                ExportFormat = (ExportFormat)prompt.ExportFormat,
                FilePath = prompt.FilePath,
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        var existingProjects = await _context.RoadmapProjects.ToListAsync(cancellationToken);
        foreach (var project in backup.RoadmapProjects)
        {
            if (existingProjects.Any(existing => existing.ProjectName.Equals(project.ProjectName, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            _context.RoadmapProjects.Add(new RoadmapProject
            {
                ProjectName = project.ProjectName,
                Description = project.Description,
                CreatedAt = now,
                UpdatedAt = now,
                Phases = project.Phases.Select(phase => new RoadmapPhase
                {
                    Title = phase.Title,
                    OrderIndex = phase.OrderIndex,
                    Tasks = phase.Tasks.Select(task => new RoadmapTask
                    {
                        Description = task.Description,
                        IsChecked = task.IsChecked,
                        OrderIndex = task.OrderIndex
                    }).ToList()
                }).ToList()
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
