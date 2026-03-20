using GymTracker.Data;
using GymTracker.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GymTracker.Pages.Exercises;

public class CreateModel(GymTrackerDbContext db) : PageModel
{
    [BindProperty]
    public Exercise Input { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        db.Exercises.Add(Input);
        await db.SaveChangesAsync();

        return RedirectToPage("Detail", new { id = Input.Id });
    }
}
