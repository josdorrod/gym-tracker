using GymTracker.Data;
using GymTracker.Data.Entities;
using GymTracker.DTO;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Services;

/// <summary>
/// Implements location persistence and business rules.
/// </summary>
public class LocationService(GymTrackerDbContext db) : ILocationService
{
    private readonly GymTrackerDbContext _db = db ?? throw new ArgumentNullException(nameof(db));

    /// <inheritdoc/>
    public async Task<List<LocationListDTO>> GetAllLocationsAsync()
    {
        return await _db.Locations
            .OrderBy(location => location.Name)
            .Select(location => new LocationListDTO
            {
                Id = location.Id,
                Name = location.Name
            })
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<LocationEditDTO?> GetLocationForEditAsync(int id)
    {
        return await _db.Locations
            .Where(location => location.Id == id)
            .Select(location => new LocationEditDTO
            {
                Name = location.Name
            })
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public async Task<LocationListDTO?> GetLocationByIdAsync(int id)
    {
        return await _db.Locations
            .Where(location => location.Id == id)
            .Select(location => new LocationListDTO
            {
                Id = location.Id,
                Name = location.Name
            })
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public async Task<int> CreateLocationAsync(LocationEditDTO locationDto)
    {
        ArgumentNullException.ThrowIfNull(locationDto);

        var location = new Location
        {
            Name = locationDto.Name.Trim()
        };

        _db.Locations.Add(location);
        await _db.SaveChangesAsync();

        return location.Id;
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateLocationAsync(int id, LocationEditDTO locationDto)
    {
        ArgumentNullException.ThrowIfNull(locationDto);

        var location = await _db.Locations.FindAsync(id);
        if (location is null)
        {
            return false;
        }

        location.Name = locationDto.Name.Trim();

        await _db.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc/>
    public async Task<LocationDeleteResult> DeleteLocationAsync(int id)
    {
        var location = await _db.Locations.FindAsync(id);
        if (location is null)
        {
            return LocationDeleteResult.NotFound;
        }

        var hasSessions = await _db.WorkoutSessions.AnyAsync(session => session.LocationId == id);
        if (hasSessions)
        {
            return LocationDeleteResult.InUse;
        }

        _db.Locations.Remove(location);
        await _db.SaveChangesAsync();
        return LocationDeleteResult.Deleted;
    }
}