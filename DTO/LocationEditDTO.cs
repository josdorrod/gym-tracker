using System.ComponentModel.DataAnnotations;

namespace GymTracker.DTO;

/// <summary>
/// Represents the editable data for a location.
/// </summary>
public class LocationEditDTO
{
    /// <summary>
    /// Gets or sets the location name.
    /// </summary>
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    public string Name { get; set; } = string.Empty;
}