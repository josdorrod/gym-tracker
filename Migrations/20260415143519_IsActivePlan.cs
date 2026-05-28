using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymTracker.Migrations
{
    /// <inheritdoc />
    public partial class IsActivePlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercises_Plans_PlanId",
                table: "Exercises");

            migrationBuilder.DropIndex(
                name: "IX_Exercises_PlanId",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "PlanId",
                table: "Exercises");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Plans",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Plans");

            migrationBuilder.AddColumn<int>(
                name: "PlanId",
                table: "Exercises",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_PlanId",
                table: "Exercises",
                column: "PlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercises_Plans_PlanId",
                table: "Exercises",
                column: "PlanId",
                principalTable: "Plans",
                principalColumn: "Id");
        }
    }
}
