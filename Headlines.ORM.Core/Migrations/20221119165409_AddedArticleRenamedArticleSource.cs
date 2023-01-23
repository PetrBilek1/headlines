using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Headlines.ORM.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddedArticleRenamedArticleSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HEADLINE_SOURCE");

            migrationBuilder.CreateTable(
                name: "ARTICLE_SOURCE",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NAME = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    RSSURL = table.Column<string>(name: "RSS_URL", type: "nvarchar(512)", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ARTICLE_SOURCE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ARTICLE",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SOURCEID = table.Column<long>(name: "SOURCE_ID", type: "bigint", nullable: false),
                    PUBLISHED = table.Column<DateTime>(type: "datetime2", nullable: true),
                    URLID = table.Column<string>(name: "URL_ID", type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    CURRENTTITLE = table.Column<string>(name: "CURRENT_TITLE", type: "nvarchar(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ARTICLE", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ARTICLE_ARTICLE_SOURCE_SOURCE_ID",
                        column: x => x.SOURCEID,
                        principalTable: "ARTICLE_SOURCE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ARTICLE_SOURCE_ID",
                table: "ARTICLE",
                column: "SOURCE_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ARTICLE");

            migrationBuilder.DropTable(
                name: "ARTICLE_SOURCE");

            migrationBuilder.CreateTable(
                name: "HEADLINE_SOURCE",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NAME = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    URL = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HEADLINE_SOURCE", x => x.ID);
                });
        }
    }
}
