using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Headlines.ORM.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddScrapeJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SCRAPE_JOB",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ARTICLEID = table.Column<long>(name: "ARTICLE_ID", type: "bigint", nullable: false),
                    PRIORITY = table.Column<int>(type: "int", nullable: false),
                    CREATED = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CHANGED = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCRAPE_JOB", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SCRAPE_JOB_ARTICLE_ARTICLE_ID",
                        column: x => x.ARTICLEID,
                        principalTable: "ARTICLE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SCRAPE_JOB_ARTICLE_ID",
                table: "SCRAPE_JOB",
                column: "ARTICLE_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SCRAPE_JOB");
        }
    }
}
