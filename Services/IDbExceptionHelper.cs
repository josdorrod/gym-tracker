
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Services;
public interface IDbExceptionHelper
{
    /// <summary>
    /// Checks if the given exception is a unique constraint violation.
    /// </summary>
    /// <param name="ex">The exception to check.</param>
    /// <returns>True if the exception is a unique constraint violation, otherwise false.</returns>
    bool IsUniqueConstraintViolation(DbUpdateException ex);    
}