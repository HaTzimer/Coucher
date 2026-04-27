using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coucher.Lib.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNotificationReadTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReadTime",
                table: "UserNotifications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReadTime",
                table: "UserNotifications",
                type: "datetime2",
                nullable: true);
        }
    }
}
