using GymTracker.Data.Entities;
using GymTracker.DTO;

namespace GymTracker.Services
{
    /// <summary>
    /// Provides CRUD operations for workout plans.
    /// </summary>
    public interface IPlanService
    {
        /// <summary>
        /// Retrieves a workout plan by its ID.
        /// </summary>
        /// <param name="id">The ID of the workout plan.</param>
        /// <returns>The workout plan if found; otherwise, <see langword="null"/>.</returns>
        Task<PlanCreateDTO?> GetPlanByIdAsync(int id);

        /// <summary>
        /// Retrieves all workout plans.
        /// </summary>        
        /// /// <returns>A list of workout plans.</returns>
        Task<List<PlanListDTO>> GetAllPlansAsync();

        /// <summary>
        /// Creates a new workout plan.
        /// </summary>
        /// <param name="plan">The workout plan to create.</param>
        /// <returns>The ID of the newly created workout plan.</returns>
        Task<int> CreatePlanAsync(PlanCreateDTO plan);

        /// <summary> 
        /// Updates an existing workout plan.
        /// </summary> 
        /// <param name="id">The ID of the workout plan to update.</param>
        /// <param name="plan">The updated workout plan data.</param>
        /// <returns><see langword="true"/> if the update was successful; otherwise, <see langword="false"/>.</returns>
        Task<bool> UpdatePlanAsync(int id, PlanCreateDTO plan);

        /// <summary>
        /// Deletes a workout plan by its ID.
        /// </summary>
        /// <param name="id">The ID of the workout plan to delete.</param>
        /// <returns><see langword="true"/> if the deletion was successful; otherwise, <see langword="false"/>.</returns>
        Task<bool> DeletePlanAsync(int id);

        /// <summary>
        /// Adds an exercise to a workout plan.
        /// </summary>
        /// <param name="planId">The ID of the workout plan.</param>
        /// <param name="exerciseId">The ID of the exercise to add.</param>
        /// <param name="searchQuery">The search query to match against exercise names and muscle groups.</param>
        /// <returns><see langword="true"/> if the exercise was successfully added; otherwise, <see langword="false"/>.</returns>
        Task<bool> AddExerciseToPlanAsync(int planId, int exerciseId);

        /// <summary>
        /// Removes an exercise from a workout plan.
        /// </summary>
        /// <param name="planId">The ID of the workout plan.</param>
        /// <param name="exerciseId">The ID of the exercise to remove.</param>
        /// <returns><see langword="true"/> if the exercise was successfully removed; otherwise, <see langword="false"/>.</returns>
        Task<bool> RemoveExerciseFromPlanAsync(int planId, int exerciseId);

        /// <summary>
        /// Retrieves all exercises for a specific workout plan.
        /// </summary>
        /// <param name="planId">The ID of the workout plan.</param>
        /// <returns>A list of exercises for the specified workout plan.</returns>
        Task<List<ExerciseListDTO>> GetExercisesForPlanAsync(int planId);
    }
}