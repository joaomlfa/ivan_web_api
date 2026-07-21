using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IvanWeb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class store : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "StoreItems");

            migrationBuilder.RenameColumn(
                name: "SendsWhatsApp",
                table: "StoreItems",
                newName: "SendWhatsApp");

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "StoreItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "StoreItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icon",
                table: "StoreItems");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "StoreItems");

            migrationBuilder.RenameColumn(
                name: "SendWhatsApp",
                table: "StoreItems",
                newName: "SendsWhatsApp");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "StoreItems",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
