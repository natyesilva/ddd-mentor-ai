using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DddMentorAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RichStudySessionAggregate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_StudySessions_UpdatedAt",
                table: "StudySessions",
                column: "UpdatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StudySessions_UpdatedAt",
                table: "StudySessions");
        }
    }
}
