using GymTracker.Data;
using GymTracker.DTO;
using Microsoft.EntityFrameworkCore;
using GymTracker.Data.Entities;

namespace GymTracker.Services;
public class ExerciseService : IExerciseService
{
    private readonly GymTrackerDbContext _db;

    public ExerciseService(GymTrackerDbContext db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }

    /// <summary>
    /// Gets an exercise by its id. Returns null if not found.
    /// </summary>
    /// <param name="id">The id of the exercise.</param>
    /// <returns>The exercise DTO if found, otherwise null.</returns>
    public async Task<ExerciseInputDTO?> GetExerciseByIdAsync(int id)
    {
        var exercise = await _db.Exercises
            .Include(e => e.Sets)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (exercise is null)
        {
            return null;
        }
        
        return new ExerciseInputDTO
        {
            Name = exercise.Name,
            MuscleGroup = exercise.MuscleGroup,
            PlannedSets = exercise.PlannedSets,
            Instructions = exercise.Instructions
        };
    }

    /// <summary>
    /// Gets all exercises in the database, ordered by name.
    /// </summary>
    /// <returns>A list of exercise DTOs.</returns>
    public async Task<List<ExerciseListDTO>> GetAllExercisesAsync()
    {
        return await _db.Exercises
            .OrderBy(e => e.Name)
            .Select(e => new ExerciseListDTO
            {
                Id = e.Id,
                Name = e.Name,
                MuscleGroup = e.MuscleGroup 
            })
            .ToListAsync();
    }

    /// <summary>
    /// Creates a new exercise in the database and returns its id.
    /// </summary>
    /// <param name="exerciseDto">The exercise DTO containing the data for the new exercise.</param>
    /// <returns>The id of the newly created exercise.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<int> CreateExerciseAsync(ExerciseInputDTO exerciseDto)
    {
        if (exerciseDto is null)
        {
            throw new ArgumentNullException(nameof(exerciseDto));
        }

        var exercise = new Exercise
        {
            Name = exerciseDto.Name,
            MuscleGroup = exerciseDto.MuscleGroup,
            PlannedSets = exerciseDto.PlannedSets,
            Instructions = exerciseDto.Instructions
        };
        _db.Exercises.Add(exercise);
        await _db.SaveChangesAsync();

        return exercise.Id;
    }

    /// <summary>
    /// Updates an existing exercise in the database. Returns true if the exercise was found and updated, false otherwise.
    /// </summary>
    /// <param name="id">The id of the exercise to update.</param>
    /// <param name="exerciseDto">The exercise DTO containing the updated data.</param>
    /// <returns>True if the exercise was found and updated, false otherwise.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<bool> UpdateExerciseAsync(int id, ExerciseInputDTO exerciseDto)
    {
        if (exerciseDto is null)
        {
            throw new ArgumentNullException(nameof(exerciseDto));
        }

        var exerciseDb = await _db.Exercises.FindAsync(id);
        if (exerciseDb is null)
        {
            return false;
        }

        exerciseDb.Name = exerciseDto.Name;
        exerciseDb.MuscleGroup = exerciseDto.MuscleGroup;
        exerciseDb.PlannedSets = exerciseDto.PlannedSets;
        exerciseDb.Instructions = exerciseDto.Instructions;

        await _db.SaveChangesAsync();

        return true;
    }
}