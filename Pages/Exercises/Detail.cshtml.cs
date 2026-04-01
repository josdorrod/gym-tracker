using GymTracker.Data;
using GymTracker.Data.Entities;
using GymTracker.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace GymTracker.Pages.Exercises;

public class DetailModel(GymTrackerDbContext db) : PageModel
{
    public ExerciseInputDTO ExerciseDto { get; set; } = null!;
    public IList<SetDetailListDTO> Sets { get; set; } = [];

    [BindProperty]
    public SetInputDTO NewSet { get; set; } = new();

    public int GetSetCount { get; set; } = 0;

    private int GetTodaySetCount(int exerciseId)
    {
        DateTime inicioDia = DateTime.Now.Date;
        DateTime finDia = inicioDia.AddDays(1).AddTicks(-1);

        long ticksInicioDia = inicioDia.Ticks;
        long ticksFinDia = finDia.Ticks;

        int setsTodayCount = db.Sets
            .Where(s => s.ExerciseId == exerciseId && s.CreatedAtUtcTicks >= ticksInicioDia && s.CreatedAtUtcTicks <= ticksFinDia)
            .Select(s => (int?)s.SetNumber)
            .Max() ?? 0;

        return setsTodayCount;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var exercise = await db.Exercises
        .Include(e => e.Sets)
        .FirstOrDefaultAsync(e => e.Id == id);
        if (exercise is null)
            return NotFound();

        ExerciseDto = new ExerciseInputDTO
        {
            Name = exercise.Name,
            MuscleGroup = exercise.MuscleGroup,
            PlannedSets = exercise.PlannedSets,
            Instructions = exercise.Instructions
        };
        Sets = exercise.Sets
            .Select(s => new SetDetailListDTO
            {
                Id = s.Id,
                Weight = s.Weight,
                Reps = s.Reps,
                SetNumber = s.SetNumber,
                CreatedAtUtcTicks = s.CreatedAtUtcTicks
            })
            .OrderByDescending(s => s.CreatedAtUtcTicks)
            .ToList();
        
        GetSetCount = GetTodaySetCount(id);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            var exercise = await db.Exercises
            .Include(e => e.Sets)
            .FirstOrDefaultAsync(e => e.Id == id);
            if (exercise is null)
                return NotFound();

            ExerciseDto = new ExerciseInputDTO
            {
                Name = exercise.Name,
                MuscleGroup = exercise.MuscleGroup,
                PlannedSets = exercise.PlannedSets,
                Instructions = exercise.Instructions
            };
            Sets = exercise.Sets
                .Select(s => new SetDetailListDTO
                {
                    Id = s.Id,
                    Weight = s.Weight,
                    Reps = s.Reps,
                    SetNumber = s.SetNumber,
                    CreatedAtUtcTicks = s.CreatedAtUtcTicks
                })
                .OrderByDescending(s => s.CreatedAtUtcTicks)
                .ToList();

            return Page();
        }

        GetSetCount = GetTodaySetCount(id) + 1;

        db.Sets.Add(new Set
        {
            ExerciseId = id,
            SetNumber = GetSetCount,
            Weight = NewSet.Weight,
            Reps = NewSet.Reps,
            CreatedAtUtcTicks = DateTimeOffset.UtcNow.UtcTicks
        });
        await db.SaveChangesAsync();

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int setId)
    {
        var set = await db.Sets.FindAsync(setId);
        if (set is null)
        {
            return NotFound();
        }

        db.Sets.Remove(set);
        await db.SaveChangesAsync();

        return RedirectToPage();
    }
}
