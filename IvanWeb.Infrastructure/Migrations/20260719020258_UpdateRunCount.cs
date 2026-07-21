using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IvanWeb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRunCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FirstRunDate",
                table: "PlayerProfiles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "RunCount",
                table: "PlayerProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstRunDate",
                table: "PlayerProfiles");

            migrationBuilder.DropColumn(
                name: "RunCount",
                table: "PlayerProfiles");
        }
    }
}
