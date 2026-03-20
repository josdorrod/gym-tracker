using GymTracker.Data;
using GymTracker.Data.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Pages.Exercises;

public class IndexModel(GymTrackerDbContext db) : PageModel
{
    public IList<Exercise> Exercises { get; set; } = [];

    public async Task OnGetAsync()
    {
        Exercises = await db.Exercises.OrderBy(e => e.Name).ToListAsync();
    }
}
