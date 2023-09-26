using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inzynierka.Migrations
{
    public partial class stringToXMLStylings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Values",
                table: "TextStyling",
                type: "xml",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Values",
                table: "TableStyling",
                type: "xml",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Values",
                table: "SpecialStyling",
                type: "xml",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Values",
                table: "TextStyling",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "xml");

            migrationBuilder.AlterColumn<string>(
                name: "Values",
                table: "TableStyling",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "xml");

            migrationBuilder.AlterColumn<string>(
                name: "Values",
                table: "SpecialStyling",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "xml");
        }
    }
}
