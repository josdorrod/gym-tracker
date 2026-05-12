namespace GymTracker.DTO;

/// <summary>
/// Represents the current active workout session for display purposes.
/// </summary>
public class WorkoutSessionActiveDTO
{
    /// <summary>
    /// Gets or sets the session identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the location identifier.
    /// </summary>
    public int LocationId { get; set; }

    /// <summary>
    /// Gets or sets the location name.
    /// </summary>
    public string LocationName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the plan identifier, if any.
    /// </summary>
    public int? PlanId { get; set; }

    /// <summary>
    /// Gets or sets the plan name, if any.
    /// </summary>
    public string? PlanName { get; set; }

    /// <summary>
    /// Gets or sets the session start time.
    /// </summary>
    public DateTime StartTime { get; set; }
}

/// <summary>
/// Input DTO for creating a new workout session.
/// </summary>
public class WorkoutSessionCreateDTO
{
    /// <summary>
    /// Gets or sets the location identifier (required).
    /// </summary>
    public int LocationId { get; set; }

    /// <summary>
    /// Gets or sets the plan identifier (optional).
    /// </summary>
    public int? PlanId { get; set; }
}