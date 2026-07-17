using AIPrompt.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIPrompt.Data;

public class AIPromptDbContext : DbContext
{
    public AIPromptDbContext(DbContextOptions<AIPromptDbContext> options)
        : base(options)
    {
    }

    public DbSet<PromptCategory> PromptCategories => Set<PromptCategory>();

    public DbSet<PromptGenre> PromptGenres => Set<PromptGenre>();

    public DbSet<TermPhrase> TermPhrases => Set<TermPhrase>();

    public DbSet<PromptTemplate> PromptTemplates => Set<PromptTemplate>();

    public DbSet<PromptBlock> PromptBlocks => Set<PromptBlock>();

    public DbSet<SavedPrompt> SavedPrompts => Set<SavedPrompt>();

    public DbSet<RoadmapProject> RoadmapProjects => Set<RoadmapProject>();

    public DbSet<RoadmapPhase> RoadmapPhases => Set<RoadmapPhase>();

    public DbSet<RoadmapTask> RoadmapTasks => Set<RoadmapTask>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TermPhrase>()
            .HasOne(t => t.Category)
            .WithMany(c => c.TermPhrases)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TermPhrase>()
            .HasOne(t => t.Genre)
            .WithMany(g => g.TermPhrases)
            .HasForeignKey(t => t.GenreId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PromptTemplate>()
            .HasOne(t => t.Category)
            .WithMany(c => c.PromptTemplates)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PromptBlock>()
            .HasOne(b => b.Template)
            .WithMany(t => t.Blocks)
            .HasForeignKey(b => b.TemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PromptBlock>()
            .HasOne(b => b.TermPhrase)
            .WithMany()
            .HasForeignKey(b => b.TermPhraseId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<SavedPrompt>()
            .HasOne(s => s.SourceTemplate)
            .WithMany()
            .HasForeignKey(s => s.SourceTemplateId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<RoadmapPhase>()
            .HasOne(p => p.RoadmapProject)
            .WithMany(r => r.Phases)
            .HasForeignKey(p => p.RoadmapProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RoadmapTask>()
            .HasOne(t => t.Phase)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.PhaseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
