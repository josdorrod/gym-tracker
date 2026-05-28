using GymTracker.DTO;

namespace GymTracker.Services;

/// <summary>
/// Provides CRUD operations for locations.
/// </summary>
public interface ILocationService
{
    /// <summary>
    /// Gets all locations ordered by name.
    /// </summary>
    /// <returns>The available locations.</returns>
    Task<List<LocationListDTO>> GetAllLocationsAsync();

    /// <summary>
    /// Gets a location for editing.
    /// </summary>
    /// <param name="id">The location identifier.</param>
    /// <returns>The editable location data, or <see langword="null"/> if it does not exist.</returns>
    Task<LocationEditDTO?> GetLocationForEditAsync(int id);

    /// <summary>
    /// Gets a location by identifier.
    /// </summary>
    /// <param name="id">The location identifier.</param>
    /// <returns>The location data, or <see langword="null"/> if it does not exist.</returns>
    Task<LocationListDTO?> GetLocationByIdAsync(int id);

    /// <summary>
    /// Creates a new location.
    /// </summary>
    /// <param name="locationDto">The location data.</param>
    /// <returns>The identifier of the new location.</returns>
    Task<int> CreateLocationAsync(LocationEditDTO locationDto);

    /// <summary>
    /// Updates an existing location.
    /// </summary>
    /// <param name="id">The location identifier.</param>
    /// <param name="locationDto">The new location data.</param>
    /// <returns><see langword="true"/> when the location exists and was updated; otherwise, <see langword="false"/>.</returns>
    Task<bool> UpdateLocationAsync(int id, LocationEditDTO locationDto);

    /// <summary>
    /// Deletes a location.
    /// </summary>
    /// <param name="id">The location identifier.</param>
    /// <returns>The outcome of the delete operation.</returns>
    Task<LocationDeleteResult> DeleteLocationAsync(int id);
}

/// <summary>
/// Represents the result of deleting a location.
/// </summary>
public enum LocationDeleteResult
{
    /// <summary>
    /// The location was deleted.
    /// </summary>
    Deleted = 0,

    /// <summary>
    /// The location does not exist.
    /// </summary>
    NotFound = 1,

    /// <summary>
    /// The location is still referenced by workout sessions.
    /// </summary>
    InUse = 2
}