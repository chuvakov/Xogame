using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace XOgame.Core.Migrations
{
    public partial class AddSettingsSounds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SettingsSounds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    IsEnabledWin = table.Column<bool>(type: "boolean", nullable: false),
                    IsEnabledLose = table.Column<bool>(type: "boolean", nullable: false),
                    IsEnabledStep = table.Column<bool>(type: "boolean", nullable: false),
                    IsEnabledDraw = table.Column<bool>(type: "boolean", nullable: false),
                    IsEnabledStartGame = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettingsSounds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SettingsSounds_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SettingsSounds_UserId",
                table: "SettingsSounds",
                column: "UserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SettingsSounds");
        }
    }
}
