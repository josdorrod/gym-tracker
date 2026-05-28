using GymTracker.Data;
using GymTracker.Data.Entities;
using GymTracker.DTO;
using GymTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Validation;

namespace GymTracker.Pages.Exercises;

public class CreateModel : PageModel
{
    private readonly IExerciseService _exerciseService;

    public CreateModel(IExerciseService exerciseService)
    {
        _exerciseService = exerciseService ?? throw new ArgumentNullException(nameof(exerciseService));
    }

    [BindProperty]
    public ExerciseCreateDTO Input { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();
        
        int exerciseId = await _exerciseService.CreateExerciseAsync(Input);
        return RedirectToPage("Detail", new { id = exerciseId });
    }
}
