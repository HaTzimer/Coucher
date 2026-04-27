using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coucher.Lib.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClosedListItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClosedListItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseSummaries",
                columns: table => new
                {
                    ExerciseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    EndDate = table.Column<DateTime>(type: "date", nullable: false),
                    TraineeUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrainerUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedTaskCountForCurrentUser = table.Column<int>(type: "int", nullable: false),
                    OpenTaskCount = table.Column<int>(type: "int", nullable: false),
                    OverdueTaskCount = table.Column<int>(type: "int", nullable: false),
                    DueSoonTaskCount = table.Column<int>(type: "int", nullable: false),
                    CompletionPercentage = table.Column<double>(type: "float", nullable: false),
                    WeeksUntilStart = table.Column<int>(type: "int", nullable: false),
                    DaysUntilStart = table.Column<int>(type: "int", nullable: false),
                    ManagerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ManagerPosition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagerRank = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentUserRole = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArchiveTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "TaskTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SeriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SerialNumber = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    DefaultWeeksBeforeExerciseStart = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskTemplates_ClosedListItems_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ClosedListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskTemplates_ClosedListItems_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "ClosedListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskTemplates_TaskTemplates_ParentId",
                        column: x => x.ParentId,
                        principalTable: "TaskTemplates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    EchelonId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Units_ClosedListItems_EchelonId",
                        column: x => x.EchelonId,
                        principalTable: "ClosedListItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaskTemplateDependencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DependsOnId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTemplateDependencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskTemplateDependencies_TaskTemplates_DependsOnId",
                        column: x => x.DependsOnId,
                        principalTable: "TaskTemplates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskTemplateDependencies_TaskTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "TaskTemplates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaskTemplateInfluencers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InfluencerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTemplateInfluencers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskTemplateInfluencers_ClosedListItems_InfluencerId",
                        column: x => x.InfluencerId,
                        principalTable: "ClosedListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskTemplateInfluencers_TaskTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "TaskTemplates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    EndDate = table.Column<DateTime>(type: "date", nullable: false),
                    TraineeUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TrainerUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CompressionFactor = table.Column<double>(type: "float", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ArchiveTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ArchivedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exercises_ClosedListItems_StatusId",
                        column: x => x.StatusId,
                        principalTable: "ClosedListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Exercises_Units_TraineeUnitId",
                        column: x => x.TraineeUnitId,
                        principalTable: "Units",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Exercises_Units_TrainerUnitId",
                        column: x => x.TrainerUnitId,
                        principalTable: "Units",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdentityNumber = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PersonalNumber = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ExternalId = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Rank = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Position = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CivilianEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    MilitaryEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ProfileImageUrl = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    LastLoginTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProfiles_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ExerciseInfluencers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InfluencerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseInfluencers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseInfluencers_ClosedListItems_InfluencerId",
                        column: x => x.InfluencerId,
                        principalTable: "ClosedListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExerciseInfluencers_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ExerciseSections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseSections_ClosedListItems_SectionId",
                        column: x => x.SectionId,
                        principalTable: "ClosedListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExerciseSections_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ExerciseTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SeriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SerialNumber = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastStatusUpdaterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastStatusUpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HasChildren = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseTasks_ClosedListItems_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ClosedListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExerciseTasks_ClosedListItems_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "ClosedListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExerciseTasks_ClosedListItems_StatusId",
                        column: x => x.StatusId,
                        principalTable: "ClosedListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExerciseTasks_ExerciseTasks_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ExerciseTasks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExerciseTasks_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExerciseTasks_TaskTemplates_SourceId",
                        column: x => x.SourceId,
                        principalTable: "TaskTemplates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ExerciseUnitContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ContactType = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProfileImageUrl = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseUnitContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseUnitContacts_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ExerciseParticipants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Role = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseParticipants_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExerciseParticipants_UserProfiles_UserId",
                        column: x => x.UserId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExerciseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserNotifications_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserNotifications_UserProfiles_UserId",
                        column: x => x.UserId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Role = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AssignedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssignedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_UserProfiles_UserId",
                        column: x => x.UserId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ExerciseTaskResponsibleUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseTaskResponsibleUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseTaskResponsibleUsers_ExerciseTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "ExerciseTasks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExerciseTaskResponsibleUsers_UserProfiles_UserId",
                        column: x => x.UserId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaskDependencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DependsOnId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskDependencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskDependencies_ExerciseTasks_DependsOnId",
                        column: x => x.DependsOnId,
                        principalTable: "ExerciseTasks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskDependencies_ExerciseTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "ExerciseTasks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClosedListItems_Key_DisplayOrder",
                table: "ClosedListItems",
                columns: new[] { "Key", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseInfluencers_ExerciseId_InfluencerId",
                table: "ExerciseInfluencers",
                columns: new[] { "ExerciseId", "InfluencerId" },
                unique: true,
                filter: "[ExerciseId] IS NOT NULL AND [InfluencerId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseInfluencers_InfluencerId",
                table: "ExerciseInfluencers",
                column: "InfluencerId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseParticipants_ExerciseId",
                table: "ExerciseParticipants",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseParticipants_UserId",
                table: "ExerciseParticipants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_EndDate",
                table: "Exercises",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_StartDate",
                table: "Exercises",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_StatusId",
                table: "Exercises",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_TraineeUnitId",
                table: "Exercises",
                column: "TraineeUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_TrainerUnitId",
                table: "Exercises",
                column: "TrainerUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseSections_ExerciseId_SectionId",
                table: "ExerciseSections",
                columns: new[] { "ExerciseId", "SectionId" },
                unique: true,
                filter: "[ExerciseId] IS NOT NULL AND [SectionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseSections_SectionId",
                table: "ExerciseSections",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseTaskResponsibleUsers_TaskId_UserId",
                table: "ExerciseTaskResponsibleUsers",
                columns: new[] { "TaskId", "UserId" },
                unique: true,
                filter: "[TaskId] IS NOT NULL AND [UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseTaskResponsibleUsers_UserId",
                table: "ExerciseTaskResponsibleUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseTasks_CategoryId",
                table: "ExerciseTasks",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseTasks_ExerciseId",
                table: "ExerciseTasks",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseTasks_ParentId",
                table: "ExerciseTasks",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseTasks_SeriesId",
                table: "ExerciseTasks",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseTasks_SourceId",
                table: "ExerciseTasks",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseTasks_StatusId",
                table: "ExerciseTasks",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseUnitContacts_ExerciseId",
                table: "ExerciseUnitContacts",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseUnitContacts_ExerciseId_ContactType",
                table: "ExerciseUnitContacts",
                columns: new[] { "ExerciseId", "ContactType" });

            migrationBuilder.CreateIndex(
                name: "IX_TaskDependencies_DependsOnId",
                table: "TaskDependencies",
                column: "DependsOnId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskDependencies_TaskId_DependsOnId",
                table: "TaskDependencies",
                columns: new[] { "TaskId", "DependsOnId" },
                unique: true,
                filter: "[TaskId] IS NOT NULL AND [DependsOnId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTemplateDependencies_DependsOnId",
                table: "TaskTemplateDependencies",
                column: "DependsOnId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTemplateDependencies_TemplateId_DependsOnId",
                table: "TaskTemplateDependencies",
                columns: new[] { "TemplateId", "DependsOnId" },
                unique: true,
                filter: "[TemplateId] IS NOT NULL AND [DependsOnId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTemplateInfluencers_InfluencerId",
                table: "TaskTemplateInfluencers",
                column: "InfluencerId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTemplateInfluencers_TemplateId_InfluencerId",
                table: "TaskTemplateInfluencers",
                columns: new[] { "TemplateId", "InfluencerId" },
                unique: true,
                filter: "[TemplateId] IS NOT NULL AND [InfluencerId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTemplates_CategoryId",
                table: "TaskTemplates",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTemplates_ParentId",
                table: "TaskTemplates",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTemplates_SeriesId",
                table: "TaskTemplates",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_Units_EchelonId",
                table: "Units",
                column: "EchelonId");

            migrationBuilder.CreateIndex(
                name: "IX_Units_Name",
                table: "Units",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_ExerciseId",
                table: "UserNotifications",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_TaskId",
                table: "UserNotifications",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_UserId_CreationTime",
                table: "UserNotifications",
                columns: new[] { "UserId", "CreationTime" });

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_UserId_IsRead",
                table: "UserNotifications",
                columns: new[] { "UserId", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_ExternalId",
                table: "UserProfiles",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_IdentityNumber",
                table: "UserProfiles",
                column: "IdentityNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_UnitId",
                table: "UserProfiles",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExerciseInfluencers");

            migrationBuilder.DropTable(
                name: "ExerciseParticipants");

            migrationBuilder.DropTable(
                name: "ExerciseSections");

            migrationBuilder.DropTable(
                name: "ExerciseSummaries");

            migrationBuilder.DropTable(
                name: "ExerciseTaskResponsibleUsers");

            migrationBuilder.DropTable(
                name: "ExerciseUnitContacts");

            migrationBuilder.DropTable(
                name: "TaskDependencies");

            migrationBuilder.DropTable(
                name: "TaskTemplateDependencies");

            migrationBuilder.DropTable(
                name: "TaskTemplateInfluencers");

            migrationBuilder.DropTable(
                name: "UserNotifications");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "ExerciseTasks");

            migrationBuilder.DropTable(
                name: "UserProfiles");

            migrationBuilder.DropTable(
                name: "Exercises");

            migrationBuilder.DropTable(
                name: "TaskTemplates");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropTable(
                name: "ClosedListItems");
        }
    }
}
