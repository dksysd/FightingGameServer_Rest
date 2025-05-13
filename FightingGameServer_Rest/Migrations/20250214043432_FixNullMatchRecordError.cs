using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FightingGameServer_Rest.Migrations
{
    /// <inheritdoc />
    public partial class FixNullMatchRecordError : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "character",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    health = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    strength = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    dexterity = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    intelligence = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    move_speed = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    attack_speed = table.Column<float>(type: "float", nullable: false, defaultValue: 0f)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    login_id = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    login_password = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    salt = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "skill",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_passive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    cool_time = table.Column<int>(type: "int", nullable: false),
                    range = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    health_coefficient = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    strength_coefficient = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    dexterity_coefficient = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    intelligence_coefficient = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    move_speed_coefficient = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    attack_speed_coefficient = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    default_command = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    character_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_skill", x => x.id);
                    table.ForeignKey(
                        name: "FK_skill_character_character_id",
                        column: x => x.character_id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "player",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    experience_point = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    match_count = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    user_id = table.Column<int>(type: "int", nullable: false, defaultValue: -1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player", x => x.id);
                    table.ForeignKey(
                        name: "FK_player_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "custom_command",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    command = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    player_id = table.Column<int>(type: "int", nullable: false),
                    character_id = table.Column<int>(type: "int", nullable: false),
                    skill_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_custom_command", x => x.id);
                    table.ForeignKey(
                        name: "FK_custom_command_character_character_id",
                        column: x => x.character_id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_custom_command_player_player_id",
                        column: x => x.player_id,
                        principalTable: "player",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_custom_command_skill_skill_id",
                        column: x => x.skill_id,
                        principalTable: "skill",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "match_record",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    started_at = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ended_at = table.Column<DateTime>(type: "timestamp", nullable: false),
                    winner_player_id = table.Column<int>(type: "int", nullable: true),
                    winner_player_character_id = table.Column<int>(type: "int", nullable: true),
                    loser_player_id = table.Column<int>(type: "int", nullable: true),
                    loser_player_character_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_match_record", x => x.id);
                    table.CheckConstraint("CK_MatchRecord_EndedAtGreaterThanStartedAt", "ended_at > started_at");
                    table.ForeignKey(
                        name: "FK_match_record_character_loser_player_character_id",
                        column: x => x.loser_player_character_id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_match_record_character_winner_player_character_id",
                        column: x => x.winner_player_character_id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_match_record_player_loser_player_id",
                        column: x => x.loser_player_id,
                        principalTable: "player",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_match_record_player_winner_player_id",
                        column: x => x.winner_player_id,
                        principalTable: "player",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_character_name",
                table: "character",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_custom_command_character_id",
                table: "custom_command",
                column: "character_id");

            migrationBuilder.CreateIndex(
                name: "IX_custom_command_player_id_character_id_skill_id",
                table: "custom_command",
                columns: new[] { "player_id", "character_id", "skill_id" });

            migrationBuilder.CreateIndex(
                name: "IX_custom_command_skill_id",
                table: "custom_command",
                column: "skill_id");

            migrationBuilder.CreateIndex(
                name: "IX_match_record_loser_player_character_id",
                table: "match_record",
                column: "loser_player_character_id");

            migrationBuilder.CreateIndex(
                name: "IX_match_record_loser_player_id",
                table: "match_record",
                column: "loser_player_id");

            migrationBuilder.CreateIndex(
                name: "IX_match_record_winner_player_character_id",
                table: "match_record",
                column: "winner_player_character_id");

            migrationBuilder.CreateIndex(
                name: "IX_match_record_winner_player_id",
                table: "match_record",
                column: "winner_player_id");

            migrationBuilder.CreateIndex(
                name: "IX_player_name",
                table: "player",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_player_user_id",
                table: "player",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_skill_character_id",
                table: "skill",
                column: "character_id");

            migrationBuilder.CreateIndex(
                name: "IX_skill_default_command_character_id",
                table: "skill",
                columns: new[] { "default_command", "character_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_skill_name",
                table: "skill",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_login_id",
                table: "user",
                column: "login_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "custom_command");

            migrationBuilder.DropTable(
                name: "match_record");

            migrationBuilder.DropTable(
                name: "skill");

            migrationBuilder.DropTable(
                name: "player");

            migrationBuilder.DropTable(
                name: "character");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
