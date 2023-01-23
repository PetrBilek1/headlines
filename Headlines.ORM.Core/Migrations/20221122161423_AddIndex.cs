using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Headlines.ORM.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_HEADLINE_CHANGE_UPVOTE_COUNT",
                table: "HEADLINE_CHANGE",
                column: "UPVOTE_COUNT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HEADLINE_CHANGE_UPVOTE_COUNT",
                table: "HEADLINE_CHANGE");
        }
    }
}
