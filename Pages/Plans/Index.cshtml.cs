using GymTracker.Data;
using GymTracker.Data.Entities;
using GymTracker.DTO;
using GymTracker.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Pages.Plans;

public class IndexModel : PageModel
{
    private readonly IPlanService _planService;

    public IndexModel(IPlanService planService)
    {
        _planService = planService ?? throw new ArgumentNullException(nameof(planService));
    }

    public IList<PlanListDTO> Plans { get; set; } = [];

    public async Task OnGetAsync()
    {
        Plans = await _planService.GetAllPlansAsync();
    }
}
