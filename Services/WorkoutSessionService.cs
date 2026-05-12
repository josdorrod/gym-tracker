using GymTracker.Data;
using GymTracker.Data.Entities;
using GymTracker.DTO;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Services;

/// <summary>
/// Implements workout session persistence and business rules.
/// </summary>
public class WorkoutSessionService(GymTrackerDbContext db) : IWorkoutSessionService
{
    private readonly GymTrackerDbContext _db = db ?? throw new ArgumentNullException(nameof(db));

    /// <inheritdoc/>
    public async Task<WorkoutSessionActiveDTO?> GetActiveSessionAsync()
    {
        var utcNow = DateTime.UtcNow;
        var startOfTodayUtc = utcNow.Date;
        var startOfTomorrowUtc = startOfTodayUtc.AddDays(1);

        var session = await _db.WorkoutSessions
            .Where(s => s.StartTime >= startOfTodayUtc && s.StartTime < startOfTomorrowUtc)
            .OrderByDescending(s => s.StartTime)
            .Select(s => new WorkoutSessionActiveDTO
            {
                Id = s.Id,
                LocationId = s.LocationId,
                LocationName = s.Location.Name,
                PlanId = s.PlanId,
                PlanName = s.Plan != null ? s.Plan.Name : null,
                StartTime = s.StartTime
            })
            .FirstOrDefaultAsync();

        return session;
    }

    /// <inheritdoc/>
    public async Task<int> CreateSessionAsync(WorkoutSessionCreateDTO sessionDto)
    {
        ArgumentNullException.ThrowIfNull(sessionDto);

        if (sessionDto.LocationId == 0)
        {
            throw new ArgumentException("LocationId is required.", nameof(sessionDto));
        }

        var location = await _db.Locations.FindAsync(sessionDto.LocationId);
        if (location is null)
        {
            throw new ArgumentException("Invalid location.", nameof(sessionDto));
        }

        Plan? plan = null;
        if (sessionDto.PlanId.HasValue)
        {
            plan = await _db.Plans.FindAsync(sessionDto.PlanId.Value);
            if (plan is null)
            {
                throw new ArgumentException("Invalid plan.", nameof(sessionDto));
            }
        }

        var session = new WorkoutSession
        {
            LocationId = sessionDto.LocationId,
            PlanId = sessionDto.PlanId,
            StartTime = DateTime.UtcNow
        };

        _db.WorkoutSessions.Add(session);
        await _db.SaveChangesAsync();

        return session.Id;
    }
}