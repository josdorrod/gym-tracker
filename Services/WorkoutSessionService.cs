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
        await CloseStaleOpenSessionsAsync(utcNow);

        var startOfTodayUtc = utcNow.Date;
        var startOfTomorrowUtc = startOfTodayUtc.AddDays(1);

        var session = await _db.WorkoutSessions
            .Where(s => s.EndTime == null
                && s.StartTime >= startOfTodayUtc
                && s.StartTime < startOfTomorrowUtc)
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

        var utcNow = DateTime.UtcNow;
        await CloseStaleOpenSessionsAsync(utcNow);

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

        var startOfTodayUtc = utcNow.Date;
        var startOfTomorrowUtc = startOfTodayUtc.AddDays(1);
        var activeSessionId = await _db.WorkoutSessions
            .Where(s => s.EndTime == null
                && s.StartTime >= startOfTodayUtc
                && s.StartTime < startOfTomorrowUtc)
            .OrderByDescending(s => s.StartTime)
            .Select(s => (int?)s.Id)
            .FirstOrDefaultAsync();

        if (activeSessionId.HasValue)
        {
            return activeSessionId.Value;
        }

        var session = new WorkoutSession
        {
            LocationId = sessionDto.LocationId,
            PlanId = sessionDto.PlanId,
            StartTime = utcNow
        };

        _db.WorkoutSessions.Add(session);
        await _db.SaveChangesAsync();

        return session.Id;
    }

    /// <inheritdoc/>
    public async Task<bool> CloseActiveSessionAsync()
    {
        var utcNow = DateTime.UtcNow;
        await CloseStaleOpenSessionsAsync(utcNow);

        var startOfTodayUtc = utcNow.Date;
        var startOfTomorrowUtc = startOfTodayUtc.AddDays(1);
        var session = await _db.WorkoutSessions
            .Where(s => s.EndTime == null
                && s.StartTime >= startOfTodayUtc
                && s.StartTime < startOfTomorrowUtc)
            .OrderByDescending(s => s.StartTime)
            .FirstOrDefaultAsync();

        if (session is null)
        {
            return false;
        }

        session.EndTime = utcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    private async Task CloseStaleOpenSessionsAsync(DateTime utcNow)
    {
        var startOfTodayUtc = utcNow.Date;
        var staleSessions = await _db.WorkoutSessions
            .Where(s => s.EndTime == null && s.StartTime < startOfTodayUtc)
            .ToListAsync();

        if (staleSessions.Count == 0)
        {
            return;
        }

        foreach (var session in staleSessions)
        {
            session.EndTime = session.StartTime.Date.AddDays(1).AddTicks(-1);
        }

        await _db.SaveChangesAsync();
    }
}
