using System.ComponentModel.DataAnnotations;

namespace GymTracker.DTO;
public class SetInputDTO
{
    /// <summary>Gets or sets the weight lifted in kilograms.</summary>
    [Range(0, 2000)]
    public double Weight { get; set; }

    /// <summary>Gets or sets the number of repetitions performed.</summary>
    [Range(1, 999)]
    public int Reps { get; set; }
}