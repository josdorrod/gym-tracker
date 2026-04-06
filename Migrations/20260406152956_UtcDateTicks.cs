using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymTracker.Migrations
{
    /// <inheritdoc />
    public partial class UtcDateTicks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sets_ExerciseId",
                table: "Sets");

            migrationBuilder.AddColumn<long>(
                name: "CreatedAtUtcDateTicks",
                table: "Sets",
                type: "INTEGER",
                nullable: false,
                computedColumnSql: "([CreatedAtUtcTicks] / 864000000000) * 864000000000",
                stored: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sets_Exercise_Date_SetNumber",
                table: "Sets",
                columns: new[] { "ExerciseId", "CreatedAtUtcDateTicks", "SetNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sets_Exercise_Date_SetNumber",
                table: "Sets");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtcDateTicks",
                table: "Sets");

            migrationBuilder.CreateIndex(
                name: "IX_Sets_ExerciseId",
                table: "Sets",
                column: "ExerciseId");
        }
    }
}
