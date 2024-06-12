using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCollection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class JiraTicketFieldsTwo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JiraAccountId",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssigneeId",
                table: "Tickets",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JiraAccountId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AssigneeId",
                table: "Tickets");
        }
    }
}
