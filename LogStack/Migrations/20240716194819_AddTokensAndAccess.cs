using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogStack.Migrations
{
    /// <inheritdoc />
    public partial class AddTokensAndAccess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HasAccess",
                columns: table => new
                {
                    UserId = table.Column<byte[]>(type: "bytea", nullable: false),
                    ProjectId = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HasAccess", x => new { x.UserId, x.ProjectId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HasAccess");
        }
    }
}
