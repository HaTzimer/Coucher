using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coacher.Lib.DAL.Migrations
{
    /// <inheritdoc />
    public partial class DropExerciseSummaries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExerciseSummaries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExerciseSummaries",
                columns: table => new
                {
                    ArchiveTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AssignedTaskCountForCurrentUser = table.Column<int>(type: "int", nullable: false),
                    CompletionPercentage = table.Column<double>(type: "float", nullable: false),
                    CurrentUserRole = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DaysUntilStart = table.Column<int>(type: "int", nullable: false),
                    DueSoonTaskCount = table.Column<int>(type: "int", nullable: false),
                    EndDate = table.Column<DateTime>(type: "date", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ManagerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ManagerPosition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagerRank = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OpenTaskCount = table.Column<int>(type: "int", nullable: false),
                    OverdueTaskCount = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TraineeUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrainerUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WeeksUntilStart = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });
        }
    }
}
