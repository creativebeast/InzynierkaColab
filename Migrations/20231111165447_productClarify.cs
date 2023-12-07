using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inzynierka.Migrations
{
    public partial class productClarify : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VATValue",
                table: "Products",
                newName: "TotalValue");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalBruttoValue",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalBruttoValue",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "TotalValue",
                table: "Products",
                newName: "VATValue");
        }
    }
}
