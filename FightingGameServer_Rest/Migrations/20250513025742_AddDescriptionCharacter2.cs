using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FightingGameServer_Rest.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionCharacter2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "character",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "character");
        }
    }
}
