using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coacher.Lib.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserRoleIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UserRoles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UserRoles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
