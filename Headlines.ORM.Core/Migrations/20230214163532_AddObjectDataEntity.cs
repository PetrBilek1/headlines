using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Headlines.ORM.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddObjectDataEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SCRAPER_TYPE",
                table: "ARTICLE_SOURCE",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OBJECT_DATA",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BUCKET = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    KEY = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    CONTENTTYPE = table.Column<string>(name: "CONTENT_TYPE", type: "nvarchar(max)", nullable: true),
                    CREATED = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CHANGED = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OBJECT_DATA", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ArticleObjectData",
                columns: table => new
                {
                    ArticleId = table.Column<long>(type: "bigint", nullable: false),
                    DetailsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleObjectData", x => new { x.ArticleId, x.DetailsId });
                    table.ForeignKey(
                        name: "FK_ArticleObjectData_ARTICLE_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "ARTICLE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticleObjectData_OBJECT_DATA_DetailsId",
                        column: x => x.DetailsId,
                        principalTable: "OBJECT_DATA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleObjectData_DetailsId",
                table: "ArticleObjectData",
                column: "DetailsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleObjectData");

            migrationBuilder.DropTable(
                name: "OBJECT_DATA");

            migrationBuilder.DropColumn(
                name: "SCRAPER_TYPE",
                table: "ARTICLE_SOURCE");
        }
    }
}
