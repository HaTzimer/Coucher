using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coacher.Lib.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddExerciseCreatedByUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Exercises",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_CreatedByUserId",
                table: "Exercises",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercises_UserProfiles_CreatedByUserId",
                table: "Exercises",
                column: "CreatedByUserId",
                principalTable: "UserProfiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercises_UserProfiles_CreatedByUserId",
                table: "Exercises");

            migrationBuilder.DropIndex(
                name: "IX_Exercises_CreatedByUserId",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Exercises");
        }
    }
}
