using System.ComponentModel.DataAnnotations;

namespace GymTracker.DTO;

public class ExerciseCreateDTO
{
    /// <summary>Get or set the name of the exercise.</summary>
    [Required]
    public string Name { get; set; } = String.Empty;
    /// <summary>Get or set the muscle group targeted by the exercise.</summary>
    [MaxLength(50)]
    public string? MuscleGroup { get; set; }
    /// <summary>Get or set the number of planned sets for the exercise.</summary>
    [Range(1, 10, ErrorMessage = "El número de series debe ser mayor a 0 y menor a 10.")]
    public int PlannedSets { get; set; } = 3;
    /// <summary>Get or set the instructions for the exercise.</summary>
    public string? Instructions { get; set; }
}