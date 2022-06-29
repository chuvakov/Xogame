using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XOgame.Core.Migrations
{
    public partial class AddUserTurn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserTurnId",
                table: "Games",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Games_UserTurnId",
                table: "Games",
                column: "UserTurnId");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Users_UserTurnId",
                table: "Games",
                column: "UserTurnId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Users_UserTurnId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_UserTurnId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "UserTurnId",
                table: "Games");
        }
    }
}
