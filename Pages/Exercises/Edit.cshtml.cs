using GymTracker.Data;
using GymTracker.Data.Entities;
using GymTracker.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GymTracker.Pages.Exercises;

public class EditModel(GymTrackerDbContext db) : PageModel
{
    [BindProperty]
    public ExerciseInputDTO ExerciseInput {get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var exercise = await db.Exercises.FindAsync(id);
        if (exercise is null)
        {
            return NotFound();
        }

        ExerciseInput = new ExerciseInputDTO
        {
            Name = exercise.Name,
            MuscleGroup = exercise.MuscleGroup,
            PlannedSets = exercise.PlannedSets,
            Instructions = exercise.Instructions
        };
        return Page();

    }

    public async Task<IActionResult> OnPostAsync (int id)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var exerciseDb = await db.Exercises.FindAsync(id);
        if (exerciseDb is null)
        {
            return Page();
        }

        exerciseDb.Name = ExerciseInput.Name;
        exerciseDb.MuscleGroup = ExerciseInput.MuscleGroup;
        exerciseDb.PlannedSets = ExerciseInput.PlannedSets;
        exerciseDb.Instructions = ExerciseInput.Instructions;

        await db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
