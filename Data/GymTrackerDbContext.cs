using System.Reflection.Emit;
using GymTracker.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Data;

/// <summary>
/// EF Core database context for GymTracker.
/// </summary>
public class GymTrackerDbContext(DbContextOptions<GymTrackerDbContext> options) : DbContext(options)
{
    /// <summary>Gets the exercises table.</summary>
    public DbSet<Exercise> Exercises => Set<Exercise>();

    /// <summary>Gets the sets table.</summary>
    public DbSet<Set> Sets => Set<Set>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Exercise>()
            .HasMany(e => e.Sets)
            .WithOne(s => s.Exercise)
            .HasForeignKey(s => s.ExerciseId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Set>(eb =>
        {
            eb.Property(s => s.CreatedAtUtcDateTicks)
            .HasComputedColumnSql("([CreatedAtUtcTicks] / 864000000000) * 864000000000", stored: true);

            eb.HasIndex(s => new { s.ExerciseId, s.CreatedAtUtcDateTicks, s.SetNumber})
            .IsUnique()
            .HasDatabaseName("IX_Sets_Exercise_Date_SetNumber");
        });
    }
}
