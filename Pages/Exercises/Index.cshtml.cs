using GymTracker.Data;
using GymTracker.Data.Entities;
using GymTracker.DTO;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Pages.Exercises;

public class IndexModel(GymTrackerDbContext db) : PageModel
{
    public IList<ExerciseListDTO> Exercises { get; set; } = [];

    public async Task OnGetAsync()
    {
        Exercises = await db.Exercises
        .OrderBy(e => e.Name)
        .Select(e => new ExerciseListDTO
        {
            Id = e.Id,
            Name = e.Name,
            MuscleGroup = e.MuscleGroup
        })
        .ToListAsync();
    }
}
