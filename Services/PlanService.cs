using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using GymTracker.Data;
using GymTracker.DTO;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Services;
/// <summary>
/// Implements plan business logic and data access for workout plans.
/// </summary>
public class PlanService : IPlanService
{

    private readonly GymTrackerDbContext _db;

    public PlanService(GymTrackerDbContext db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }

    /// <inheritdoc/>
    public async Task<PlanCreateDTO?> GetPlanByIdAsync(int id)
    {
        var plan = await _db.Plans.FindAsync(id);
        if (plan is null)
        {
            return null;
        } 

        return new PlanCreateDTO
        {
            Name = plan.Name,
            Description = plan.Description,
            IsActive = plan.IsActive
        };
    }

    /// <inheritdoc/>
    public async Task<int> CreatePlanAsync(PlanCreateDTO plan)
    {
        if (plan is null)
        {
            throw new ArgumentNullException(nameof(plan));
        }

        var newPlan = new Data.Entities.Plan
        {
            Name = plan.Name,
            Description = plan.Description,
            IsActive = plan.IsActive
        };
        _db.Plans.Add(newPlan);
        await _db.SaveChangesAsync();

        return newPlan.Id;
    }

    /// <inheritdoc/>
    public async Task<List<PlanListDTO>> GetAllPlansAsync()
    {
        return await _db.Plans
            .Where(p => p.IsActive == true)
            .OrderBy(p => p.Name)
            .Select(p => new PlanListDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description
            })
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<bool> UpdatePlanAsync(int id, PlanCreateDTO plan)
    {
        if (plan is null)
        {
            throw new ArgumentNullException(nameof(plan));
        }

        var existingPlan = await _db.Plans.FindAsync(id);
        if (existingPlan is null)
        {
            return false;
        }

        existingPlan.Name = plan.Name;
        existingPlan.Description = plan.Description;
        existingPlan.IsActive = plan.IsActive;

        _db.Plans.Update(existingPlan);
        await _db.SaveChangesAsync();

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> DeletePlanAsync(int id)
    {
        var existingPlan = await _db.Plans.FindAsync(id);
        if (existingPlan is null)
        {
            return false;
        }

        existingPlan.IsActive = false;
        _db.Plans.Update(existingPlan);
        await _db.SaveChangesAsync();

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> AddExerciseToPlanAsync(int planId, int exerciseId)
    {
        var plan = await _db.Plans.FindAsync(planId);
        if (plan is null)
        {
            throw new ArgumentException($"Plan with id {planId} not found.", nameof(planId));
        }

        var exercise = await _db.Exercises.FindAsync(exerciseId);
        if (exercise is null)
        {
            throw new ArgumentException($"Exercise with id {exerciseId} not found.", nameof(exerciseId));
        }

        bool alreadyAdded = await _db.PlanExercises.AnyAsync(pe => pe.PlanId == planId
                            && pe.ExerciseId == exerciseId);
        
        if (alreadyAdded)
        {
            return false;
        }

        int currenMaxOrder = await _db.PlanExercises
            .Where(pe => pe.PlanId == planId)
            .Select(pe => (int?)pe.Order)
            .MaxAsync() ?? 0;

        var newPlanExercise = new Data.Entities.PlanExercise
        {
            PlanId = planId,
            ExerciseId = exerciseId,
            Order = currenMaxOrder + 1
        };

        _db.PlanExercises.Add(newPlanExercise);
        await _db.SaveChangesAsync();

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> RemoveExerciseFromPlanAsync(int planId, int exerciseId)
    {
        var planExercise = await _db.PlanExercises
            .FirstOrDefaultAsync(pe => pe.PlanId == planId && pe.ExerciseId == exerciseId);
        if (planExercise is null)
        {
            return false; 
        }

        _db.PlanExercises.Remove(planExercise);
        await _db.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc/>
    public async Task<List<ExerciseListDTO>> GetExercisesForPlanAsync(int planId)
    {
        var exercises = await _db.PlanExercises
            .Where(pe => pe.PlanId == planId)
            .Include(pe => pe.Exercise)
            .OrderBy(pe => pe.Order)
            .Select(pe => new ExerciseListDTO
            {
                Id = pe.Exercise.Id,
                Name = pe.Exercise.Name,
                MuscleGroup = pe.Exercise.MuscleGroup
            })
            .ToListAsync();

        return exercises;  
    }
}