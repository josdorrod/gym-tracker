using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymTracker.Data.Entities;

/// <summary>
/// Represents a single set performed during a workout, associated with an exercise.
/// </summary>
public class Set
{
    /// <summary>Gets or sets the primary key.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the foreign key to the parent exercise.</summary>
    public int ExerciseId { get; set; }

    /// <summary>Gets or sets the weight lifted in kilograms.</summary>
    [Range(0, 2000)]
    public double Weight { get; set; }

    /// <summary>Gets or sets the number of repetitions performed.</summary>
    [Range(1, 999)]
    public int Reps { get; set; }

    /// <summary>Gets or sets the set number within the exercise.</summary>
    public int SetNumber { get; set; }

    /// <summary>Gets or sets the UTC timestamp ticks when the set was recorded.</summary>
    public long CreatedAtUtcTicks { get; set; }

    /// <summary>Gets or sets the UTC date ticks when the set was recorded (for easier querying).</summary>
    public long CreatedAtUtcDateTicks { get; private set;}

    /// <summary>Gets or sets the navigation property to the parent exercise.</summary>
    public Exercise? Exercise { get; set; }
}
