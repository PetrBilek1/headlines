using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Headlines.ORM.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddNameToHeadlineSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NAME",
                table: "HEADLINE_SOURCE",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NAME",
                table: "HEADLINE_SOURCE");
        }
    }
}
