using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeTrack.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddmoderationfieldinItemTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ModerationStatus",
                table: "tags",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ImageModerationStatus",
                table: "items",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModeratedAt",
                table: "items",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModerationByUserId",
                table: "items",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModerationNote",
                table: "items",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_items_ModerationByUserId",
                table: "items",
                column: "ModerationByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_items_Users_ModerationByUserId",
                table: "items",
                column: "ModerationByUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_items_Users_ModerationByUserId",
                table: "items");

            migrationBuilder.DropIndex(
                name: "IX_items_ModerationByUserId",
                table: "items");

            migrationBuilder.DropColumn(
                name: "ModerationStatus",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "ImageModerationStatus",
                table: "items");

            migrationBuilder.DropColumn(
                name: "ModeratedAt",
                table: "items");

            migrationBuilder.DropColumn(
                name: "ModerationByUserId",
                table: "items");

            migrationBuilder.DropColumn(
                name: "ModerationNote",
                table: "items");
        }
    }
}
