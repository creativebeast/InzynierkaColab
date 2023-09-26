using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inzynierka.Migrations
{
    public partial class stylingIdAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TextStyling",
                table: "Stylings",
                newName: "TextStylingId");

            migrationBuilder.RenameColumn(
                name: "SpecialStyling",
                table: "Stylings",
                newName: "SpecialStylingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TextStylingId",
                table: "Stylings",
                newName: "TextStyling");

            migrationBuilder.RenameColumn(
                name: "SpecialStylingId",
                table: "Stylings",
                newName: "SpecialStyling");
        }
    }
}
