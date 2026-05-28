using GymTracker.Data;
using GymTracker.Data.Entities;
using GymTracker.DTO;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Services;

public class SetService : ISetService
{
    private GymTrackerDbContext _db;
    private readonly IDbExceptionHelper _dbExceptionHelper;

    public SetService(GymTrackerDbContext db, IDbExceptionHelper dbExceptionHelper)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _dbExceptionHelper = dbExceptionHelper ?? throw new ArgumentNullException(nameof(dbExceptionHelper));
    }

    public async Task<int> GetTodaySetCount(int exerciseId)
    {
        long todayDateTicks = DateTime.UtcNow.Date.Ticks;
        var setsTodayCount = await _db.Sets
            .Where(s => s.ExerciseId == exerciseId 
                    && s.CreatedAtUtcDateTicks == todayDateTicks)
            .Select(s => (int?)s.SetNumber)
            .MaxAsync() ?? 0;

        return setsTodayCount;
    }

    public async Task<List<SetDetailListDTO>> GetAllSetsAsync(int exerciseId)
    {
        return await _db.Sets
        .Where(s => s.ExerciseId == exerciseId)
        .OrderByDescending(s => s.CreatedAtUtcTicks)
        .Select(s => new SetDetailListDTO
        {
            Id = s.Id,
            Weight = s.Weight,
            Reps = s.Reps,
            SetNumber = s.SetNumber,
            CreatedAtUtcTicks = s.CreatedAtUtcTicks
        })
        .ToListAsync();
    }

    public async Task<int> CreateSetAsync(int exerciseId, SetCreateDTO setDto)
    {
        if (setDto is null)
        {
            throw new ArgumentNullException(nameof(setDto));
        }

        const int maxAttempts = 3;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            int setNumber = await GetTodaySetCount(exerciseId) + 1;

            var setEntity = new Set
            {
                ExerciseId = exerciseId,
                Weight = setDto.Weight,
                Reps = setDto.Reps,
                SetNumber = setNumber,
                WorkoutSessionId = setDto.WorkoutSessionId,
                CreatedAtUtcTicks = DateTime.UtcNow.Ticks,
            };
            _db.Sets.Add(setEntity);
            try
            {
                await _db.SaveChangesAsync();
                return setEntity.Id;
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                _db.Entry(setEntity).State = EntityState.Detached;
                if (attempt == maxAttempts -1)
                {
                    throw new InvalidOperationException("Failed to create set after multiple attempts due to concurrent updates. Please try again.", ex);
                }
                await Task.Delay(50 * (attempt + 1));
                continue;
            }
        }
        throw new InvalidOperationException("Failed to create set after multiple attempts due to concurrent updates. Please try again.");
    }

    private bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        return _dbExceptionHelper.IsUniqueConstraintViolation(ex);
    }

    public async Task<bool> DeleteSetAsync(int setId)
    {
        var set = await _db.Sets.FindAsync(setId);
        if (set is null)
        {
            return false;
        }

        _db.Sets.Remove(set);
        await _db.SaveChangesAsync();

        return true;
    }
}