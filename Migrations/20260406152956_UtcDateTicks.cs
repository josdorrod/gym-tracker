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

            if (migrationBuilder.ActiveProvider == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                migrationBuilder.Sql("""
                    CREATE TABLE "Sets_new" (
                        "Id" INTEGER NOT NULL CONSTRAINT "PK_Sets" PRIMARY KEY AUTOINCREMENT,
                        "ExerciseId" INTEGER NOT NULL,
                        "Weight" REAL NOT NULL,
                        "Reps" INTEGER NOT NULL,
                        "SetNumber" INTEGER NOT NULL,
                        "CreatedAtUtcTicks" INTEGER NOT NULL,
                        "CreatedAtUtcDateTicks" INTEGER NOT NULL GENERATED ALWAYS AS (([CreatedAtUtcTicks] / 864000000000) * 864000000000) STORED, 
                        CONSTRAINT "FK_Sets_Exercises_ExerciseId"
                        FOREIGN KEY ("ExerciseId") REFERENCES "Exercises" ("Id") ON DELETE CASCADE
                    );
                    
                    INSERT INTO "Sets_new" (
                        "Id",
                        "ExerciseId",
                        "Weight",
                        "Reps",
                        "SetNumber",
                        "CreatedAtUtcTicks"
                    )
                    SELECT
                        "Id",
                        "ExerciseId",
                        "Weight",
                        "Reps",
                        "SetNumber",
                        "CreatedAtUtcTicks"
                    FROM "Sets";

                    DROP TABLE "Sets";
                    ALTER TABLE "Sets_new" RENAME TO "Sets";

                    CREATE UNIQUE INDEX "IX_Sets_Exercise_Date_SetNumber" 
                    ON "Sets" ("ExerciseId", "CreatedAtUtcDateTicks", "SetNumber");
                    """);
            }
            else
            {
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
