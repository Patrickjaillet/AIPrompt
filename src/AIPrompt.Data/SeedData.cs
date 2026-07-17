using AIPrompt.Data.Entities;

namespace AIPrompt.Data;

public static class SeedData
{
    public static void Apply(AIPromptDbContext context)
    {
        if (context.PromptCategories.Any())
        {
            return;
        }

        var categories = new[]
        {
            new PromptCategory { Name = "Code", Description = "Prompts pour la génération et la revue de code", IconKey = "CodeTags" },
            new PromptCategory { Name = "Rédaction", Description = "Prompts pour la rédaction de contenu texte", IconKey = "Pencil" },
            new PromptCategory { Name = "Image", Description = "Prompts pour la génération d'images", IconKey = "Image" },
            new PromptCategory { Name = "Marketing", Description = "Prompts pour le marketing et la communication", IconKey = "Bullhorn" },
            new PromptCategory { Name = "Support technique", Description = "Prompts pour le support et le dépannage", IconKey = "LifeBuoy" },
            new PromptCategory { Name = "Roleplay", Description = "Prompts pour les scénarios de jeu de rôle", IconKey = "TheaterMasks" },
            new PromptCategory { Name = "Documentation", Description = "Prompts pour la documentation technique", IconKey = "FileDocument" },
            new PromptCategory { Name = "Roadmap", Description = "Prompts pour la génération de roadmaps projet", IconKey = "MapMarkerPath" }
        };

        var genres = new[]
        {
            new PromptGenre { Name = "Formel", Description = "Registre soutenu et professionnel" },
            new PromptGenre { Name = "Créatif", Description = "Registre inventif et original" },
            new PromptGenre { Name = "Technique", Description = "Registre précis et spécialisé" },
            new PromptGenre { Name = "Concis", Description = "Registre bref et direct" },
            new PromptGenre { Name = "Directif", Description = "Registre impératif orienté action" },
            new PromptGenre { Name = "Narratif", Description = "Registre orienté récit et mise en contexte" }
        };

        context.PromptCategories.AddRange(categories);
        context.PromptGenres.AddRange(genres);
        context.SaveChanges();

        var codeCategory = categories[0];
        var roadmapCategory = categories[7];
        var technicalGenre = genres[2];
        var directiveGenre = genres[4];

        var now = DateTime.UtcNow;
        var phrases = new[]
        {
            new TermPhrase { Content = "Écris une fonction qui", CategoryId = codeCategory.Id, GenreId = directiveGenre.Id, Tags = "code,fonction", Language = "FR", CreatedAt = now, UpdatedAt = now },
            new TermPhrase { Content = "Explique en détail le fonctionnement de", CategoryId = codeCategory.Id, GenreId = technicalGenre.Id, Tags = "code,explication", Language = "FR", CreatedAt = now, UpdatedAt = now },
            new TermPhrase { Content = "Ajoute une phase de tests unitaires pour", CategoryId = roadmapCategory.Id, GenreId = directiveGenre.Id, Tags = "roadmap,tests", Language = "FR", CreatedAt = now, UpdatedAt = now },
            new TermPhrase { Content = "Rédige la documentation utilisateur de", CategoryId = roadmapCategory.Id, GenreId = technicalGenre.Id, Tags = "roadmap,documentation", Language = "FR", CreatedAt = now, UpdatedAt = now }
        };

        context.TermPhrases.AddRange(phrases);
        context.SaveChanges();
    }
}
