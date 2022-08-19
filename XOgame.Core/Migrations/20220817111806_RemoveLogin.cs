using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XOgame.Core.Migrations
{
    public partial class RemoveLogin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Login",
                table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Login",
                table: "Users",
                type: "text",
                nullable: true);
        }
    }
}
