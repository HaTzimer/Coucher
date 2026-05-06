using Coacher.Lib.DAL;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coacher.Lib.DAL.Migrations
{
    [DbContext(typeof(CoacherDbContext))]
    [Migration("20260506120000_RenameTaskTemplateDefaultLeadTimeToTimeSpan")]
    public partial class RenameTaskTemplateDefaultLeadTimeToTimeSpan : Migration
    {
        private const long TicksPerWeek = 6048000000000L;

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DefaultTimeBeforeExerciseToStartTicks",
                table: "TaskTemplates",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.Sql($@"
                UPDATE [TaskTemplates]
                SET [DefaultTimeBeforeExerciseToStartTicks] = CAST([DefaultWeeksBeforeExerciseStart] AS bigint) * {TicksPerWeek}");

            migrationBuilder.DropColumn(
                name: "DefaultWeeksBeforeExerciseStart",
                table: "TaskTemplates");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DefaultWeeksBeforeExerciseStart",
                table: "TaskTemplates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql($@"
                UPDATE [TaskTemplates]
                SET [DefaultWeeksBeforeExerciseStart] = CAST([DefaultTimeBeforeExerciseToStartTicks] / {TicksPerWeek} AS int)");

            migrationBuilder.DropColumn(
                name: "DefaultTimeBeforeExerciseToStartTicks",
                table: "TaskTemplates");
        }
    }
}
