using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inzynierka.Migrations
{
    public partial class ProductListTotal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductList",
                table: "Invoices");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalBruttoValue",
                table: "ProductsList",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalNettoValue",
                table: "ProductsList",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPostDiscountValue",
                table: "ProductsList",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalBruttoValue",
                table: "ProductsList");

            migrationBuilder.DropColumn(
                name: "TotalNettoValue",
                table: "ProductsList");

            migrationBuilder.DropColumn(
                name: "TotalPostDiscountValue",
                table: "ProductsList");

            migrationBuilder.AddColumn<int>(
                name: "ProductList",
                table: "Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
