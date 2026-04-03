using GymTracker.DTO;
using GymTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GymTracker.Pages.Exercises;

public class EditModel : PageModel
{
    private readonly IExerciseService _exerciseService;

    public EditModel(IExerciseService exerciseService)
    {
        _exerciseService = exerciseService ?? throw new ArgumentNullException(nameof(exerciseService));
    }

    [BindProperty]
    public ExerciseInputDTO ExerciseInput {get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var exercise = await _exerciseService.GetExerciseByIdAsync(id);
        if (exercise is null)
        {
            return NotFound();
        }

        ExerciseInput = exercise;

        return Page();

    }

    public async Task<IActionResult> OnPostAsync (int id)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        bool exerciseUpdated = await _exerciseService.UpdateExerciseAsync(id, ExerciseInput);

        if (!exerciseUpdated)
        {
            return NotFound();
        }

        return RedirectToPage("Index");
    }
}
