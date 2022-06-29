using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XOgame.Core.Migrations
{
    public partial class AddCellNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColumnNumber",
                table: "GameProgresses");

            migrationBuilder.RenameColumn(
                name: "RowNumber",
                table: "GameProgresses",
                newName: "CellNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CellNumber",
                table: "GameProgresses",
                newName: "RowNumber");

            migrationBuilder.AddColumn<int>(
                name: "ColumnNumber",
                table: "GameProgresses",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
