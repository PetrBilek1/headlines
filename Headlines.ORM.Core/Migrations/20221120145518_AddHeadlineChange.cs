using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Headlines.ORM.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddHeadlineChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LINK",
                table: "ARTICLE",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "HEADLINE_CHANGE",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ARTICLEID = table.Column<long>(name: "ARTICLE_ID", type: "bigint", nullable: false),
                    DETECTED = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TITLEBEFORE = table.Column<string>(name: "TITLE_BEFORE", type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    TITLEAFTER = table.Column<string>(name: "TITLE_AFTER", type: "nvarchar(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HEADLINE_CHANGE", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HEADLINE_CHANGE_ARTICLE_ARTICLE_ID",
                        column: x => x.ARTICLEID,
                        principalTable: "ARTICLE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HEADLINE_CHANGE_ARTICLE_ID",
                table: "HEADLINE_CHANGE",
                column: "ARTICLE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_HEADLINE_CHANGE_DETECTED",
                table: "HEADLINE_CHANGE",
                column: "DETECTED");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HEADLINE_CHANGE");

            migrationBuilder.DropColumn(
                name: "LINK",
                table: "ARTICLE");
        }
    }
}
