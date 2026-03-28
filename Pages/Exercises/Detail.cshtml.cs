using GymTracker.Data;
using GymTracker.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Pages.Exercises;

public class DetailModel(GymTrackerDbContext db) : PageModel
{
    public Exercise Exercise { get; set; } = null!;
    public IList<Set> Sets { get; set; } = [];

    [BindProperty]
    public Set NewSet { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var exercise = await db.Exercises.FindAsync(id);
        if (exercise is null)
            return NotFound();

        Exercise = exercise;
        Sets = await db.Sets
            .Where(s => s.ExerciseId == id)
            .OrderByDescending(s => s.CreatedAtUtcTicks)
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            var exercise = await db.Exercises.FindAsync(id);
            if (exercise is null)
                return NotFound();

            Exercise = exercise;
            Sets = await db.Sets
                .Where(s => s.ExerciseId == id)
                .OrderByDescending(s => s.CreatedAtUtcTicks)
                .ToListAsync();

            return Page();
        }

        db.Sets.Add(new Set
        {
            ExerciseId = id,
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
