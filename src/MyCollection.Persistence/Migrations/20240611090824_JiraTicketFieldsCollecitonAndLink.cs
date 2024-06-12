using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCollection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class JiraTicketFieldsCollecitonAndLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Collection",
                table: "Tickets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Link",
                table: "Tickets",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Collection",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "Link",
                table: "Tickets");
        }
    }
}
