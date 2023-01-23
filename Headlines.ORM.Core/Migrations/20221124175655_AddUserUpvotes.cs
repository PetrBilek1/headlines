using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Headlines.ORM.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddUserUpvotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "USER_UPVOTES",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    USERTOKEN = table.Column<string>(name: "USER_TOKEN", type: "nvarchar(128)", maxLength: 128, nullable: false),
                    JSON = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_UPVOTES", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_USER_UPVOTES_USER_TOKEN",
                table: "USER_UPVOTES",
                column: "USER_TOKEN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "USER_UPVOTES");
        }
    }
}
