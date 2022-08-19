using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XOgame.Core.Migrations
{
    public partial class AddLoginAndPassword : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Login",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Users",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Login",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Users");
        }
    }
}
