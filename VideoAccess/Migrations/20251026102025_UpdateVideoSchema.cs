using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVideoSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UploadedAt",
                table: "Videos",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "Videos",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProcessingStatus",
                table: "Videos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Videos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Videos_ProcessingStatus",
                table: "Videos",
                column: "ProcessingStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Videos_ProcessingStatus",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "ProcessingStatus",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Videos");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Videos",
                newName: "UploadedAt");
        }
    }
}
