using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCollection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UniqueConstrainsForLikeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Likes_ItemId",
                table: "Likes");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_ItemId_UserId",
                table: "Likes",
                columns: new[] { "ItemId", "UserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Likes_ItemId_UserId",
                table: "Likes");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_ItemId",
                table: "Likes",
                column: "ItemId");
        }
    }
}
