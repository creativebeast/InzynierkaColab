using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inzynierka.Migrations
{
    public partial class referenceStling : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReferenceToken",
                table: "TextStyling",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceToken",
                table: "TableStyling",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceToken",
                table: "SpecialStyling",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceToken",
                table: "TextStyling");

            migrationBuilder.DropColumn(
                name: "ReferenceToken",
                table: "TableStyling");

            migrationBuilder.DropColumn(
                name: "ReferenceToken",
                table: "SpecialStyling");
        }
    }
}
