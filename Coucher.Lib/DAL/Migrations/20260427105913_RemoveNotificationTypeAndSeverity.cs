using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coucher.Lib.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNotificationTypeAndSeverity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Severity",
                table: "UserNotifications");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "UserNotifications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Severity",
                table: "UserNotifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "UserNotifications",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
