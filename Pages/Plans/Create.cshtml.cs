using GymTracker.Data;
using GymTracker.Data.Entities;
using GymTracker.DTO;
using GymTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Validation;

namespace GymTracker.Pages.Plans;

public class CreateModel : PageModel
{
    private readonly IPlanService _planService;


    public CreateModel(IPlanService planService)
    {
        _planService = planService ?? throw new ArgumentNullException(nameof(planService));
    }

    [BindProperty]
    public PlanCreateDTO Input { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();
        
        int planId = await _planService.CreatePlanAsync(Input);

        return RedirectToPage("Detail", new { id = planId });
    }
}
