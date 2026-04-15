using System.Collections;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using GymTracker.Data.Entities;
using GymTracker.DTO;
using GymTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GymTracker.Pages.Plans;

public class DetailModel : PageModel
{

    private readonly IPlanService _planService;
    private readonly IExerciseService _exerciseService;

    public DetailModel(IPlanService planService, IExerciseService exerciseService)
    {
        _planService = planService ?? throw new ArgumentNullException(nameof(planService));
        _exerciseService = exerciseService ?? throw new ArgumentNullException(nameof(exerciseService));
    }

    public PlanCreateDTO PlanDto { get; set; } = null!;
    public IList<ExerciseListDTO> SearchResults { get; set; } = [];
    public string SearchExerciseQuery { get; set; } = string.Empty;
    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }
    public IList<ExerciseListDTO> PlanExercises { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(int id, string? searchExercise)
    {
        var plan = await _planService.GetPlanByIdAsync(id);
        if (plan is null)
        {
            return NotFound();
        }

        PlanExercises = await _planService.GetExercisesForPlanAsync(id);

        Id = id;
        PlanDto = plan;
        SearchExerciseQuery = searchExercise ?? string.Empty;
        SearchResults = await _exerciseService.SearchExercisesAsync(SearchExerciseQuery);

        return Page();
    }

    public async Task<IActionResult> OnPostAddExerciseAsync(int Id, int exerciseId, string? searchExercise)
    {
        var plan = await _planService.GetPlanByIdAsync(Id);
        if (plan is null)
        {
            return NotFound();
        }

        await _planService.AddExerciseToPlanAsync(Id, exerciseId);

        return RedirectToPage(new { id = Id, searchExercise });
    }

    public async Task<IActionResult> OnPostRemoveExerciseAsync(int Id, int exerciseId)
    {
        var removedExerciseFromPlan = await _planService.RemoveExerciseFromPlanAsync(Id, exerciseId);

        if (!removedExerciseFromPlan)
        {
            return NotFound();
        }

        return RedirectToPage();
    }
}