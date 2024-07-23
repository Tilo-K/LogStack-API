using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogStack.Migrations
{
    /// <inheritdoc />
    public partial class AddDateParams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Day",
                table: "Logs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "Logs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Logs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Day",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Logs");
        }
    }
}
