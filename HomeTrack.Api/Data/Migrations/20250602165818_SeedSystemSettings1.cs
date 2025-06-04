using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeTrack.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedSystemSettings1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "updateAt",
                value: new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 2,
                column: "updateAt",
                value: new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "updateAt",
                value: new DateTime(2025, 6, 2, 16, 54, 2, 306, DateTimeKind.Utc).AddTicks(1797));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 2,
                column: "updateAt",
                value: new DateTime(2025, 6, 2, 16, 54, 2, 306, DateTimeKind.Utc).AddTicks(1803));
        }
    }
}
