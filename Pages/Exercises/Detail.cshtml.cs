using GymTracker.Data;
using GymTracker.Data.Entities;
using GymTracker.DTO;
using GymTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace GymTracker.Pages.Exercises;

public class DetailModel : PageModel
{
    private readonly IExerciseService _exerciseService;
    private readonly ISetService _setService;
    private readonly IWorkoutSessionService _sessionService;

    public DetailModel(
        IExerciseService exerciseService,
        ISetService setService,
        IWorkoutSessionService sessionService)
    {
        _exerciseService = exerciseService ?? throw new ArgumentNullException(nameof(exerciseService));
        _setService = setService ?? throw new ArgumentNullException(nameof(setService));
        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
    }

    public ExerciseCreateDTO ExerciseDto { get; set; } = null!;
    public IList<SetDetailListDTO> Sets { get; set; } = [];

    [BindProperty]
    public SetCreateDTO NewSet { get; set; } = new();

    public int GetSetCount { get; set; } = 0;

    public WorkoutSessionActiveDTO? ActiveSession { get; set; }


    public async Task<IActionResult> OnGetAsync(int id)
    {
        var exercise = await _exerciseService.GetExerciseByIdAsync(id);
        if (exercise is null)
        {
            return NotFound();
        }

        Sets = await _setService.GetAllSetsAsync(id, 10);
        ExerciseDto = exercise;

        GetSetCount = await _setService.GetTodaySetCount(id);

        ActiveSession = await _sessionService.GetActiveSessionAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        ActiveSession = await _sessionService.GetActiveSessionAsync();

        if (ActiveSession is null)
        {
            return RedirectToPage("/WorkoutSessions/Start", new { returnUrl = Url.Page("/Exercises/Detail", new { id }) });
        }

        if (!ModelState.IsValid)
        {
            var exercise = await _exerciseService.GetExerciseByIdAsync(id);
            if (exercise is null)
                return NotFound();

            Sets = await _setService.GetAllSetsAsync(id, 10);

            return Page();
        }

        NewSet.WorkoutSessionId = ActiveSession.Id;
        int newSetId = await _setService.CreateSetAsync(id, NewSet);

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int setId)
    {
        bool setDeleted = await _setService.DeleteSetAsync(setId);
        if (!setDeleted)
        {
            return NotFound();
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEndSessionAsync(int id)
    {
        await _sessionService.CloseActiveSessionAsync();
        return RedirectToPage(new { id });
    }
}
