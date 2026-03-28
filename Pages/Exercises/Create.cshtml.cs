using GymTracker.Data;
using GymTracker.Data.Entities;
using GymTracker.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Validation;

namespace GymTracker.Pages.Exercises;

public class CreateModel(GymTrackerDbContext db) : PageModel
{
    [BindProperty]
    public ExerciseInputDTO Input { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();
        
        var exercise = new Exercise
        {
            Name = Input.Name,
            MuscleGroup = Input.MuscleGroup,
            PlannedSets = Input.PlannedSets,
            Instructions = Input.Instructions
        };

        db.Exercises.Add(exercise);
        await db.SaveChangesAsync();

        return RedirectToPage("Detail", new { id = exercise.Id });
    }
}
