using GymTracker.Data;
using GymTracker.Data.Entities;
using GymTracker.DTO;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Services
{
    public class SetService : ISetService
    {
        private GymTrackerDbContext _db;

        public SetService(GymTrackerDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<int> GetTodaySetCount(int exerciseId)
        {
            DateTime inicioDia = DateTime.Now.Date;
            DateTime finDia = inicioDia.AddDays(1).AddTicks(-1);

            long ticksInicioDia = inicioDia.Ticks;
            long ticksFinDia = finDia.Ticks;

            var setsTodayCount = await _db.Sets
                .Where(s => s.ExerciseId == exerciseId && s.CreatedAtUtcTicks >= ticksInicioDia && s.CreatedAtUtcTicks <= ticksFinDia)
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

            int setNumber = await GetTodaySetCount(exerciseId) + 1;

            var setEntity = new Set
            {
                ExerciseId = exerciseId,
                Weight = setDto.Weight,
                Reps = setDto.Reps,
                SetNumber = setNumber,
                CreatedAtUtcTicks = DateTime.UtcNow.Ticks
            };
            _db.Sets.Add(setEntity);
            await _db.SaveChangesAsync();

            return setEntity.Id;
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
}