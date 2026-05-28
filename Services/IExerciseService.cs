using System.Security.Cryptography.X509Certificates;
using GymTracker.DTO;

namespace GymTracker.Services;

/// <summary>
/// Provides CRUD operations for exercises.
/// </summary>
public interface IExerciseService
{
    /// <summary>
    /// Gets an exercise by its ID.
    /// </summary>
    /// <param name="id">The ID of the exercise.</param>
    /// <returns>The exercise with the specified ID, or <see langword="null"/> if not found.</returns>
    public Task<ExerciseCreateDTO?> GetExerciseByIdAsync(int id);

    /// <summary>
    /// Gets all exercises.
    /// </summary>
    /// <returns>A list of all exercises.</returns>
    public Task<List<ExerciseListDTO>> GetAllExercisesAsync();

    /// <summary>
    /// Creates a new exercise.
    /// </summary>
    /// <param name="exerciseDto">The exercise data transfer object.</param>
    /// <returns>The ID of the newly created exercise.</returns>
    public Task<int> CreateExerciseAsync(ExerciseCreateDTO exerciseDto);

    /// <summary>
    /// Updates an existing exercise.
    /// </summary>
    /// <param name="id">The ID of the exercise to update.</param>
    /// <param name="exerciseDto">The updated exercise data.</param>
    /// <returns><see langword="true"/> if the update was successful; otherwise, <see langword="false"/>.</returns>
    public Task<bool> UpdateExerciseAsync(int id, ExerciseCreateDTO exerciseDto);

    /// <summary>
    /// Searches for exercises by name or muscle group.
    /// </summary>
    /// <param name="query">The search query to match against exercise names and muscle groups.</param>
    /// <returns>A list of exercises that match the search query.</returns>
    public Task<List<ExerciseListDTO>> SearchExercisesAsync(string? query);
}