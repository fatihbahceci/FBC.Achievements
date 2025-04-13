using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBC.Achievements.Migrations
{
    /// <inheritdoc />
    public partial class DisableMultipleGoals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedGoals",
                table: "UserAchievements");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompletedGoals",
                table: "UserAchievements",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
