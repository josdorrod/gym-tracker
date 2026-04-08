using System.Net.WebSockets;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
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

    /// <summary>Gets the workout sessions table.</summary>
    public DbSet<WorkoutSession> WorkoutSessions => Set<WorkoutSession>();

    /// <summary>Gets the plans table.</summary>
    public DbSet<Plan> Plans => Set<Plan>();

    /// <summary>Gets the locations table.</summary>
    public DbSet<Location> Locations => Set<Location>();

    /// <summary>Gets the plan exercises table.</summary>
    public DbSet<PlanExercise> PlanExercises => Set<PlanExercise>();


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

        modelBuilder.Entity<WorkoutSession>()
            .HasMany(ws => ws.Sets)
            .WithOne(s => s.WorkoutSession)
            .HasForeignKey(s => s.WorkoutSessionId)
            .OnDelete(DeleteBehavior.SetNull);
        
        modelBuilder.Entity<WorkoutSession>()
            .HasOne(ws => ws.Plan)
            .WithMany()
            .HasForeignKey(ws => ws.PlanId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WorkoutSession>()
            .HasOne(ws => ws.Location)
            .WithMany()
            .HasForeignKey(ws => ws.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PlanExercise>()
            .HasKey(pe => new { pe.PlanId, pe.ExerciseId })
            .HasName("PK_PlanExercises");

        modelBuilder.Entity<PlanExercise>()
            .HasOne<Exercise>()
            .WithMany()
            .HasForeignKey(pe => pe.ExerciseId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<PlanExercise>()
            .HasOne<Plan>()
            .WithMany()
            .HasForeignKey(pe => pe.PlanId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
