using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IvanWeb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PlayerProfileId",
                table: "Songs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Daily8BitRuns",
                table: "PlayerProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DailyClockRuns",
                table: "PlayerProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DailyHardRuns",
                table: "PlayerProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastPlayedDate",
                table: "PlayerProfiles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "QuizSongsCorrectAnswers",
                table: "PlayerProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuizSongsIncorrectAnswers",
                table: "PlayerProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuizSongsRuns",
                table: "PlayerProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Songs_PlayerProfileId",
                table: "Songs",
                column: "PlayerProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Songs_PlayerProfiles_PlayerProfileId",
                table: "Songs",
                column: "PlayerProfileId",
                principalTable: "PlayerProfiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Songs_PlayerProfiles_PlayerProfileId",
                table: "Songs");

            migrationBuilder.DropIndex(
                name: "IX_Songs_PlayerProfileId",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "PlayerProfileId",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "Daily8BitRuns",
                table: "PlayerProfiles");

            migrationBuilder.DropColumn(
                name: "DailyClockRuns",
                table: "PlayerProfiles");

            migrationBuilder.DropColumn(
                name: "DailyHardRuns",
                table: "PlayerProfiles");

            migrationBuilder.DropColumn(
                name: "LastPlayedDate",
                table: "PlayerProfiles");

            migrationBuilder.DropColumn(
                name: "QuizSongsCorrectAnswers",
                table: "PlayerProfiles");

            migrationBuilder.DropColumn(
                name: "QuizSongsIncorrectAnswers",
                table: "PlayerProfiles");

            migrationBuilder.DropColumn(
                name: "QuizSongsRuns",
                table: "PlayerProfiles");
        }
    }
}
