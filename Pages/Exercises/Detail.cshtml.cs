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
    public DetailModel(IExerciseService exerciseService, ISetService setService)
    {
        _exerciseService = exerciseService ?? throw new ArgumentNullException(nameof(exerciseService));
        _setService = setService ?? throw new ArgumentNullException(nameof(exerciseService));
    }

    public ExerciseInputDTO ExerciseDto { get; set; } = null!;
    public IList<SetDetailListDTO> Sets { get; set; } = [];

    [BindProperty]
    public SetInputDTO NewSet { get; set; } = new();

    public int GetSetCount { get; set; } = 0;


    public async Task<IActionResult> OnGetAsync(int id)
    {
        var exercise = await _exerciseService.GetExerciseByIdAsync(id);
        if (exercise is null)
        {
            return NotFound();
        }

        Sets = await _setService.GetAllSetsAsync(id);
        ExerciseDto = exercise;

        GetSetCount = await _setService.GetTodaySetCount(id);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            var exercise = await _exerciseService.GetExerciseByIdAsync(id);
            if (exercise is null)
                return NotFound();

            Sets = await _setService.GetAllSetsAsync(id);

            return Page();
        }

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
}
