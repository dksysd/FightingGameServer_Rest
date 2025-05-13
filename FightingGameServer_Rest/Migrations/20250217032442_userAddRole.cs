using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FightingGameServer_Rest.Migrations
{
    /// <inheritdoc />
    public partial class userAddRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "role",
                table: "user",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "role",
                table: "user");
        }
    }
}
