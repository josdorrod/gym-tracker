using GymTracker.Data;
using GymTracker.Data.Entities;
using GymTracker.DTO;
using GymTracker.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Pages.Exercises;

public class IndexModel : PageModel
{
    private readonly IExerciseService _exerciseService;

    public IndexModel(IExerciseService exerciseService)
    {
        _exerciseService = exerciseService ?? throw new ArgumentNullException(nameof(exerciseService));
    }
    public IList<ExerciseListDTO> Exercises { get; set; } = [];

    public async Task OnGetAsync()
    {
        Exercises = await _exerciseService.GetAllExercisesAsync();
    }
}
