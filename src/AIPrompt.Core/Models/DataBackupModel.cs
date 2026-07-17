namespace AIPrompt.Core.Models;

public class DataBackupModel
{
    public DateTime ExportedAt { get; set; }

    public List<PromptCategoryModel> Categories { get; set; } = [];

    public List<PromptGenreModel> Genres { get; set; } = [];

    public List<TermPhraseModel> Terms { get; set; } = [];

    public List<PromptTemplateModel> Templates { get; set; } = [];

    public List<SavedPromptModel> SavedPrompts { get; set; } = [];

    public List<RoadmapProjectModel> RoadmapProjects { get; set; } = [];
}
