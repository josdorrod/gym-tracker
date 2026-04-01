using System.ComponentModel.DataAnnotations;

namespace GymTracker.DTO;
public class SetDetailListDTO
{
    /// <summary>Gets or sets the primary key.</summary>
    public int Id { get; set; }

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
    
    /// <summary>Gets the UTC timestamp when the set was recorded.</summary>
    public DateTimeOffset CreatedAt
    {
        get { return new DateTimeOffset(CreatedAtUtcTicks, TimeSpan.Zero); }
    }
}