using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IssueTracker.Migrations
{
    /// <inheritdoc />
    public partial class updateTicketTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NewProperty",
                table: "Tickets",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewProperty",
                table: "Tickets");
        }
    }
}
