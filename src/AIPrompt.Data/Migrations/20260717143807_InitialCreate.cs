using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIPrompt.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PromptCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    IconKey = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromptCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PromptGenres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromptGenres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoadmapProjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProjectName = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadmapProjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PromptTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromptTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromptTemplates_PromptCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "PromptCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TermPhrases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    GenreId = table.Column<int>(type: "INTEGER", nullable: false),
                    Tags = table.Column<string>(type: "TEXT", nullable: false),
                    Language = table.Column<string>(type: "TEXT", nullable: false),
                    UsageCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TermPhrases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TermPhrases_PromptCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "PromptCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TermPhrases_PromptGenres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "PromptGenres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoadmapPhases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoadmapProjectId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    OrderIndex = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadmapPhases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadmapPhases_RoadmapProjects_RoadmapProjectId",
                        column: x => x.RoadmapProjectId,
                        principalTable: "RoadmapProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SavedPrompts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    FinalContent = table.Column<string>(type: "TEXT", nullable: false),
                    SourceTemplateId = table.Column<int>(type: "INTEGER", nullable: true),
                    ExportFormat = table.Column<int>(type: "INTEGER", nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedPrompts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedPrompts_PromptTemplates_SourceTemplateId",
                        column: x => x.SourceTemplateId,
                        principalTable: "PromptTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PromptBlocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TemplateId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    TermPhraseId = table.Column<int>(type: "INTEGER", nullable: true),
                    FreeText = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromptBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromptBlocks_PromptTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "PromptTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromptBlocks_TermPhrases_TermPhraseId",
                        column: x => x.TermPhraseId,
                        principalTable: "TermPhrases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RoadmapTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PhaseId = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    IsChecked = table.Column<bool>(type: "INTEGER", nullable: false),
                    OrderIndex = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadmapTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadmapTasks_RoadmapPhases_PhaseId",
                        column: x => x.PhaseId,
                        principalTable: "RoadmapPhases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PromptBlocks_TemplateId",
                table: "PromptBlocks",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_PromptBlocks_TermPhraseId",
                table: "PromptBlocks",
                column: "TermPhraseId");

            migrationBuilder.CreateIndex(
                name: "IX_PromptTemplates_CategoryId",
                table: "PromptTemplates",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadmapPhases_RoadmapProjectId",
                table: "RoadmapPhases",
                column: "RoadmapProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadmapTasks_PhaseId",
                table: "RoadmapTasks",
                column: "PhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedPrompts_SourceTemplateId",
                table: "SavedPrompts",
                column: "SourceTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_TermPhrases_CategoryId",
                table: "TermPhrases",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TermPhrases_GenreId",
                table: "TermPhrases",
                column: "GenreId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromptBlocks");

            migrationBuilder.DropTable(
                name: "RoadmapTasks");

            migrationBuilder.DropTable(
                name: "SavedPrompts");

            migrationBuilder.DropTable(
                name: "TermPhrases");

            migrationBuilder.DropTable(
                name: "RoadmapPhases");

            migrationBuilder.DropTable(
                name: "PromptTemplates");

            migrationBuilder.DropTable(
                name: "PromptGenres");

            migrationBuilder.DropTable(
                name: "RoadmapProjects");

            migrationBuilder.DropTable(
                name: "PromptCategories");
        }
    }
}
