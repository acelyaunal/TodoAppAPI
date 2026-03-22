using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoAppAPI.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTodoOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                INSERT INTO Users (Email, PasswordHash, CreatedAt)
                SELECT 'legacy.todo.owner@local.invalid', 'MIGRATED_LEGACY_ACCOUNT_DISABLED', '2026-03-22 00:00:00'
                WHERE NOT EXISTS (
                    SELECT 1 FROM Users WHERE Email = 'legacy.todo.owner@local.invalid'
                );
                """);

            migrationBuilder.Sql("""
                CREATE TABLE "__EFMigrations_Todos_Temp" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_Todos" PRIMARY KEY AUTOINCREMENT,
                    "Title" TEXT NOT NULL,
                    "Description" TEXT NOT NULL,
                    "IsCompleted" INTEGER NOT NULL,
                    "CreatedAt" TEXT NOT NULL,
                    "UserId" INTEGER NOT NULL,
                    CONSTRAINT "FK_Todos_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
                );
                """);

            migrationBuilder.Sql("""
                INSERT INTO "__EFMigrations_Todos_Temp" ("Id", "Title", "Description", "IsCompleted", "CreatedAt", "UserId")
                SELECT
                    "Id",
                    "Title",
                    "Description",
                    "IsCompleted",
                    "CreatedAt",
                    (SELECT "Id" FROM "Users" WHERE "Email" = 'legacy.todo.owner@local.invalid' LIMIT 1)
                FROM "Todos";
                """);

            migrationBuilder.DropTable(
                name: "Todos");

            migrationBuilder.RenameTable(
                name: "__EFMigrations_Todos_Temp",
                newName: "Todos");

            migrationBuilder.CreateIndex(
                name: "IX_Todos_CreatedAt",
                table: "Todos",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Todos_UserId_CreatedAt",
                table: "Todos",
                columns: new[] { "UserId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Todos_UserId_CreatedAt",
                table: "Todos");

            migrationBuilder.DropIndex(
                name: "IX_Todos_CreatedAt",
                table: "Todos");

            migrationBuilder.Sql("""
                CREATE TABLE "__EFMigrations_Todos_Temp" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_Todos" PRIMARY KEY AUTOINCREMENT,
                    "Title" TEXT NOT NULL,
                    "Description" TEXT NOT NULL,
                    "IsCompleted" INTEGER NOT NULL,
                    "CreatedAt" TEXT NOT NULL
                );
                """);

            migrationBuilder.Sql("""
                INSERT INTO "__EFMigrations_Todos_Temp" ("Id", "Title", "Description", "IsCompleted", "CreatedAt")
                SELECT "Id", "Title", "Description", "IsCompleted", "CreatedAt"
                FROM "Todos";
                """);

            migrationBuilder.DropTable(
                name: "Todos");

            migrationBuilder.RenameTable(
                name: "__EFMigrations_Todos_Temp",
                newName: "Todos");

            migrationBuilder.CreateIndex(
                name: "IX_Todos_CreatedAt",
                table: "Todos",
                column: "CreatedAt");

            migrationBuilder.Sql("""
                DELETE FROM "Users"
                WHERE "Email" = 'legacy.todo.owner@local.invalid';
                """);
        }
    }
}
