using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIPrompt.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTermPhraseFts5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE VIRTUAL TABLE TermPhraseFts USING fts5(
                    Content,
                    Tags,
                    content='TermPhrases',
                    content_rowid='Id'
                );
                """);

            migrationBuilder.Sql("INSERT INTO TermPhraseFts(rowid, Content, Tags) SELECT Id, Content, Tags FROM TermPhrases;");

            migrationBuilder.Sql(
                """
                CREATE TRIGGER TermPhrases_ai AFTER INSERT ON TermPhrases BEGIN
                    INSERT INTO TermPhraseFts(rowid, Content, Tags) VALUES (new.Id, new.Content, new.Tags);
                END;
                """);

            migrationBuilder.Sql(
                """
                CREATE TRIGGER TermPhrases_ad AFTER DELETE ON TermPhrases BEGIN
                    INSERT INTO TermPhraseFts(TermPhraseFts, rowid, Content, Tags) VALUES('delete', old.Id, old.Content, old.Tags);
                END;
                """);

            migrationBuilder.Sql(
                """
                CREATE TRIGGER TermPhrases_au AFTER UPDATE ON TermPhrases BEGIN
                    INSERT INTO TermPhraseFts(TermPhraseFts, rowid, Content, Tags) VALUES('delete', old.Id, old.Content, old.Tags);
                    INSERT INTO TermPhraseFts(rowid, Content, Tags) VALUES (new.Id, new.Content, new.Tags);
                END;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS TermPhrases_au;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS TermPhrases_ad;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS TermPhrases_ai;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS TermPhraseFts;");
        }
    }
}
