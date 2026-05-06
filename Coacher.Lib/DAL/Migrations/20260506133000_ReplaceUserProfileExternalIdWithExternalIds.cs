using Coacher.Lib.DAL;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coacher.Lib.DAL.Migrations
{
    [DbContext(typeof(CoacherDbContext))]
    [Migration("20260506133000_ReplaceUserProfileExternalIdWithExternalIds")]
    public partial class ReplaceUserProfileExternalIdWithExternalIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExternalIds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExternalIdValue = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    ExternalSourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalIds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalIds_ClosedListItems_ExternalSourceId",
                        column: x => x.ExternalSourceId,
                        principalTable: "ClosedListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExternalIds_UserProfiles_UserId",
                        column: x => x.UserId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExternalIds_ExternalIdValue",
                table: "ExternalIds",
                column: "ExternalIdValue");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalIds_ExternalSourceId",
                table: "ExternalIds",
                column: "ExternalSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalIds_UserId",
                table: "ExternalIds",
                column: "UserId");

            migrationBuilder.Sql(@"
DECLARE @LegacyExternalSourceId uniqueidentifier = NEWID();
DECLARE @Now datetime2 = SYSUTCDATETIME();

IF EXISTS (SELECT 1 FROM [UserProfiles] WHERE [ExternalId] IS NOT NULL)
BEGIN
    INSERT INTO [ClosedListItems] ([Id], [Key], [Value], [Description], [DisplayOrder], [IsArchive], [CreationTime], [LastUpdateTime])
    VALUES (@LegacyExternalSourceId, 'ExternalSource', 'LegacyMigrationSource', 'Auto-created source for migrated UserProfiles.ExternalId values.', NULL, 0, @Now, @Now);

    INSERT INTO [ExternalIds] ([Id], [UserId], [ExternalIdValue], [ExternalSourceId], [CreationTime])
    SELECT NEWID(), [Id], [ExternalId], @LegacyExternalSourceId, @Now
    FROM [UserProfiles]
    WHERE [ExternalId] IS NOT NULL;
END");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_ExternalId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "UserProfiles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "UserProfiles",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.Sql(@"
UPDATE up
SET [ExternalId] = externalIds.[ExternalIdValue]
FROM [UserProfiles] up
OUTER APPLY
(
    SELECT TOP (1) [ExternalIdValue]
    FROM [ExternalIds]
    WHERE [UserId] = up.[Id]
    ORDER BY [CreationTime], [Id]
) externalIds
WHERE externalIds.[ExternalIdValue] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_ExternalId",
                table: "UserProfiles",
                column: "ExternalId");

            migrationBuilder.DropTable(
                name: "ExternalIds");
        }
    }
}
