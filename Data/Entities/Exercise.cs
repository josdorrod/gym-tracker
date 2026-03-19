using System.ComponentModel.DataAnnotations;

namespace GymTracker.Data.Entities;

/// <summary>
/// Represents a gym exercise that can be performed and tracked.
/// </summary>
public class Exercise
{
    /// <summary>Gets or sets the primary key.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the name of the exercise.</summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the muscle group targeted by this exercise.</summary>
    [MaxLength(50)]
    public string? MuscleGroup { get; set; }

    /// <summary>Gets or sets optional instructions for performing the exercise.</summary>
    public string? Instructions { get; set; }

    /// <summary>Gets the sets logged for this exercise.</summary>
    public ICollection<Set> Sets { get; set; } = [];
}
