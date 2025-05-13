using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FightingGameServer_Rest.Migrations
{
    /// <inheritdoc />
    public partial class commandStringToJson2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_skill_default_command_character_id",
                table: "skill");

            migrationBuilder.AlterColumn<string>(
                name: "default_command",
                table: "skill",
                type: "json",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldMaxLength: 20)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "default_command",
                table: "skill",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "json")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_skill_default_command_character_id",
                table: "skill",
                columns: new[] { "default_command", "character_id" },
                unique: true);
        }
    }
}
