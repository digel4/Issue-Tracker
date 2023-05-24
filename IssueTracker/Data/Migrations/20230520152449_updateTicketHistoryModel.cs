using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IssueTracker.Migrations
{
    /// <inheritdoc />
    public partial class updateTicketHistoryModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewValue",
                table: "TicketHistories");

            migrationBuilder.DropColumn(
                name: "OldValue",
                table: "TicketHistories");

            migrationBuilder.DropColumn(
                name: "Property",
                table: "TicketHistories");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Tickets",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Tickets",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AddColumn<string>(
                name: "NewValue",
                table: "TicketHistories",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OldValue",
                table: "TicketHistories",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Property",
                table: "TicketHistories",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
