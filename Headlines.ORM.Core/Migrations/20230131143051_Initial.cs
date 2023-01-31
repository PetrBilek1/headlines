using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Headlines.ORM.Core.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ARTICLE_SOURCE",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NAME = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    RSSURL = table.Column<string>(name: "RSS_URL", type: "nvarchar(512)", maxLength: 512, nullable: true),
                    URLIDSOURCE = table.Column<int>(name: "URL_ID_SOURCE", type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ARTICLE_SOURCE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "USER_UPVOTES",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    USERTOKEN = table.Column<string>(name: "USER_TOKEN", type: "nvarchar(128)", maxLength: 128, nullable: true),
                    JSON = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_UPVOTES", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ARTICLE",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SOURCEID = table.Column<long>(name: "SOURCE_ID", type: "bigint", nullable: false),
                    PUBLISHED = table.Column<DateTime>(type: "datetime2", nullable: true),
                    URLID = table.Column<string>(name: "URL_ID", type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    CURRENTTITLE = table.Column<string>(name: "CURRENT_TITLE", type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    LINK = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true)
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

            migrationBuilder.CreateTable(
                name: "HEADLINE_CHANGE",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ARTICLEID = table.Column<long>(name: "ARTICLE_ID", type: "bigint", nullable: false),
                    DETECTED = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TITLEBEFORE = table.Column<string>(name: "TITLE_BEFORE", type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    TITLEAFTER = table.Column<string>(name: "TITLE_AFTER", type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    UPVOTECOUNT = table.Column<long>(name: "UPVOTE_COUNT", type: "bigint", nullable: false)
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
                name: "IX_ARTICLE_SOURCE_ID",
                table: "ARTICLE",
                column: "SOURCE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ARTICLE_URL_ID",
                table: "ARTICLE",
                column: "URL_ID");

            migrationBuilder.CreateIndex(
                name: "IX_HEADLINE_CHANGE_ARTICLE_ID",
                table: "HEADLINE_CHANGE",
                column: "ARTICLE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_HEADLINE_CHANGE_DETECTED",
                table: "HEADLINE_CHANGE",
                column: "DETECTED");

            migrationBuilder.CreateIndex(
                name: "IX_HEADLINE_CHANGE_UPVOTE_COUNT",
                table: "HEADLINE_CHANGE",
                column: "UPVOTE_COUNT");

            migrationBuilder.CreateIndex(
                name: "IX_USER_UPVOTES_USER_TOKEN",
                table: "USER_UPVOTES",
                column: "USER_TOKEN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HEADLINE_CHANGE");

            migrationBuilder.DropTable(
                name: "USER_UPVOTES");

            migrationBuilder.DropTable(
                name: "ARTICLE");

            migrationBuilder.DropTable(
                name: "ARTICLE_SOURCE");
        }
    }
}
