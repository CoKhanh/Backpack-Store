using Microsoft.EntityFrameworkCore.Migrations;

namespace LapTrinhWeb.Data.Migrations
{
    public partial class DeleteAmountFromProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Products");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Amount",
                table: "Products",
                nullable: false,
                defaultValue: 0);
        }
    }
}
