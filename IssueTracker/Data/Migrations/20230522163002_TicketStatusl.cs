using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IssueTracker.Migrations
{
    /// <inheritdoc />
    public partial class TicketStatusl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewProperty",
                table: "Tickets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NewProperty",
                table: "Tickets",
                type: "text",
                nullable: true);
        }
    }
}
