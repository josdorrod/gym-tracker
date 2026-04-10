namespace GymTracker.DTO;

/// <summary>
/// Represents a location in list and detail views.
/// </summary>
public class LocationListDTO
{
    /// <summary>
    /// Gets or sets the location identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the location name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}