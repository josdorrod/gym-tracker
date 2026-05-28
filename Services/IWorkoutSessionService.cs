using GymTracker.DTO;

namespace GymTracker.Services;

/// <summary>
/// Provides operations for managing workout sessions.
/// </summary>
public interface IWorkoutSessionService
{
    /// <summary>
    /// Gets the most recent workout session from today (if any).
    /// Returns null if no session exists for today.
    /// </summary>
    /// <returns>The active session for today, or <see langword="null"/>.</returns>
    Task<WorkoutSessionActiveDTO?> GetActiveSessionAsync();

    /// <summary>
    /// Creates a new workout session with the specified location and optional plan.
    /// </summary>
    /// <param name="sessionDto">The session data (LocationId required, PlanId optional).</param>
    /// <returns>The identifier of the newly created session.</returns>
    Task<int> CreateSessionAsync(WorkoutSessionCreateDTO sessionDto);

    /// <summary>
    /// Closes the active workout session for today.
    /// </summary>
    /// <returns><see langword="true"/> when a session was closed; otherwise, <see langword="false"/>.</returns>
    Task<bool> CloseActiveSessionAsync();
}
