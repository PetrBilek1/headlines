using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Headlines.ORM.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddChangedArticleToRecordEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UPDATED",
                table: "ARTICLE",
                newName: "CREATED");

            migrationBuilder.AddColumn<DateTime>(
                name: "CHANGED",
                table: "ARTICLE",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CHANGED",
                table: "ARTICLE");

            migrationBuilder.RenameColumn(
                name: "CREATED",
                table: "ARTICLE",
                newName: "UPDATED");
        }
    }
}
